using IAIFWebCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using IAIFWebCatalog.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IAIFWebCatalog.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10; // Number of items per page

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query, string category, string industry, string size, string sortBy, int page = 1)
        {
            // Initialize an empty list for results
            var results = new List<Company>();

            if (string.IsNullOrEmpty(query))
            {
                query = "";
            }

            if (!string.IsNullOrEmpty(query))
            {
                // Search the database for companies matching the query
                var searchTerm = $"%{query}%".ToUpper();
                results = await _context.Companies
                    .Where(c => EF.Functions.Like(c.Name.ToUpper(), searchTerm) ||
                                EF.Functions.Like(c.Description.ToUpper(), searchTerm) ||
                                EF.Functions.Like(c.Services.ToUpper(), searchTerm) ||
                                EF.Functions.Like(c.Tags.ToUpper(), searchTerm))
                    .Include(c => c.Industry)
                    .Include(c => c.Category)
                    .ToListAsync();
            }
            else
            {
                results = await _context.Companies
                    .Include(c => c.Industry)
                    .Include(c => c.Category)
                    .ToListAsync();
            }

            // Filter by category if specified
            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                // Check if category contains multiple values (from checkboxes)
                if (category.Contains(','))
                {
                    var categoryValues = category.Split(',');
                    results = results.Where(c => c.Category != null && 
                                               categoryValues.Contains(c.Category.Name)).ToList();
                }
                else
                {
                    // Try to parse as ID first
                    if (int.TryParse(category, out int categoryId))
                    {
                        results = results.Where(c => c.Category != null && c.Category.Id == categoryId).ToList();
                    }
                    else
                    {
                        // Otherwise filter by name
                        results = results.Where(c => c.Category != null && 
                                                  c.Category.Name.ToLower() == category.ToLower()).ToList();
                    }
                }
            }

            // Filter by industry if specified
            if (!string.IsNullOrEmpty(industry))
            {
                results = results.Where(c => c.Industry != null && 
                                          c.Industry.Name.ToLower() == industry.ToLower()).ToList();
            }

            // Filter by company size if specified
            /* if (!string.IsNullOrEmpty(size))
            {
                var sizeRanges = size.Split('');
                results = results.Where(c => {
                    if (c.EmployeeCount == null) return false;
                    
                    foreach (var range in sizeRanges)
                    {
                        switch (range)
                        {
                            case "1-10":
                                if (c.EmployeeCount >= 1 && c.EmployeeCount <= 10) return true;
                                break;
                            case "11-50":
                                if (c.EmployeeCount >= 11 && c.EmployeeCount <= 50) return true;
                                break;
                            case "51-200":
                                if (c.EmployeeCount >= 51 && c.EmployeeCount <= 200) return true;
                                break;
                            case "201-500":
                                if (c.EmployeeCount >= 201 && c.EmployeeCount <= 500) return true;
                                break;
                            case "501-1000":
                                if (c.EmployeeCount >= 501 && c.EmployeeCount <= 1000) return true;
                                break;
                            case "1000+":
                                if (c.EmployeeCount > 1000) return true;
                                break;
                        }
                    }
                    return false;
                }).ToList();
            }
            */
            

            // Sort results
            switch (sortBy)
            {
                case "name-asc":
                    results = results.OrderBy(c => c.Name).ToList();
                    break;
                case "name-desc":
                    results = results.OrderByDescending(c => c.Name).ToList();
                    break;
                case "newest":
                    results = results.OrderByDescending(c => c.FoundedYear).ToList();
                    break;
                default: // relevance - no change to order
                    break;
            }
            
            // Calculate pagination
            var totalItems = results.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            
            // Ensure page is within valid range
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
            
            // Get the current page of results
            var pagedResults = results
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            
            var categories = await _context.Categories.ToListAsync();
            var industries = await _context.Industries.ToListAsync();
    
            // Add to ViewData
            ViewData["Categories"] = categories;
            ViewData["Industries"] = industries;
            ViewData["Query"] = query;
            ViewData["Category"] = category;
            ViewData["Industry"] = industry;
            ViewData["Size"] = size;
            ViewData["SortBy"] = sortBy ?? "relevance";
            ViewData["ResultCount"] = totalItems;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            
            return View(pagedResults);
        }
    }
}
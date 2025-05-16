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

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query, string category, string industry, string sortBy)
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
            if (category != "all" && !string.IsNullOrEmpty(category))
            {
                results = results.Where(c => c.Category != null && c.Category.Name.ToLower() == category.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(industry))
            {
                results = results.Where(c => c.Industry != null && c.Industry.Name.ToLower() == industry.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                sortBy = "name-asc";
            }

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
            
            var categories = await _context.Categories.ToListAsync();
            var industries = await _context.Industries.ToListAsync();
    
            // Add to ViewData
            ViewData["Categories"] = categories;
            ViewData["Industries"] = industries;
            ViewData["Query"] = query;
            ViewData["Category"] = category;
            ViewData["Industry"] = industry;
            ViewData["SortBy"] = sortBy ?? "relevance";
            ViewData["ResultCount"] = results.Count; 
            return View(results);
        }
    }
}
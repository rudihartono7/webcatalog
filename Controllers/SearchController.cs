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

        public async Task<IActionResult> Index(string query, string category = "all", string sortBy = "relevance")
        {
            // Initialize an empty list for results
            var results = new List<Company>();
            
            if (!string.IsNullOrEmpty(query))
            {
                // Search the database for companies matching the query
                results = await _context.Companies
                    .Where(c => c.Name.Contains(query) || 
                               c.Description.Contains(query) || 
                               c.Services.Contains(query) || 
                               c.Tags.Contains(query))
                    .Include(c => c.Industry)
                    .Include(c => c.Category)
                    .ToListAsync();
                
                // Filter by category if specified
                if (category != "all")
                {
                    results = results.Where(c => c.Category != null && c.Category.Name.ToLower() == category.ToLower()).ToList();
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
            }
            
            ViewData["Query"] = query;
            ViewData["Category"] = category;
            ViewData["SortBy"] = sortBy;
            ViewData["ResultCount"] = results.Count;
            
            return View(results);
        }
    }
}
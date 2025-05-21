using IAIFWebCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using IAIFWebCatalog.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IAIFWebCatalog.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            
            // Get company count for each category
            var categoryCounts = await _context.Companies
                .GroupBy(c => c.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CategoryId ?? 0, x => x.Count);
            
            ViewData["CategoryCounts"] = categoryCounts;
            
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (category == null)
            {
                return NotFound();
            }
            
            // Get companies in this category with more details
            var companies = await _context.Companies
                .Where(c => c.CategoryId == id)
                .Include(c => c.Industry)
                .OrderByDescending(c => c.FoundedYear)
                .ToListAsync();
            
            ViewData["Companies"] = companies;
            ViewData["CompanyCount"] = companies.Count;
            
            // Get total count of companies in this category
            var totalCompanies = await _context.Companies
                .CountAsync(c => c.CategoryId == id);
                
            ViewData["TotalCompanies"] = totalCompanies;
            
            return View(category);
        }
    }
}
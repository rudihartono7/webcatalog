using IAIFWebCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using IAIFWebCatalog.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IAIFWebCatalog.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies
                .Include(c => c.Category)
                .Include(c => c.Industry)
                .ToListAsync();
            
            return View(companies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var company = await _context.Companies
                .Include(c => c.Products)
                .Include(c => c.TeamMembers)
                .Include(c => c.Category)
                .Include(c => c.Industry)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (company == null)
            {
                return NotFound();
            }
            
            // Get related companies for the "Similar Companies" section
            // Companies in the same category or industry
            var relatedCompanies = await _context.Companies
                .Where(c => (c.CategoryId == company.CategoryId || c.IndustryId == company.IndustryId) && c.Id != company.Id)
                .Take(3)
                .ToListAsync();
            
            ViewData["RelatedCompanies"] = relatedCompanies;
            
            return View(company);
        }
    }
}
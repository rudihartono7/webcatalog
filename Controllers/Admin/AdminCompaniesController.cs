using IAIFWebCatalog.Data;
using IAIFWebCatalog.Models;
using IAIFWebCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IAIFWebCatalog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Companies
        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies
                .Include(c => c.Industry)
                .ToListAsync();
            return View(companies);
        }

        // GET: Admin/Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Industry)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Admin/Companies/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IndustryId"] = new SelectList(await _context.Industries.ToListAsync(), "Id", "Name");
            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Name", "Name");
            return View();
        }

        // POST: Admin/Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["IndustryId"] = new SelectList(await _context.Industries.ToListAsync(), "Id", "Name", company.IndustryId);
            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Name", "Name", company.Category);
            return View(company);
        }

        // GET: Admin/Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            
            ViewData["IndustryId"] = new SelectList(await _context.Industries.ToListAsync(), "Id", "Name", company.IndustryId);
            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Name", "Name", company.Category);
            return View(company);
        }

        // POST: Admin/Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["IndustryId"] = new SelectList(await _context.Industries.ToListAsync(), "Id", "Name", company.IndustryId);
            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Name", "Name", company.Category);
            return View(company);
        }

        // GET: Admin/Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Industry)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Admin/Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Companies/AddProduct/5
        public async Task<IActionResult> AddProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                CompanyId = company.Id,
                CompanyName = company.Name
            };

            return View(viewModel);
        }

        // POST: Admin/Companies/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    ImageUrl = viewModel.ImageUrl,
                    CompanyId = viewModel.CompanyId
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = viewModel.CompanyId });
            }

            return View(viewModel);
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}
using IAIFWebCatalog.Data;
using IAIFWebCatalog.Models;
using IAIFWebCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IAIFWebCatalog.Controllers
{
    [Authorize(Roles = "CompanyAdmin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
            
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

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
            
            if (company == null || company.Id != viewModel.CompanyId)
            {
                return Forbid();
            }

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
                return RedirectToAction("Profile", "Account");
            }

            viewModel.CompanyName = company.Name;
            return View(viewModel);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var product = await _context.Products
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (product == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
            if (company == null || company.Id != product.CompanyId)
            {
                return Forbid();
            }

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CompanyId = product.CompanyId,
                CompanyName = product.Company.Name
            };

            return View(viewModel);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
            
            if (company == null || company.Id != viewModel.CompanyId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products.FindAsync(id);
                    if (product == null)
                    {
                        return NotFound();
                    }

                    product.Name = viewModel.Name;
                    product.Description = viewModel.Description;
                    product.Price = viewModel.Price;
                    product.ImageUrl = viewModel.ImageUrl;

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile", "Account");
            }

            viewModel.CompanyName = company.Name;
            return View(viewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var product = await _context.Products
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (product == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
            if (company == null || company.Id != product.CompanyId)
            {
                return Forbid();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
            if (company == null || company.Id != product.CompanyId)
            {
                return Forbid();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Account");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
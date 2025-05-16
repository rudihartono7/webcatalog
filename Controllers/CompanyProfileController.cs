using IAIFWebCatalog.Data;
using IAIFWebCatalog.Models;
using IAIFWebCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IAIFWebCatalog.Controllers
{
    [Authorize(Roles = "CompanyAdmin")]
    public class CompanyProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CompanyProfileController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: CompanyProfile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies
                .Include(c => c.Products)
                .Include(c => c.Category)
                .Include(c => c.Industry)
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
            if (company == null)
            {
                return NotFound();
            }
            
            var model = new ProfileViewModel
            {
                User = user,
                Company = company
            };
            
            return View(model);
        }

        // GET: CompanyProfile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
            if (company == null)
            {
                return NotFound();
            }
            
            // Populate dropdown lists for categories and industries
            ViewData["CategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", company.CategoryId);
            ViewData["IndustryId"] = new SelectList(await _context.Industries.ToListAsync(), "Id", "Name", company.IndustryId);
            
            return View(company);
        }

        // POST: CompanyProfile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Company company)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var existingCompany = await _context.Companies
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
            if (existingCompany == null)
            {
                return NotFound();
            }
            
            // Ensure we're only updating the company owned by the current user
            if (existingCompany.Id != company.Id)
            {
                return Forbid();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Update only allowed fields
                    existingCompany.Name = company.Name;
                    existingCompany.Description = company.Description;
                    existingCompany.Address = company.Address;
                    existingCompany.City = company.City;
                    existingCompany.Country = company.Country;
                    existingCompany.Phone = company.Phone;
                    existingCompany.Email = company.Email;
                    existingCompany.Website = company.Website;
                    existingCompany.EmployeeCount = company.EmployeeCount;
                    existingCompany.FoundedYear = company.FoundedYear;
                    existingCompany.Services = company.Services;
                    existingCompany.Tags = company.Tags;
                    existingCompany.ImageUrl = company.ImageUrl;
                    existingCompany.CategoryId = company.CategoryId;
                    existingCompany.IndustryId = company.IndustryId;
                    existingCompany.SocialMedia = company.SocialMedia;
                    
                    _context.Update(existingCompany);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Company profile updated successfully.";
                    return RedirectToAction(nameof(Index));
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
            }
            
            // If we got this far, something failed, redisplay form
            ViewData["CategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", company.CategoryId);
            ViewData["IndustryId"] = new SelectList(await _context.Industries.ToListAsync(), "Id", "Name", company.IndustryId);
            return View(company);
        }

        // GET: CompanyProfile/Products
        public async Task<IActionResult> Products()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
            if (company == null)
            {
                return NotFound();
            }
            
            return View(company);
        }

        // GET: CompanyProfile/AddProduct
        public async Task<IActionResult> AddProduct()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
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

        // POST: CompanyProfile/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
            if (company == null)
            {
                return NotFound();
            }
            
            // Ensure we're adding a product to the company owned by the current user
            if (company.Id != viewModel.CompanyId)
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
                    CompanyId = company.Id
                };
                
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Product added successfully.";
                return RedirectToAction(nameof(Products));
            }
            
            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        // GET: CompanyProfile/EditProduct/5
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
            if (company == null)
            {
                return NotFound();
            }
            
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == company.Id);
                
            if (product == null)
            {
                return NotFound();
            }
            
            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CompanyId = company.Id,
                CompanyName = company.Name
            };
            
            return View(viewModel);
        }

        // POST: CompanyProfile/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
            if (company == null)
            {
                return NotFound();
            }
            
            // Ensure we're editing a product owned by the company of the current user
            if (company.Id != viewModel.CompanyId)
            {
                return Forbid();
            }
            
            if (ModelState.IsValid)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == viewModel.Id && p.CompanyId == company.Id);
                    
                if (product == null)
                {
                    return NotFound();
                }
                
                product.Name = viewModel.Name;
                product.Description = viewModel.Description;
                product.Price = viewModel.Price;
                product.ImageUrl = viewModel.ImageUrl;
                
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Products));
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
            }
            
            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        // GET: CompanyProfile/DeleteProduct/5
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
            if (company == null)
            {
                return NotFound();
            }
            
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == company.Id);
                
            if (product == null)
            {
                return NotFound();
            }
            
            return View(product);
        }

        // POST: CompanyProfile/DeleteProduct/5
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                
            if (company == null)
            {
                return NotFound();
            }
            
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == company.Id);
                
            if (product == null)
            {
                return NotFound();
            }
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Products));
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
        
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
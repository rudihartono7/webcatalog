using IAIFWebCatalog.Data;
using IAIFWebCatalog.Models;
using IAIFWebCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using IAIFWebCatalog.Services;

namespace IAIFWebCatalog.Controllers
{
    [Authorize(Roles = "CompanyAdmin")]
    public class CompanyProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly AzureStorageService _azureStorageService;

        // Update the constructor to inject AzureStorageService
        public CompanyProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, AzureStorageService azureStorageService)
        {
            _context = context;
            _userManager = userManager;
            _azureStorageService = azureStorageService;
        }

        // Update the Edit POST action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Services,Tags,Address,City,Country,Phone,Email,Website,FoundedYear,EmployeeCount,CategoryId,IndustryId,SocialMedia,BrochureUrl,AdminUserId")] Company company, IFormFile CompanyImage, IFormFile CompanyBrochure)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            // Check if the current user is the admin of this company
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || company.AdminUserId != currentUser.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle company image upload
                    if (CompanyImage != null && CompanyImage.Length > 0)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(company.ImageUrl))
                        {
                            // Extract the file name from the URL
                            string oldFileName = Path.GetFileName(new Uri(company.ImageUrl).LocalPath);
                            await _azureStorageService.DeleteFileAsync(oldFileName);
                        }

                        // Upload new image
                        using (var stream = CompanyImage.OpenReadStream())
                        {
                            string fileName = await _azureStorageService.UploadFileAsync(stream, CompanyImage.FileName, CompanyImage.ContentType);
                            company.ImageUrl = _azureStorageService.GetBlobUrl(fileName);
                        }
                    }

                    // Handle company brochure upload
                    if (CompanyBrochure != null && CompanyBrochure.Length > 0)
                    {
                        // Delete old brochure if exists
                        if (!string.IsNullOrEmpty(company.BrochureUrl))
                        {
                            // Extract the file name from the URL
                            string oldFileName = Path.GetFileName(new Uri(company.BrochureUrl).LocalPath);
                            await _azureStorageService.DeleteFileAsync(oldFileName);
                        }

                        // Upload new brochure
                        using (var stream = CompanyBrochure.OpenReadStream())
                        {
                            string fileName = await _azureStorageService.UploadFileAsync(stream, CompanyBrochure.FileName, CompanyBrochure.ContentType);
                            company.BrochureUrl = _azureStorageService.GetBlobUrl(fileName);
                        }
                    }

                    _context.Update(company);
                    await _context.SaveChangesAsync();
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
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", company.CategoryId);
            ViewData["Industries"] = new SelectList(_context.Industries, "Id", "Name", company.IndustryId);
            return View(company);
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
                    Price = viewModel.Price ?? 0,
                    CompanyId = company.Id,
                    ProductUrl = viewModel.ProductUrl,
                    UserCategories = string.Join(",", viewModel.SelectedUserCategories),
                };
                
                // Handle image file upload
                if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                {
                    using (var stream = viewModel.ImageFile.OpenReadStream())
                    {
                        string fileName = await _azureStorageService.UploadFileAsync(
                            stream, 
                            viewModel.ImageFile.FileName, 
                            viewModel.ImageFile.ContentType);
                        product.ImageUrl = _azureStorageService.GetBlobUrl(fileName);
                    }
                }
                else if (!string.IsNullOrEmpty(viewModel.ImageUrl))
                {
                    // If no new file but URL provided, keep the existing URL
                    product.ImageUrl = viewModel.ImageUrl;
                }
                
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
                ProductUrl = product.ProductUrl,
                CompanyId = company.Id,
                CompanyName = company.Name,
                ImageUrl = product.ImageUrl,
                SelectedUserCategories = product.UserCategories?.Split(',').Select(c => c.Trim()).ToList() ?? new List<string>()
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
                product.Price = viewModel.Price ?? 0;
                product.ProductUrl = viewModel.ProductUrl;
                product.UserCategories = viewModel.SelectedUserCategories != null ? string.Join(",", viewModel.SelectedUserCategories) : null;
                
                // Handle image file upload
                if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        // Extract the file name from the URL
                        string oldFileName = Path.GetFileName(new Uri(product.ImageUrl).LocalPath);
                        await _azureStorageService.DeleteFileAsync(oldFileName);
                    }
                    
                    // Upload new image
                    using (var stream = viewModel.ImageFile.OpenReadStream())
                    {
                        string fileName = await _azureStorageService.UploadFileAsync(
                            stream, 
                            viewModel.ImageFile.FileName, 
                            viewModel.ImageFile.ContentType);
                        product.ImageUrl = _azureStorageService.GetBlobUrl(fileName);
                    }
                }
                // If no new file uploaded, keep the existing ImageUrl
                
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

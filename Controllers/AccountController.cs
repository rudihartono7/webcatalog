using IAIFWebCatalog.Data;
using IAIFWebCatalog.Models;
using IAIFWebCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IAIFWebCatalog.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the company already exists
                var existingCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Name == model.CompanyName);
                if (existingCompany != null)
                {
                    ModelState.AddModelError("CompanyName", "A company with this name already exists.");
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                // Create the company
                var company = new Company
                {
                    Name = model.CompanyName,
                    EmployeeCount = model.CompanySize,
                    // Set other company properties as needed
                };
                
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                
                // Create the user
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CompanyId = company.Id.ToString()
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    // Assign CompanyAdmin role
                    await _userManager.AddToRoleAsync(user, "CompanyAdmin");
                    
                    // Update company with admin user ID
                    company.AdminUserId = user.Id;
                    _context.Companies.Update(company);
                    await _context.SaveChangesAsync();
                    
                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            // If we got this far, something failed, redisplay form
            TempData["ShowRegistrationPopup"] = true;
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            
            // Check if user is a company admin
            if (await _userManager.IsInRoleAsync(user, "CompanyAdmin"))
            {
                // Redirect to the CompanyProfile controller
                return RedirectToAction("Index", "CompanyProfile");
            }
            
            var company = await _context.Companies
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.AdminUserId == user.Id);
                    
            var model = new ProfileViewModel
            {
                User = user,
                Company = company
            };
            
            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
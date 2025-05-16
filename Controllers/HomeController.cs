using IAIFWebCatalog.Models;
using IAIFWebCatalog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IAIFWebCatalog.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IAIFWebCatalog.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                // Get categories from database instead of hardcoded list
                FeaturedCategories = await _context.Categories
                    .Take(6)  // Take the first 6 categories
                    .ToListAsync(),
                    
                // Get random companies from the database
                FeaturedCompanies = await _context.Companies
                    .OrderBy(c => Guid.NewGuid()) // Random ordering
                    .Take(5) // Take 5 random companies
                    .ToListAsync()
            };
            
            return View(viewModel);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Cookies()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            // In a real application, you would save the email to a database
            // For now, we'll just redirect back to the home page with a success message
            TempData["SuccessMessage"] = "Thank you for subscribing to our newsletter!";
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> RegisterCompany()
        {
            // Get categories for dropdown
            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCompany(RegisterCompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new user
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = "Admin",
                    LastName = "",
                };

                // Create the user account
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Create a new company
                    var company = new Company
                    {
                        Name = model.CompanyName,
                        EmployeeCount = model.CompanySize,
                        AdminUserId = user.Id,
                        CategoryId = model.CategoryId
                    };

                    // Save the company to the database
                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();

                    // Assign CompanyAdmin role
                    await _userManager.AddToRoleAsync(user, "CompanyAdmin");

                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to home page or company profile
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If validation fails, repopulate the categories dropdown
            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.CategoryId);
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}

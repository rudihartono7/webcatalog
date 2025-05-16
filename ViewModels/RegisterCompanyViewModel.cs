using System.ComponentModel.DataAnnotations;

namespace IAIFWebCatalog.ViewModels
{
    public class RegisterCompanyViewModel
    {
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        
        [Required]
        [Display(Name = "Company Size")]
        public string CompanySize { get; set; }

        [Required]
        [Display(Name = "Company Category")]
        public int? CategoryId { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        [Display(Name = "Accept Terms")]
        public bool AcceptTerms { get; set; }
    }
}
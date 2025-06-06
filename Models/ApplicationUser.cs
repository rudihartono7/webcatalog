using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IAIFWebCatalog.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        public string? CompanyId { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
    }
}
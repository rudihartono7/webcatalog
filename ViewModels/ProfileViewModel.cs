using IAIFWebCatalog.Models;
using Microsoft.AspNetCore.Http;

namespace IAIFWebCatalog.ViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public Company Company { get; set; }
        
        // New properties for file uploads
        public IFormFile? CompanyImageFile { get; set; }
        public IFormFile? ProfilePdfFile { get; set; }
    }
}
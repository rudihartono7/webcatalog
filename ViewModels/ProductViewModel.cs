using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace IAIFWebCatalog.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        public decimal? Price { get; set; }
        
        // Replace ImageUrl with file upload
        public IFormFile? ImageFile { get; set; }
        
        // Keep this for displaying existing images
        public string? ImageFileName { get; set; }
        
        public string? ImageUrl { get; set; }

        // New field for product URL
        [Url]
        public string? ProductUrl { get; set; }
        
        // New field for user categories
        public string? UserCategories { get; set; }
        
        // List of available user categories for dropdown/multi-select
        public List<string> AvailableUserCategories { get; set; } = new List<string> 
        { 
            "Swasta", 
            "BUMN", 
            "Kementerian/Pemerinthan", 
            "Perusahaan Internasional"
        };
        
        // Selected user categories (for multi-select)
        public List<string> SelectedUserCategories { get; set; } = new List<string>();
        
        public int CompanyId { get; set; }
        
        public string? CompanyName { get; set; }
    }
}
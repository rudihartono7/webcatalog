using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAIFWebCatalog.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        public decimal? Price { get; set; }

        public string? ImageUrl { get; set; }

        public string? ImageAltText { get; set; } // New field for alt text
        
        // Removing ImageUrl and replacing with ImageFileName
        public string? ImageFileName { get; set; }
        
        // New field for product URL
        public string? ProductUrl { get; set; }
        
        // New field for user category (comma-separated values)
        public string? UserCategories { get; set; }
        
        public int CompanyId { get; set; }
        
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAIFWebCatalog.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }
        
        public string ImageUrl { get; set; }
        
        [Required]
        public int CompanyId { get; set; }

        public string Tags { get; set; } = String.Empty;
        
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}
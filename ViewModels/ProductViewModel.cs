using System.ComponentModel.DataAnnotations;

namespace IAIFWebCatalog.ViewModels
{
    public class ProductViewModel
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
        
        public string CompanyName { get; set; }
    }
}
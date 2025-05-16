using System.ComponentModel.DataAnnotations;

namespace IAIFWebCatalog.Models
{
    public class Industry
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        public List<Company> Companies { get; set; } = new List<Company>();
    }
}
using IAIFWebCatalog.Models;

namespace IAIFWebCatalog.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> FeaturedCategories { get; set; } = new List<Category>();
        public List<Company> FeaturedCompanies { get; set; } = new List<Company>();
    }
}
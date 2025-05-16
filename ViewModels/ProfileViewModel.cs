using IAIFWebCatalog.Models;

namespace IAIFWebCatalog.ViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public Company Company { get; set; }
    }
}
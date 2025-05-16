namespace IAIFWebCatalog.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string BackgroundClass { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IAIFWebCatalog.Models
{
    public class Company
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public string? Address { get; set; }
        
        public int FoundedYear { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Services { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        
        public string? EmployeeCount { get; set; }
        
        public Dictionary<string, string>? SocialMedia { get; set; }
        
        public List<Product> Products { get; set; } = new List<Product>();

        public List<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        // New properties for authentication
        public string? AdminUserId { get; set; }
        
        [ForeignKey("AdminUserId")]
        public ApplicationUser? AdminUser { get; set; }
        
        public int? IndustryId { get; set; }
        
        [ForeignKey("IndustryId")]
        public Industry? Industry { get; set; }
        
        public int? CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
    
    public class TeamMember
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
    }
}
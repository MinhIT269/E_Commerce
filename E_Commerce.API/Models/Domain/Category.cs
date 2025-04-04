using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } 

        [Required, MaxLength(100)]
        public string? CategoryName { get; set; }

        public int? ParentCategoryId { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<ProductCategory>? ProductCategories { get; set; }
    }
}

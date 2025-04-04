using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class ProductImage
    {
        [Key]
        public Guid ProductImageId { get; set; }

        public Guid ProductId { get; set; }

        [Required, MaxLength(255)]
        public string? ImageUrl { get; set; } 

        public Product? Product { get; set; }
    }
}

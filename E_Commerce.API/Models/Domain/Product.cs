using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }  

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public string? MetaDescription { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PromotionPrice { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public int Warranty { get; set; }
       
        [Required, MaxLength(255)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime Hot { get; set; }

        [Required]
        public Guid BrandId { get; set; }

        public Brand? Brand { get; set; }

        public ICollection<ProductCategory>? ProductCategories { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        public ICollection<ProductImage>? ProductImages { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }
    }
}

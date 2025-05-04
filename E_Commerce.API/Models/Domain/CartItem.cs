using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public Guid CartId { get; set; } 

        public Cart? Cart { get; set; }

        [Required]
        public Guid ProductId { get; set; } 

        public Product? Product { get; set; }
    }
} 

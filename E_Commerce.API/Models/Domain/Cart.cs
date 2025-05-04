using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public User? User { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }
    }
}

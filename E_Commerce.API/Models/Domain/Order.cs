using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } 

        [Required]
        public string UserId { get; set; }  = string.Empty;

        public DateTime? OrderDate { get; set; }

        [Required, Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18, 3)")]
        public decimal TotalAmount { get; set; }

        [Required, MaxLength(50)]
        public string? Status { get; set; }

        public User? User { get; set; }   // Navigation property

        public ICollection<OrderDetail>? OrderDetails { get; set; }  // Navigation property for related OrderDetails

        public Guid? PromotionId { get; set; }    // Foreign Key to Promotion

        [Range(0, 100)]
        public decimal? DiscountPercentage { get; set; }

        public Promotion? Promotion { get; set; }

        public Transaction? Transaction { get; set; }
    }
}

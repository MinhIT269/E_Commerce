using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; }

        public DateTime TransactionDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 3)")]  
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]  
        public string? Status { get; set; }

        [StringLength(500)]  
        public string? TransactionDetails { get; set; }

        [Required]
        public Guid OrderId { get; set; } 

        public Order? Order { get; set; }

        [Required]
        public Guid PaymentMethodId { get; set; } 

        public PaymentMethod? PaymentMethod { get; set; }
    }
}

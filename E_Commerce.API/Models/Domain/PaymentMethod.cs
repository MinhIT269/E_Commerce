using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class PaymentMethod
    {
        [Key]
        public Guid PaymentMethodId { get; set; }

        [Required, MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(400)]
        public string? Description { get; set; }

        public ICollection<PaymentDetail>? PaymentDetails { get; set; } 

        public ICollection<Transaction>? Transactions { get; set; }  
    }
}

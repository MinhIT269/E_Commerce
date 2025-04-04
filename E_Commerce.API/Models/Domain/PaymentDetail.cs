using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class PaymentDetail
    {
        [Key]
        public Guid PaymentDetailId { get; set; }

        public string? CardNumber { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string? CardHolderName { get; set; }

        [Required]
        public string UserId { get; set; }   

        public User? User { get; set; }

        [Required]
        public Guid PaymentMethodId { get; set; }  

        public PaymentMethod? PaymentMethod { get; set; }
    }
}

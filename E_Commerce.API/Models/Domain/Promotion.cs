using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class Promotion
    {
        [Key]
        public Guid PromotionId { get; set; }

        [Required, MaxLength(50)]
        public string? Code { get; set; }

        [Required, Range(0, 100)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Percentage { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int MaxUsage { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}

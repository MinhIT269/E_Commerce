using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class Brand
    {
        [Key]
        public Guid BrandId { get; set; }

        [Required, MaxLength(150)]
        public string? BrandName { get; set; }

        [MaxLength(900)]
        public string? Description { get; set; }

        public string? Logo { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; }   

        [Required]
        public Guid ProductId { get; set; }  

        [Required]
        public string UserId { get; set; }      

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; }

        public Product? Product { get; set; }  

        public User? User { get; set; }       
    }
}

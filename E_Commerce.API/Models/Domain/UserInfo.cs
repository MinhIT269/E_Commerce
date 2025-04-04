using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Domain
{
    public class UserInfo
    {
        [Key]
        public Guid UserInfoId { get; set; }  // Primary Key

        [Required]
        public string UserId { get; set; }  // Foreign Key

        [Required, StringLength(250)]
        public string? Address { get; set; }  // Không cần nullable

        [Required, StringLength(50)]
        public string? FirstName { get; set; }

        [Required, StringLength(50)]
        public string? LastName { get; set; }

        public bool Gender { get; set; } 

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace E_Commerce.API.Models.Domain
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
        
        public ICollection<Review>? Reviews { get; set; }

        public ICollection<PaymentDetail>? PaymentDetails { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Cart> Carts { get; set; }

        public UserInfo? UserInfo { get; set; }
    }
}

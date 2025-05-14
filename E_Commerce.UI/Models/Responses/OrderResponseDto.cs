using E_Commerce.UI.Models.Requests;

namespace E_Commerce.UI.Models.Responses
{
    public class OrderResponseDto
    {
        public UserInfoDto UserInfo { get; set; } = new();
        public Guid OrderId { get; set; }        // Foreign Key
        public string? Status { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public List<ProductDetailOrder>? Products { get; set; } // Navigation property
    }
}

namespace E_Commerce.API.Models.Responses
{
    public class OrderDetailResponseDto
    {
        public UserInfoDto UserInfo { get; set; } = new();
        public Guid OrderId { get; set; } 
        public string Email {  get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public List<ProductDetailOrder>? Products { get; set; } 
    }
}

using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Models.Requests
{
    public class OrderRequestDto
    {
        public string Description { get; set; } = string.Empty;
        public List<OrderItemRequestDto> Items { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public string? PromotionCode { get; set; }
        public UserInfoDto UserInfo { get; set; } = new();
    }
}

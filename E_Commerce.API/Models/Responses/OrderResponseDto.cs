namespace E_Commerce.API.Models.Responses
{
    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "pending";
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}

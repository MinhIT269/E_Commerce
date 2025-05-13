namespace E_Commerce.API.Models.Responses
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }  
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public int totalProduct { get; set; }
        public string? PaymentMethod { get; set; }
        public Guid PromotionId { get; set; }  
        public string? Code { get; set; }
    }
}

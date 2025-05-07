namespace E_Commerce.API.Models.Requests
{
    public class VnPaymentRequestDto
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

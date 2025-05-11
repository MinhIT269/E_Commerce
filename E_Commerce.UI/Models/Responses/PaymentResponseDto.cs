namespace E_Commerce.UI.Models.Responses
{
    public class PaymentResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public string StatusClass { get; set; } = string.Empty;
    }
}

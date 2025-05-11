namespace E_Commerce.UI.Models.Responses
{
    public class PromotionResponseDto
    {
        public Guid PromotionId { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxUsage { get; set; }
        public bool IsExpired => EndDate < DateTime.UtcNow;
    }
}

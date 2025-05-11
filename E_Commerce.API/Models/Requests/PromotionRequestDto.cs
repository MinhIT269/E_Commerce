namespace E_Commerce.API.Models.Requests
{
    public class PromotionRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxUsage { get; set; }
    }
}

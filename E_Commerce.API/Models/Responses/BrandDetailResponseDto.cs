namespace E_Commerce.API.Models.Responses
{
    public class BrandDetailResponseDto
    {
        public Guid BrandId { get; set; }
        public string? BrandName { get; set; }
        public int StockAvailable { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
    }
}

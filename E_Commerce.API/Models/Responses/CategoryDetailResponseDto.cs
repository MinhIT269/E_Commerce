namespace E_Commerce.API.Models.Responses
{
    public class CategoryDetailResponseDto
    {
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int ProductStock { get; set; }
    }
}

namespace E_Commerce.UI.Models.Requests
{
    public class CategoryRequestDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public string? Description { get; set; }
    }
}

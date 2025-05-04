namespace E_Commerce.API.Models.Requests
{
    public class RemoveCartItemRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public Guid CartItemId { get; set; }
    }
}

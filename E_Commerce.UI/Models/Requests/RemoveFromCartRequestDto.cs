namespace E_Commerce.UI.Models.Requests
{
    public class RemoveFromCartRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public Guid CartItemId { get; set; }
    }
}

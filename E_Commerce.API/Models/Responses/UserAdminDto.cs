namespace E_Commerce.API.Models.Responses
{
    public class UserAdminDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public int Order { get; set; }
    }
}

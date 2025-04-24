namespace E_Commerce.API.Models.Responses
{
    public class RegisterResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
    }
}

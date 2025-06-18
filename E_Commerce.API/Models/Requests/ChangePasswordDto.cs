namespace E_Commerce.API.Models.Requests
{
    public class ChangePasswordDto
    {
        public string Id {  get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

namespace E_Commerce.UI.Models.Requests
{
    public class UserInfoDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool Gender { get; set; } = true;
    }
}

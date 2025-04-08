namespace E_Commerce.API.Models.Response
{
    public class LoginResponseDto
    {
        public bool IsLogedIn { get; set; } = false;

        public string? RefreshToken { get; set; }

        public string? JwtToken { get; set; }
    }
}

namespace E_Commerce.API.Models.Requests
{
    public class RefreshTokenDto
    {
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; } 
    }
}

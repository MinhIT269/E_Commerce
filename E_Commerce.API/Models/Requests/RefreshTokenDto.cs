﻿namespace E_Commerce.API.Models.Requests
{
    public class RefreshTokenDto
    {
        public string JwtToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty ;
    }
}

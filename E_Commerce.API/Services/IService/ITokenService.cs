using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace E_Commerce.API.Services.IService
{
    public interface ITokenService
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
        string CreateRefreshToken();
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}

using Microsoft.AspNetCore.Identity;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}

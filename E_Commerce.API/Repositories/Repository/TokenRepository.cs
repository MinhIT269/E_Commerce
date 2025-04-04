using E_Commerce.API.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.API.Repositories.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;
        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateJWTToken(IdentityUser user, List<string> roles)
        {
            // Create claims
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])); //khoa doi xung
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // thuat toan ma hoa
            var token = new JwtSecurityToken(
                _configuration["JWT:Issuer"], // ai phat hanh
                _configuration["JWT:Audience"], // ai su dung
                claims, // danh sach chua thong tin nguoi dung
                expires: DateTime.Now.AddMinutes(15), 
                signingCredentials: credentials); // chu ki bao mat
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

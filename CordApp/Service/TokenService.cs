using CordApp.Data;
using CordApp.Interface;
using CordApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CordApp.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        private readonly ApplicationDBContext _dbContext;
        public TokenService(IConfiguration config, ApplicationDBContext dBContext)
        {
            _configuration = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
            _dbContext = dBContext;
        }
        public string CreateToken(AppUser user)
        {
            var userRole = _dbContext.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
            var roleName = "";

            if (userRole.Count > 0)
            {
                roleName = _dbContext.Roles.Where(r => r.Id == userRole[0]).Select(r => r.Name).FirstOrDefault();
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(ClaimTypes.Role, roleName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

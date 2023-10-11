using ApiEcomm.Models;
using System.Security.Claims;

namespace ApiEcomm.Extensions
{
    public static class RoleClaimsExtension
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Email)
            };
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Slug)));
            return claims;
        }
    }
}

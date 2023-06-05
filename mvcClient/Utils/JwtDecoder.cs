using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace mvcClient.Utils
{
    public static class JwtDecoder
    {
        public static JwtSecurityToken? DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            return jwtSecurityToken;
        }

        public static DateTime GetExpirationDate(string token)
        {
            var jwtSecurityToken = DecodeToken(token);
            return jwtSecurityToken.ValidTo;
        }

        public static List<Claim> GetClaims(string token)
        {
              var jwtSecurityToken = DecodeToken(token);
            return jwtSecurityToken.Claims.ToList();
        }
    }
}

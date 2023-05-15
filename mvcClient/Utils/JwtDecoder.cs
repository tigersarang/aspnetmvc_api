using System.IdentityModel.Tokens.Jwt;

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
    }
}

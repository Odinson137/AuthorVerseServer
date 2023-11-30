using System.IdentityModel.Tokens.Jwt;

namespace AuthorVerseForum.Services
{
    public class UncodingJwtoken
    {
        public string GetUserId(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            string? userId = jwtSecurityToken.Claims.First().Value;
            return userId;
        }
    }
}

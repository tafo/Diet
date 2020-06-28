using Microsoft.IdentityModel.Tokens;

namespace Diet.Api.Infrastructure.Security
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
    }
}
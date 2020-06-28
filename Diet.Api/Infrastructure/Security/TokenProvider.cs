using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Diet.Api.Domain;
using Microsoft.Extensions.Options;

namespace Diet.Api.Infrastructure.Security
{
    public class TokenProvider : ITokenProvider
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IClock _clock;
        public TokenProvider(IOptions<JwtOptions> jwtIssuerOptions, IClock clock)
        {
            _jwtOptions = jwtIssuerOptions.Value;
            _clock = clock;
        }

        public string Create(Account account)
        {
            var currentTime = _clock.UtcNow;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Role, account.Role), 
            };

            // ToDo : Update expiration
            var jwtSecurityToken = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                currentTime,
                currentTime.Add(TimeSpan.FromMinutes(300)),
                _jwtOptions.SigningCredentials);

            var handler = new JwtSecurityTokenHandler();
            var encodedJwtToken = handler.WriteToken(jwtSecurityToken);
            return encodedJwtToken;
        }
    }
}
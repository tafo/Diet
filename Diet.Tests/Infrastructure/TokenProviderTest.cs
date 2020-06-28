using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Diet.Api.Domain;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace Diet.Tests.Infrastructure
{
    public class TokenProviderTest
    {
        [Fact]
        public void Create_Valid_Token()
        {
            // Arrange
            const string issuer = "DietIssuer";
            const string audience = "DietAudience";
            const string key = "8bIpdabhJwjOmwa2e6Gzjtpsuk5H0ECk";
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var jwtOptions = new JwtOptions
                {Issuer = issuer, Audience = audience, SigningCredentials = signingCredentials};

            var mockJwtOptions = new Mock<IOptions<JwtOptions>>();
            mockJwtOptions.Setup(x => x.Value).Returns(jwtOptions);

            var current = DateTime.UtcNow;
            var validTo = current.AddMinutes(300);
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow).Returns(current);

            var provider = new TokenProvider(mockJwtOptions.Object, mockClock.Object);
            var id = Guid.NewGuid();

            // Act
            var rawToken = provider.Create(new Account {Id = id, Role = Role.Admin});
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.ReadJwtToken(rawToken);

            // Assert
            Assert.Equal(issuer, securityToken.Issuer);
            Assert.Equal(audience, securityToken.Audiences.First());
            Assert.Equal(signingCredentials.Algorithm, securityToken.SignatureAlgorithm);
            Assert.Equal(id.ToString(), securityToken.Claims.First().Value);
            
            Assert.Equal(current.Date, securityToken.ValidFrom.Date);
            Assert.Equal(current.Hour, securityToken.ValidFrom.Hour);
            Assert.Equal(current.Minute, securityToken.ValidFrom.Minute);
            Assert.Equal(current.Second, securityToken.ValidFrom.Second);

            Assert.Equal(validTo.Date, securityToken.ValidTo.Date);
            Assert.Equal(validTo.Hour, securityToken.ValidTo.Hour);
            Assert.Equal(validTo.Minute, securityToken.ValidTo.Minute);
            Assert.Equal(validTo.Second, securityToken.ValidTo.Second);
        }
    }
}
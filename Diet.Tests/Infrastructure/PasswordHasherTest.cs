using Diet.Api.Infrastructure.Security;
using Xunit;

namespace Diet.Tests.Infrastructure
{
    public class PasswordHasherTest
    {
        [Fact]
        public void FailureCase()
        {
            // Arrange
            var hasher = new PasswordHasher();

            // Act
            var hashedPassword = hasher.HashPassword("password");
            var result = hasher.VerifyHashedPassword("password2", hashedPassword);

            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void SuccessCase()
        {
            // Arrange
            var hasher = new PasswordHasher();

            // Act
            var hashedPassword = hasher.HashPassword("password");
            var result = hasher.VerifyHashedPassword("password", hashedPassword);

            // Assert
            Assert.True(result);
        }
    }
}
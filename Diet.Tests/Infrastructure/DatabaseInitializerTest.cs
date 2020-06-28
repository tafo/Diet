using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diet.Tests.Infrastructure
{
    [Collection(nameof(TestFixtureCollection))]
    public class DatabaseInitializerTest
    {
        private readonly TestFixture _fixture;

        public DatabaseInitializerTest(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Should_Have_A_Default_Admin_Account()
        {
            // Arrange
            _fixture.StartScope();

            // Act
            var account = await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleAsync());

            // Assert
            Assert.NotNull(account);
        }
    }
}
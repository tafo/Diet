using System.Threading.Tasks;
using Diet.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Diet.Tests.Infrastructure
{
    [Collection(nameof(TestFixtureCollection))]
    public class ScopeTest
    {
        private readonly TestFixture _fixture;
        public ScopeTest(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Test_Methods_Should_Use_Empty_Database()
        {
            {
                // Arrange
                _fixture.StartScope();

                // Act
                await _fixture.InsertAsync(new Meal());
                var count = await _fixture.ExecuteDbContextAsync(db => db.Meals.CountAsync());

                // Assert
                Assert.Equal(1, count);
            }
            {
                // Arrange
                _fixture.StartScope();

                // Act
                var count = await _fixture.ExecuteDbContextAsync(db => db.Meals.CountAsync());

                // Assert
                Assert.Equal(0, count);
            }
        }
    }
}
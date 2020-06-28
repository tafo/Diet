using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Diet.Api.Domain;
using Diet.Api.Features.Account;
using Diet.Api.Infrastructure.ExceptionHandling;
using Diet.Api.Infrastructure.Providers;
using Diet.Api.Infrastructure.Security;
using Diet.Tests.EnvironmentServices;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Index = Diet.Api.Features.Account.Index;

namespace Diet.Tests.Features
{
    [Collection(nameof(TestFixtureCollection))]
    public class AccountTest
    {
        private readonly TestFixture _fixture;

        public AccountTest(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Should_Create_Account()
        {
            // Arrange
            _fixture.StartScope();
            var request = new Create.Request
            {
                Email = "email@domain.com",
                Password = Guid.NewGuid().ToString()
            };
            var passwordHasher = _fixture.GetService<IPasswordHasher>();

            // Act
            var response = await _fixture.SendAsync(request);
            var account =
                await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email));

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response.Token);
            Assert.NotNull(account);
            Assert.Equal(request.Email, account.Email);
            Assert.True(passwordHasher.VerifyHashedPassword(request.Password, account.PasswordHash));
        }

        [Fact]
        public async Task Throws_Create_Exception_For_Existing_Email()
        {
            // Arrange
            _fixture.StartScope();
            var request = new Create.Request
            {
                Email = "email@domain.com",
                Password = Guid.NewGuid().ToString()
            };

            {   // Create Account
                var response = await _fixture.SendAsync(request);

                // Inner-Assert
                Assert.NotNull(response);
                Assert.NotEmpty(response.Token);
            }

            {
                // Act & Assert
                var exception = await Assert.ThrowsAsync<RestException>(() => _fixture.SendAsync(request));
                Assert.Equal(HttpStatusCode.Conflict, exception.HttpStatus);
            }
        }

        [Fact]
        public async Task Should_Get_Token()
        {
            // Arrange
            _fixture.StartScope();

            var createRequest = new Create.Request
            {
                Email = "email@domain.com",
                Password = Guid.NewGuid().ToString()
            };

            var request = new Token.Request
            {
                Email = createRequest.Email,
                Password = createRequest.Password
            };

            // Act
            await _fixture.SendAsync(createRequest);
            var response = await _fixture.SendAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response.Token);
        }

        [Fact]
        public async Task Throws_Get_Token_Exception_For_Invalid_Credentials()
        {
            // Arrange
            _fixture.StartScope();

            var createRequest = new Create.Request
            {
                Email = "email@domain.com",
                Password = Guid.NewGuid().ToString()
            };

            var request = new Token.Request
            {
                Email = _fixture.GetEmail(),
                Password = createRequest.Password
            };

            // Act
            await Assert.ThrowsAsync<RestException>(() => _fixture.SendAsync(request));
        }

        [Fact]
        public async Task Should_Create_Account_Settings()
        {
            // Arrange
            _fixture.StartScope();

            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();
            testAccount.Id = Guid.NewGuid();

            var initialCount = await _fixture.CountAsync<AccountSetting>();

            var request = new UpdateSetting.Request
            {
                TargetCalories = 11.11M
            };

            // Act
            await _fixture.SendAsync(request);
            var finalCount = await _fixture.CountAsync<AccountSetting>();

            // Assert
            Assert.Equal(0, initialCount);
            Assert.Equal(1, finalCount);
        }

        [Fact]
        public async Task Should_Update_Existing_Account_Setting()
        {
            // Arrange
            _fixture.StartScope();

            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();
            testAccount.Id = Guid.NewGuid();

            var request = new UpdateSetting.Request
            {
                TargetCalories = 11.11M
            };

            await _fixture.SendAsync(request);
            var initialCount = await _fixture.CountAsync<AccountSetting>();

            request.TargetCalories = 22.22M;
            await _fixture.SendAsync(request);

            // Act
            var finalCount = await _fixture.CountAsync<AccountSetting>();
            var accountSetting =
                await _fixture.ExecuteDbContextAsync(db =>
                    db.AccountSettings.SingleOrDefaultAsync(x => x.AccountId == testAccount.Id));

            // Assert
            Assert.Equal(1, initialCount);
            Assert.Equal(1, finalCount);
            Assert.Equal(request.TargetCalories, accountSetting.TargetCalories);
        }

        [Fact]
        public async Task Should_Get_Account_Index_Without_Filter()
        {
            _fixture.StartScope();

            {   // Arrange-TestAccountList
                var request = new Create.Request
                {
                    Email = "email@domain.com",
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);

                request.Email = "email2@domain.com";
                await _fixture.SendAsync(request);

                request.Email = "email3@domain.com";
                await _fixture.SendAsync(request);

                // Act
                var count =
                    await _fixture.CountAsync<Account>();

                // Assert
                // There is also an AdminAccount
                Assert.Equal(3 + 1, count);
            }

            {
                // Arrange
                var request = new Index.Request
                {
                    PageSize = 100
                };

                // Act
                var response = await _fixture.SendAsync(request);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(4, response.Items.Count);
            }
        }

        [Fact]
        public async Task Should_Get_Account_Index_With_Filter()
        {
            _fixture.StartScope();

            {   // Arrange-TestAccountList
                var request = new Create.Request
                {
                    Email = "email@domain.com",
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);

                request.Email = "email2@domain.com";
                await _fixture.SendAsync(request);

                request.Email = "email3@domain.com";
                await _fixture.SendAsync(request);

                // Act
                var count =
                    await _fixture.CountAsync<Account>();

                // Assert
                // There is also an AdminAccount
                Assert.Equal(3 + 1, count);
            }

            {   // Single Result Test
                // Arrange
                var request = new Index.Request
                {
                    Filter = "Email eq 'email@domain.com'"
                };

                // Act
                var response = await _fixture.SendAsync(request);

                // Assert
                Assert.NotNull(response);
                Assert.Single(response.Items);
                Assert.Equal("email@domain.com", response.Items.First().Email);
            }

            {   // Multiple Result Test
                // Arrange
                var request = new Index.Request
                {
                    Filter = "Role eq 'RegularUser'"
                };

                // Act
                var response = await _fixture.SendAsync(request);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(3, response.Items.Count);
                Assert.Equal("email@domain.com", response.Items.First().Email);
            }
        }

        [Fact]
        public async Task Should_Update_Account()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {
                // Arrange
                var request = new Update.Request
                {
                    Password = Guid.NewGuid().ToString()
                };
                var passwordHasher = _fixture.GetService<IPasswordHasher>();

                // Act
                await _fixture.SendAsync(request);
                var account =
                    await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Id == testAccount.Id));

                // Assert
                Assert.NotNull(account);
                Assert.True(passwordHasher.VerifyHashedPassword(request.Password, account.PasswordHash));
            }
        }

        [Fact]
        public async Task Should_Delete_Account_With_Valid_Email()
        {
            _fixture.StartScope();

            string email;

            {   // Arrange-Account-To-Delete
                var request = new Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                email = request.Email;
            }

            {
                // Arrange
                var request = new Delete.Request
                {
                    Email = email
                };

                // Act
                await _fixture.SendAsync(request);
                var account = await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == email));
                var count = await _fixture.ExecuteDbContextAsync(db => db.Accounts.CountAsync());

                // Assert
                Assert.Null(account);

                // There should be only Admin account
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task Throws_Delete_Exception_For_Invalid_Email()
        {
            _fixture.StartScope();

            {   // Arrange-Account-To-Delete
                var request = new Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
            }

            {
                // Arrange
                var request = new Delete.Request
                {
                    Email = _fixture.GetEmail()
                };

                // Act & Assert
                await Assert.ThrowsAsync<RestException>(() => _fixture.SendAsync(request));
            }
        }
    }
}
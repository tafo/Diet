using System;
using System.Linq;
using System.Threading.Tasks;
using Diet.Api.Domain;
using Diet.Api.Features.Meal;
using Diet.Api.Infrastructure.ExceptionHandling;
using Diet.Api.Infrastructure.Providers;
using Diet.Api.Services;
using Diet.Tests.EnvironmentServices;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Index = Diet.Api.Features.Meal.Index;

namespace Diet.Tests.Features
{
    [Collection(nameof(TestFixtureCollection))]
    public class MealTest
    {
        private readonly TestFixture _fixture;

        public MealTest(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Should_Create_Meal()
        {
            // Arrange
            _fixture.StartScope();

            var request = new Create.Request
            {
                Date = DateTime.UtcNow,
                Time = "11:11",
                Text = Guid.NewGuid().ToString(),
                Calories = 1
            };

            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();
            testAccount.Id = Guid.NewGuid();

            // Act
            var response = await _fixture.SendAsync(request);
            var meal = await _fixture.FindAsync<Meal>(response.Id);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(request.Date.ToShortDateString(), meal.Date.ToShortDateString());
            Assert.Equal(request.Time, meal.Time.ToShortTimeString());
            Assert.Equal(request.Text, meal.Text);
            Assert.Equal(request.Calories, meal.Calories);
            Assert.Equal(testAccount.Id, meal.AccountId);
        }

        [Fact]
        public async Task Should_Get_NotProvided_Calories_From_Service_For_Valid_MealName()
        {
            // Arrange
            _fixture.StartScope();

            var request = new Create.Request
            {
                Date = DateTime.UtcNow,
                Time = "11:11",
                Text = Guid.NewGuid().ToString()
            };

            const decimal calories = 55.55M;

            var service = (TestCaloriesService)_fixture.GetService<ICaloriesService>();
            service.Calories.Add(request.Text, calories);

            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();
            testAccount.Id = Guid.NewGuid();

            // Act
            var response = await _fixture.SendAsync(request);
            var meal = await _fixture.FindAsync<Meal>(response.Id);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(request.Date.ToShortDateString(), meal.Date.ToShortDateString());
            Assert.Equal(request.Time, meal.Time.ToShortTimeString());
            Assert.Equal(request.Text, meal.Text);
            Assert.Equal(calories, meal.Calories);
            Assert.Equal(testAccount.Id, meal.AccountId);
        }

        [Fact]
        public async Task Should_Set_Null_For_Invalid_MealName()
        {
            // Arrange
            _fixture.StartScope();

            var request = new Create.Request
            {
                Date = DateTime.UtcNow,
                Time = "11:11",
                Text = Guid.NewGuid().ToString()
            };

            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();
            testAccount.Id = Guid.NewGuid();

            // Act
            var response = await _fixture.SendAsync(request);
            var meal = await _fixture.FindAsync<Meal>(response.Id);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(request.Date.ToShortDateString(), meal.Date.ToShortDateString());
            Assert.Equal(request.Time, meal.Time.ToShortTimeString());
            Assert.Equal(request.Text, meal.Text);
            Assert.Null(meal.Calories);
            Assert.Equal(testAccount.Id, meal.AccountId);
        }

        [Fact]
        public async Task Should_Set_False_To_CalorieStatus_Without_Setting()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {   // Arrange
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = Guid.NewGuid().ToString(),
                    Calories = 1
                };

                // Act
                var response = await _fixture.SendAsync(request);
                var meal = await _fixture.FindAsync<Meal>(response.Id);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(request.Date.ToShortDateString(), meal.Date.ToShortDateString());
                Assert.Equal(request.Time, meal.Time.ToShortTimeString());
                Assert.Equal(request.Text, meal.Text);
                Assert.Equal(request.Calories, meal.Calories);
                Assert.False(meal.CalorieStatus);
                Assert.Equal(testAccount.Id, meal.AccountId);
            }
        }

        [Fact]
        public async Task Should_Set_False_To_CalorieStatus_If_CurrentCalories_IsGreaterThan_TargetCalories_With_Single_Meal()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {   // Arrange-AccountSetting
                var request = new Api.Features.Account.UpdateSetting.Request
                {
                    TargetCalories = 1
                };
                await _fixture.SendAsync(request);
            }
            
            {   // Arrange
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = Guid.NewGuid().ToString(),
                    Calories = 2
                };

                // Act
                var response = await _fixture.SendAsync(request);
                var meal = await _fixture.FindAsync<Meal>(response.Id);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(request.Date.ToShortDateString(), meal.Date.ToShortDateString());
                Assert.Equal(request.Time, meal.Time.ToShortTimeString());
                Assert.Equal(request.Text, meal.Text);
                Assert.Equal(request.Calories, meal.Calories);
                Assert.False(meal.CalorieStatus);
                Assert.Equal(testAccount.Id, meal.AccountId);
            }
        }

        [Fact]
        public async Task Should_Set_True_To_CalorieStatus_If_CurrentCalories_IsLessThan_TargetCalories_With_Single_Meal()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {   // Arrange-AccountSetting
                var request = new Api.Features.Account.UpdateSetting.Request
                {
                    TargetCalories = 2
                };
                await _fixture.SendAsync(request);
            }

            {   // Arrange
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = Guid.NewGuid().ToString(),
                    Calories = 1
                };

                // Act
                var response = await _fixture.SendAsync(request);
                var meal = await _fixture.FindAsync<Meal>(response.Id);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(request.Date.ToShortDateString(), meal.Date.ToShortDateString());
                Assert.Equal(request.Time, meal.Time.ToShortTimeString());
                Assert.Equal(request.Text, meal.Text);
                Assert.Equal(request.Calories, meal.Calories);
                Assert.True(meal.CalorieStatus);
                Assert.Equal(testAccount.Id, meal.AccountId);
            }
        }

        [Fact]
        public async Task Should_Set_False_To_CalorieStatus_If_CurrentCalories_IsGreaterThan_TargetCalories_With_Multiple_Meals()
        {
            _fixture.StartScope();
            
            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {   // Arrange-AccountSetting
                var request = new Api.Features.Account.UpdateSetting.Request
                {
                    TargetCalories = 3
                };
                await _fixture.SendAsync(request);
            }

            {   // Arrange
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = Guid.NewGuid().ToString(),
                    Calories = 2
                };

                // Act
                await _fixture.SendAsync(request);
                var response = await _fixture.SendAsync(request);
                var meal = await _fixture.FindAsync<Meal>(response.Id);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(request.Date.ToShortDateString(), meal.Date.ToShortDateString());
                Assert.Equal(request.Time, meal.Time.ToShortTimeString());
                Assert.Equal(request.Text, meal.Text);
                Assert.Equal(request.Calories, meal.Calories);
                Assert.False(meal.CalorieStatus);
                Assert.Equal(testAccount.Id, meal.AccountId);
            }
        }

        [Fact]
        public async Task Should_Set_True_To_CalorieStatus_If_CurrentCalories_IsLessThan_TargetCalories_With_Multiple_Meals()
        {
            _fixture.StartScope();
            
            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {   // Arrange-AccountSetting
                var request = new Api.Features.Account.UpdateSetting.Request
                {
                    TargetCalories = 3
                };
                await _fixture.SendAsync(request);
            }

            {   // Arrange
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = Guid.NewGuid().ToString(),
                    Calories = 1
                };

                // Act
                await _fixture.SendAsync(request);
                var response = await _fixture.SendAsync(request);
                var meal = await _fixture.FindAsync<Meal>(response.Id);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(request.Date.ToShortDateString(), meal.Date.ToShortDateString());
                Assert.Equal(request.Time, meal.Time.ToShortTimeString());
                Assert.Equal(request.Text, meal.Text);
                Assert.Equal(request.Calories, meal.Calories);
                Assert.True(meal.CalorieStatus);
                Assert.Equal(testAccount.Id, meal.AccountId);
            }
        }

        [Fact]
        public async Task Should_Get_Meal_Index_Without_Filter()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {   // Arrange-AccountSetting
                var request = new Api.Features.Account.UpdateSetting.Request
                {
                    TargetCalories = 10
                };
                await _fixture.SendAsync(request);
            }

            {   // Arrange-MealList
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = "string",
                    Calories = 3
                };
                await _fixture.SendAsync(request);

                request.Date = request.Date.AddDays(1);
                request.Text = "string";
                await _fixture.SendAsync(request);

                request.Date = request.Date.AddDays(1);
                request.Text = "string1";
                await _fixture.SendAsync(request);

                request.Date = request.Date.AddDays(1);
                request.Text = "string1";
                await _fixture.SendAsync(request);

                // Act
                var count =
                    await _fixture.CountAsync<Meal>();

                // Assert
                Assert.Equal(4, count);
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
        public async Task Should_Get_Meal_Index_With_Filter()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {   // Arrange-AccountSetting
                var request = new Api.Features.Account.UpdateSetting.Request
                {
                    TargetCalories = 10
                };
                await _fixture.SendAsync(request);
            }

            {   // Arrange-MealList
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = "string",
                    Calories = 3
                };
                await _fixture.SendAsync(request);

                request.Text = "string";
                await _fixture.SendAsync(request);

                request.Text = "string1";
                await _fixture.SendAsync(request);

                request.Text = "OverCalories"; // Now => 3+3+3+3 > 10
                await _fixture.SendAsync(request);

                request.Date = request.Date.AddDays(1); // This will be used to create complex query
                request.Text = "New Day";
                await _fixture.SendAsync(request);

                // Act
                var count =
                    await _fixture.CountAsync<Meal>();

                // Assert
                Assert.Equal(5, count);
            }

            {   // Single Result Test
                // Arrange
                var request = new Index.Request
                {
                    Filter = "CalorieStatus = 'false'"
                };

                // Act
                var response = await _fixture.SendAsync(request);

                // Assert
                Assert.NotNull(response);
                Assert.Single(response.Items);
                Assert.Equal("11:11", response.Items.First().Time);
                Assert.Equal("OverCalories", response.Items.First().Text);
            }

            {   // Multiple Result Test With Single Filter
                // Arrange
                var request = new Index.Request
                {
                    Filter = "CalorieStatus = 'true'"
                };

                // Act
                var response = await _fixture.SendAsync(request);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(4, response.Items.Count);
            }

            {   // Multiple Result Test With Multiple Filter
                // Arrange
                var request = new Index.Request
                {
                    Filter = "(CalorieStatus = 'false') or (Text eq 'string')"
                };

                // Act
                var response = await _fixture.SendAsync(request);

                // Assert
                Assert.NotNull(response);
                // 2 item by Text = 'string' + 1 item by CalorieStatus = 'false'
                Assert.Equal(3, response.Items.Count);
                Assert.Contains(response.Items, x => x.Text.Equals("string"));
                Assert.Contains(response.Items, x => x.Text.Equals("OverCalories"));
            }

            {   // Single Result Test With Complex Multiple Filter
                // Arrange
                var request = new Index.Request
                {
                    Filter = $"(CalorieStatus = 'true' or Text = 'New Day') AnD Date = '{DateTime.UtcNow.Date}'"
                };

                // Act
                var response = await _fixture.SendAsync(request);

                // Assert
                Assert.NotNull(response);
                Assert.Equal(3, response.Items.Count);

                // Because of date expression 
                Assert.DoesNotContain(response.Items, x => x.Text.Equals("New Day"));

                // Because of CalorieStatus expression
                // Even if first part of the expression matches this item, second part eliminated it 
                Assert.DoesNotContain(response.Items, x => x.Text.Equals("OverCalories"));
                Assert.DoesNotContain(response.Items, x => x.CalorieStatus.Equals(false));
            }
        }

        [Fact]
        public async Task Should_Update_Meal()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(), 
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            Guid id;
            {
                // Arrange-Meal-To-Update
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = "string",
                    Calories = 3
                };
                var response = await _fixture.SendAsync(request);
                id = response.Id;
            }

            {
                // Arrange
                var request = new Update.Request
                {
                    Id =  id,
                    Date = DateTime.UtcNow.Date.AddDays(1),
                    Time = "11:15",
                    Text = "Another String",
                    Calories = 22
                };

                // Act
                await _fixture.SendAsync(request);
                var meal =
                    await _fixture.ExecuteDbContextAsync(db => db.Meals.SingleOrDefaultAsync(x => x.Id == id));

                // Assert
                Assert.Equal(request.Date.ToShortDateString(), meal.Date.ToShortDateString());
                Assert.Equal(request.Time, meal.Time.ToShortTimeString());
                Assert.Equal(request.Text, meal.Text);
                Assert.Equal(request.Calories, meal.Calories);
                Assert.False(meal.CalorieStatus);
                Assert.Equal(testAccount.Id, meal.AccountId);
            }
        }

        [Fact]
        public async Task Throws_Update_Exception_For_Invalid_Identifier()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(), 
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {
                // Arrange-Meal-To-Update
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = "string",
                    Calories = 3
                };
                await _fixture.SendAsync(request);
            }

            {
                // Arrange
                var request = new Update.Request
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.UtcNow.Date.AddDays(1),
                    Time = "11:15",
                    Text = "Another String",
                    Calories = 22
                };

                // Act & Assert
                await Assert.ThrowsAsync<RestException>(() => _fixture.SendAsync(request));
            }
        }

        [Fact]
        public async Task Throws_Update_Exception_If_Sender_Is_Not_Meal_Owner()
        {
            _fixture.StartScope();

            // Arrange-TestAccount(RegularUser)
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();
            testAccount.Role = Role.RegularUser;

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            // This is created by another Account
            var createRequest = new Create.Request
            {
                Date = DateTime.UtcNow.Date,
                Time = "11:11",
                Text = "string",
                Calories = 3
            };
            var createResponse = await _fixture.SendAsync(createRequest);

            {   // Arrange-Account(Change the Account)
                var request = new Api.Features.Account.Create.Request
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
                    Id = createResponse.Id,
                    Date = DateTime.UtcNow.Date.AddDays(1),
                    Time = "11:15",
                    Text = "Another String",
                    Calories = 22
                };

                // Act & Assert
                await Assert.ThrowsAsync<RestException>(() => _fixture.SendAsync(request));
            }
        }

        [Fact]
        public async Task Should_Delete_Meal()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            Guid id;
            {
                // Arrange-Meal-To-Delete
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = "string",
                    Calories = 3
                };
                var response = await _fixture.SendAsync(request);
                id = response.Id;
            }

            {
                // Arrange
                var request = new Delete.Request
                {
                    Id = id
                };

                // Act
                await _fixture.SendAsync(request);
                var count = await _fixture.CountAsync<Meal>();

                // Assert
                Assert.Equal(0, count);
            }
        }

        [Fact]
        public async Task Throws_Delete_Exception_For_Invalid_Identifier()
        {
            _fixture.StartScope();

            // Arrange-TestAccount
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            {
                // Arrange-Meal-To-Delete
                var request = new Create.Request
                {
                    Date = DateTime.UtcNow.Date,
                    Time = "11:11",
                    Text = "string",
                    Calories = 3
                };
                await _fixture.SendAsync(request);
            }

            {
                // Arrange
                var request = new Delete.Request
                {
                    Id = Guid.NewGuid()
                };

                // Act & Assert
                await Assert.ThrowsAsync<RestException>(() => _fixture.SendAsync(request));
            }
        }

        [Fact]
        public async Task Throws_Delete_Exception_If_Sender_Is_Not_Meal_Owner()
        {
            _fixture.StartScope();

            // Arrange-TestAccount(RegularUser)
            var testAccount = (TestAccountProvider)_fixture.GetService<ICurrentAccountProvider>();
            testAccount.Role = Role.RegularUser;

            {   // Arrange-Account
                var request = new Api.Features.Account.Create.Request
                {
                    Email = _fixture.GetEmail(),
                    Password = Guid.NewGuid().ToString()
                };
                await _fixture.SendAsync(request);
                testAccount.Id =
                    (await _fixture.ExecuteDbContextAsync(db => db.Accounts.SingleOrDefaultAsync(x => x.Email == request.Email))).Id;
            }

            // This is created by another Account
            var createRequest = new Create.Request
            {
                Date = DateTime.UtcNow.Date,
                Time = "11:11",
                Text = "string",
                Calories = 3
            };
            var createResponse = await _fixture.SendAsync(createRequest);

            {   // Arrange-Account(Change the Account)
                var request = new Api.Features.Account.Create.Request
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
                var request = new Delete.Request
                {
                    Id = createResponse.Id
                };

                // Act & Assert
                await Assert.ThrowsAsync<RestException>(() => _fixture.SendAsync(request));
            }
        }
    }
}
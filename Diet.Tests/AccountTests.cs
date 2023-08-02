using System.Net.Http;
using Diet.Api.Features.Account;
using FluentAssertions;
using Xunit;

namespace Diet.Tests
{
    public class AccountTests : IClassFixture<Fixture>
    {
        private Fixture Fixture { get; }
        public AccountTests(Fixture fixture)
        {
            Fixture = fixture;
        }
        
        [Fact]
        public async void Check_Account_Create()
        {
            var createRequest = new Create.Request();
            string request = System.Text.Json.JsonSerializer.Serialize(createRequest);
            StringContent stringContent = new StringContent(request);
            HttpResponseMessage response = await Fixture.Client.PostAsync("account", stringContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<Create.Response>(stringResponse);
            result.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Check_Account_Delete()
        {
            var request = new Delete.Request();
            var stringContent = Fixture.GetStringContent(request);
            var response = await Fixture.Client.PostAsync("account/delete", stringContent);
            var result = await Fixture.GetResponseAsync<Delete.Response>(response);
            result.Should().NotBeNull();
        }
    }
}
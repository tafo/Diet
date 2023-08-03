using System.Collections.Generic;
using System.Net;
using Diet.Api.Features.Account;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Sdk;

namespace Diet.IntegrationTests;

public class AccountTests: IClassFixture<TestFixture>, IClassFixture<AbcFixture>
{
    private TestFixture TestFixture { get; }
    private AbcFixture AbcFixture { get; }
    public AccountTests(TestFixture testFixture, AbcFixture abcFixture)
    {
        TestFixture = testFixture;
        AbcFixture = abcFixture;
    }
    
    /// <summary>
    /// Happy Path
    /// </summary>
    [Fact]
    public async void Check_Account_Create()
    {
        // Every test has 3 parts, 1) Arrange, 2) Act, 3) Assert
        
        // Arrange
        var applicationFactory = new WebApplicationFactory<Program>();
        var client = applicationFactory.CreateClient();
        var request = new Create.Request
        {
            Email = "user@string.com",
            Password = "123"
        };

        var stringContent = TestFixture.GetStringContent(request);

        // Act, Action
        var response = await client.PostAsync("account/create", stringContent);
        
        // (arrange)
        var result = await TestFixture.GetResultAsync<Create.Response>(response);
        
        // Assert
        result.Should().NotBeNull();
        result?.Token.Should().NotBeNull();
        result?.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async void Check_Account_Create_Without_Email()
    {
        // Every test has 3 parts, 1) Arrange, 2) Act, 3) Assert
        
        // Arrange
        var applicationFactory = new WebApplicationFactory<Program>();
        var client = applicationFactory.CreateClient();
        var request = new Create.Request
        {
            Password = "123"
        };

        var stringContent = TestFixture.GetStringContent(request);

        // Act, Action
        var response = await client.PostAsync("account/create", stringContent);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        // var result = await TestFixture.GetResultAsync<ObjectResult>(response);
    }
}
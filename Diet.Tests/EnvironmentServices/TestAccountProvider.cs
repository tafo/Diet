using System;
using Diet.Api.Infrastructure.Providers;

namespace Diet.Tests.EnvironmentServices
{
    public class TestAccountProvider : ICurrentAccountProvider
    {
        public Guid Id { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
    }
}
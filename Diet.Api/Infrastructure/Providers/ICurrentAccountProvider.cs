using System;

namespace Diet.Api.Infrastructure.Providers
{
    public interface ICurrentAccountProvider
    {
        public Guid Id { get; }
        public string Role { get; set; }
    }
}
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diet.Api.Infrastructure.Providers
{
    public class CurrentAccountProvider : ICurrentAccountProvider
    {
        public CurrentAccountProvider([FromServices] IHttpContextAccessor httpContextAccessor)
        {
            Id = Guid.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
        }

        public Guid Id { get; set; }
        public string Role { get; set; }
    }
}
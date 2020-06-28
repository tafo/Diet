using System;
using Diet.Api;
using Diet.Api.Data;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.Providers;
using Diet.Api.Services;
using Diet.Tests.EnvironmentServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diet.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddEnvironmentServices(IServiceCollection services)
        {
            services.AddDbContext<DietContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddSingleton(typeof(IClock), typeof(TestClock));
            services.AddScoped<ICurrentAccountProvider, TestAccountProvider>();
        }

        public override void AddIntegrationServices(IServiceCollection services)
        {
            services.AddSingleton<ICaloriesService, TestCaloriesService>();
        }
    }
}
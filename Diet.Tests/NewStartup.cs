using System;
using System.IO;
using System.Text;
using Diet.Api;
using Diet.Api.Data;
using Diet.Api.Features.Account;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.Providers;
using Diet.Api.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Diet.Tests;

public class NewStartup
{
    public IConfiguration Configuration { get; set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<ExceptionMiddleware>();
        services.AddControllers();
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<AccountController>());
        services.AddDbContext<DietContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        // services.AddSingleton(typeof(IClock), typeof(Clock));
        // services.AddScoped(typeof(ICurrentAccountProvider), typeof(CurrentAccountProvider));

        // AddJwt(services);
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        // services.AddSingleton<ITokenProvider, TokenProvider>();
        // services.AddMemoryCache();
        // services.AddMiniProfiler().AddEntityFramework();
        // services.AddMediatR(typeof(Startup));
        //
        // AddEnvironmentServices(services);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        
        // app.UseMiniProfiler();
        //
        // app.UseExceptionHandler("/error");
        //
        // app.UseRouting();
        //
        // app.UseAuthentication();
        //
        // // ToDo : Get client application origin from product team
        // app.UseCors(builder =>
        //     builder
        //         .AllowAnyOrigin()
        //         .AllowAnyHeader()
        //         .AllowAnyMethod());
        //
        // app.UseAuthorization();
        //
        // app.UseEndpoints(endpoints =>
        // {
        //     endpoints.MapControllers();
        // });
    }

    /// <summary>
    /// JSON Web Token Authentication based on https://tools.ietf.org/html/rfc7519
    /// </summary>
    /// <param name="services"></param>
    private void AddJwt(IServiceCollection services)
    {
        // var issuer = Configuration.GetValue<string>("Security:Issuer");
        // var audience = Configuration.GetValue<string>("Security:Audience");
        // var key = Configuration.GetValue<string>("Security:Key");
        //
        // var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
        // var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //
        // services.Configure<JwtOptions>(options =>
        // {
        //     options.Issuer = issuer;
        //     options.Audience = audience;
        //     options.SigningCredentials = signingCredentials;
        // });
        //
        // var tokenValidationParameters = new TokenValidationParameters
        // {
        //     ValidateIssuer = true,
        //     ValidateAudience = true,
        //     ValidateLifetime = true,
        //     ValidateIssuerSigningKey = true,
        //     ValidIssuer = issuer,
        //     ValidAudience = audience,
        //     IssuerSigningKey = signingCredentials.Key
        // };
        //
        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(options =>
        //     {
        //         options.TokenValidationParameters = tokenValidationParameters;
        //     });
    }
}
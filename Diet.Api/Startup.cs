using System.Text;
using Diet.Api.Data;
using Diet.Api.Features.Filter;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.Providers;
using Diet.Api.Infrastructure.Security;
using Diet.Api.Services;
using Diet.Api.Services.Nutritionix;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Diet.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ExceptionMiddleware>();
            AddJwt(services);
            AddIntegrationServices(services);
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenProvider, TokenProvider>();
            services.AddMemoryCache();
            services.AddMiniProfiler().AddEntityFramework();
            services.AddMediatR(typeof(Startup));

            AddEnvironmentServices(services);

            services.AddControllers(options => { options.Filters.Add(typeof(ValidationActionFilter)); })
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                        },
                        new string[] { }
                    }
                });
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Diet API", Version = "v1" });
            });

            // Customize the Swagger generator here
            services.ConfigureSwaggerGen(options =>
            {
                // Use fully qualified schema object names to ensure uniqueness
                // It is necessary to use identical request models
                options.CustomSchemaIds(configuration => configuration.FullName);
            });
        }

        public virtual void AddIntegrationServices(IServiceCollection services)
        {
            services.Configure<ServiceConfiguration>(options =>
            {
                options.Endpoint = Configuration.GetValue<string>("Services:Nutritionix:Endpoint");
                options.AppId = Configuration.GetValue<string>("Services:Nutritionix:AppId");
                options.AppKey = Configuration.GetValue<string>("Services:Nutritionix:AppKey");
            });

            services.AddSingleton<ICaloriesService, CaloriesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            
            app.UseMiniProfiler();

            //app.UseExceptionHandler("/error");

            app.UseRouting();

            app.UseAuthentication();

            // ToDo : Get client application origin from product team
            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            // Enable middleware to serve swagger-ui assets(HTML, JS, CSS etc.)
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "Diet API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public virtual void AddEnvironmentServices(IServiceCollection services)
        {
            services.AddDbContext<DietContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DietConnection")));
            services.AddSingleton(typeof(IClock), typeof(Clock));
            services.AddScoped(typeof(ICurrentAccountProvider), typeof(CurrentAccountProvider));
            services.AddScoped(typeof(IExpressionProvider<>), typeof(ExpressionProvider<>));
        }

        /// <summary>
        /// JSON Web Token Authentication based on https://tools.ietf.org/html/rfc7519
        /// </summary>
        /// <param name="services"></param>
        private void AddJwt(IServiceCollection services)
        {
            var issuer = Configuration.GetValue<string>("Security:Issuer");
            var audience = Configuration.GetValue<string>("Security:Audience");
            var key = Configuration.GetValue<string>("Security:Key");

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audience;
                options.SigningCredentials = signingCredentials;
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = signingCredentials.Key
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });
        }
    }
}

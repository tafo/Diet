using System;
using System.Text.Json.Serialization;
using Diet.Api;
using Diet.Api.Data;
using Diet.Api.Features.Account;
using Diet.Api.Infrastructure;
using Diet.Api.Infrastructure.Providers;
using Diet.Api.Infrastructure.Security;
using Diet.Api.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICaloriesService, CaloriesService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(typeof(ICurrentAccountProvider), typeof(CurrentAccountProvider));
builder.Services.AddSingleton(typeof(IClock), typeof(Clock));
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<ITokenProvider, TokenProvider>();
builder.Services.AddDbContext<DietContext>(options =>
    options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<AccountController>());
builder.Services.AddControllers(options => { options.Filters.Add(typeof(ValidationActionFilter)); })
    .AddJsonOptions(option =>
    {
        option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<AccountController>());

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.Run();
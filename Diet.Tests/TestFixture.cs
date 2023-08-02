using System;
using System.IO;
using System.Threading.Tasks;
using Diet.Api.Data;
using Diet.Api.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Diet.Tests
{
    [CollectionDefinition(nameof(TestFixtureCollection))]
    public class TestFixtureCollection : ICollectionFixture<TestFixture>
    {

    }

    /// <summary>
    /// Every test method must call StartScope to get same DietContext during the execution
    /// </summary>
    public class TestFixture
    {
        public readonly IServiceScopeFactory ServiceScopeFactory;

        public TestFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            var startup = new TestStartup(configuration);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            ServiceScopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
        }

        private IServiceProvider ServiceProvider { get; set; }

        public void StartScope()
        {
            ServiceProvider = ServiceScopeFactory.CreateScope().ServiceProvider;
            GetService<DietContext>().Database.EnsureCreated();
        }

        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        public async Task<T> ExecuteDbContextAsync<T>(Func<DietContext, Task<T>> action)
        {
            return await action(ServiceProvider.GetService<DietContext>());
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ServiceProvider.GetService<IMediator>().Send(request);
        }

        public Task SendAsync<TResponse>(IRequest request)
        {
            return ServiceProvider.GetService<IMediator>().Send(request);
        }

        public Task InsertAsync<TEntity>(TEntity entity) where TEntity : Entity
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);

                return db.SaveChangesAsync();
            });
        }

        public Task<TEntity> FindAsync<TEntity>(Guid id) where TEntity : Entity
        {
            return ExecuteDbContextAsync(db => db.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id));
        }

        public Task<int> CountAsync<TEntity>() where TEntity : Entity
        {
            return ExecuteDbContextAsync(db => db.Set<TEntity>().CountAsync());
        }

        #region Helper Methods

        public string GetEmail()
        {
            return $"{Guid.NewGuid()}@domain.com";
        }

        #endregion
    }
}
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Branch.Repository;
using StackExchange.Redis.Branch.UnitTest.Fakes;
using System.Reflection;
using Xunit;

namespace StackExchange.Redis.Branch.UnitTest
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddRedisBranch_AddBranchesToServiceCollection_ReturnRedisBranch()
        {
            //Arrange 
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return FakesFactory.CreateConnectionMultiplexerFake();
            });

            //Act
            services.AddRedisBranch(Assembly.GetExecutingAssembly());

            //Assert
            //Connection Multiplexer and StockRepository
            Assert.Equal(2, services.Count);
        }

        [Fact]
        public void AddRedisBranch_AddBranchesToServiceCollectionAndBuildProvider_GetRedisBranch()
        {
            //Arrange 
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return FakesFactory.CreateConnectionMultiplexerFake();
            });

            //Act
            services.AddRedisBranch(Assembly.GetExecutingAssembly());
            ServiceProvider provider = services.BuildServiceProvider();
            IRedisRepository<StockEntity> notNullStockEntity = provider.GetRequiredService<IRedisRepository<StockEntity>>();

            //Assert
            Assert.NotNull(notNullStockEntity);
        }
    }
}

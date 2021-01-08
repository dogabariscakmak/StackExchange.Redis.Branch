using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Branch.IntegrationTest.Fakes;
using StackExchange.Redis.Branch.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StackExchange.Redis.Branch.IntegrationTest.Helpers
{
    public class RedisFixture : IDisposable
    {
        public ServiceProvider DI { get; private set; }
        public TestSettings TestSettings { get; private set; }
        public List<StockEntity> TestData { get; private set; }

        private readonly DockerStarter dockerStarter;
        private bool _disposed;

        public RedisFixture()
        {
            TestData = new List<StockEntity>();

            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            bool IsGithubAction = false;
            Boolean.TryParse(Environment.GetEnvironmentVariable("IS_GITHUB_ACTION"), out IsGithubAction);

            TestSettings = new TestSettings()
            {
                RedisConnectionConfiguration = config.GetSection(TestSettings.Position)["RedisConnectionConfiguration"],
                DockerComposeFile = config.GetSection(TestSettings.Position)["DockerComposeFile"],
                DockerWorkingDir = config.GetSection(TestSettings.Position)["DockerWorkingDir"],
                DockerComposeExePath = config.GetSection(TestSettings.Position)["DockerComposeExePath"],
                TestDataFilePath = config.GetSection(TestSettings.Position)["TestDataFilePath"],
                IsGithubAction = IsGithubAction
            };

            if (!TestSettings.IsGithubAction)
            {
                dockerStarter = new DockerStarter(TestSettings.DockerComposeExePath, TestSettings.DockerComposeFile, TestSettings.DockerWorkingDir);
                dockerStarter.Start();
            }

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(TestSettings.RedisConnectionConfiguration);
            });
            services.AddRedisBranch(Assembly.GetExecutingAssembly());

            DI = services.BuildServiceProvider();
        }

        public async Task ReloadTestDataAsync()
        {
            TestData.Clear();
            using (StreamReader file = File.OpenText(TestSettings.TestDataFilePath))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                TestData.AddRange((List<StockEntity>)serializer.Deserialize(file, typeof(List<StockEntity>)));
            }

            var connectionMultiplexer = (ConnectionMultiplexer)DI.GetService<IConnectionMultiplexer>();
            var server = connectionMultiplexer.GetServer("localhost:6379");
            await server.FlushDatabaseAsync();

            StockRepository stockRepository = (StockRepository)DI.GetService<IRedisRepository<StockEntity>>();
            foreach (StockEntity entity in TestData)
            {
                entity.FillCalculatedProperties();
                await stockRepository.AddAsync(entity);
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DI.Dispose();
                    if (!TestSettings.IsGithubAction)
                    {
                        dockerStarter.Dispose();
                    }
                }

                _disposed = true;
            }
        }
    }

    [CollectionDefinition("Redis")]
    public class RedisCollection : ICollectionFixture<RedisFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}

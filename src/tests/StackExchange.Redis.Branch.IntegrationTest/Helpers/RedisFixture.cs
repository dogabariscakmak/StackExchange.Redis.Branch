using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Branch.IntegrationTest.Fakes;
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
            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            bool IsGithubAction = false;
            Boolean.TryParse(Environment.GetEnvironmentVariable("IS_GITHUB_ACTION"), out IsGithubAction);

            TestSettings = new TestSettings()
            {
                IsDockerComposeRequired = Convert.ToBoolean(config.GetSection(TestSettings.Position)["IsDockerComposeRequired"]),
                RedisConnectionConfiguration = config.GetSection(TestSettings.Position)["RedisConnectionConfiguration"],
                RedisDockerComposeFile = config.GetSection(TestSettings.Position)["RedisDockerComposeFile"],
                RedisDockerWorkingDir = config.GetSection(TestSettings.Position)["RedisDockerWorkingDir"],
                DockerComposeExePath = config.GetSection(TestSettings.Position)["DockerComposeExePath"],
                TestDataFilePath = config.GetSection(TestSettings.Position)["TestDataFilePath"],
                IsGithubAction = IsGithubAction
            };

            using (StreamReader file = File.OpenText(TestSettings.TestDataFilePath))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                TestData = (List<StockEntity>)serializer.Deserialize(file, typeof(List<StockEntity>));
            }

            if (TestSettings.IsDockerComposeRequired)
            {
                dockerStarter = new DockerStarter(TestSettings.DockerComposeExePath, TestSettings.RedisDockerComposeFile, TestSettings.RedisDockerWorkingDir);
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
                    if (TestSettings.IsDockerComposeRequired)
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

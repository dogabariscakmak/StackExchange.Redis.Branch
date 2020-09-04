using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Branch.IntegrationTest.Fakes;
using StackExchange.Redis.Branch.IntegrationTest.Helpers;
using StackExchange.Redis.Branch.Repository;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StackExchange.Redis.Branch.IntegrationTest
{
    [Collection("Redis")]
    [TestCaseOrderer("StackExchange.Redis.Branch.IntegrationTest.PriorityOrderer", "StackExchange.Redis.Branch.IntegrationTest")]
    public class RedisRepositoryTest
    {
        private readonly RedisFixture fixture;
        private StockRepository stockRepository;

        public RedisRepositoryTest(RedisFixture fixture)
        {
            this.fixture = fixture;
            this.stockRepository = (StockRepository)fixture.DI.GetService<IRedisRepository<StockEntity>>();
        }

        [Fact, TestPriority(0)]
        public async Task AddTestDataToRedis()
        {
            //Arrange and Act
            foreach (StockEntity entity in fixture.TestData)
            {
                await stockRepository.AddAsync(entity);
            }

            //Assert
            Assert.Equal(9, stockRepository.GetBranches().Count());
            Assert.Equal(80, await stockRepository.CountByBranchAsync(StockRepository.BRANCH_GROUPALL, "All"));
        }
    }
}

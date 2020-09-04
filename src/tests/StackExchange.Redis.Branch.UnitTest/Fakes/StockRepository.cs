using StackExchange.Redis.Branch.Repository;

namespace StackExchange.Redis.Branch.UnitTest.Fakes
{
    public class StockRepository : RedisRepositoryBase<StockEntity>
    {
        public StockRepository(IConnectionMultiplexer redisConnectionMultiplexer) : base(redisConnectionMultiplexer)
        {
        }

        public override void CreateBranches()
        {
            return;
        }

        public string GetProfitLevel(StockEntity stock)
        {
            if (stock.PriceChangeRate > 10) return "GreatProfit";
            else if (stock.PriceChangeRate > 0) return "NormalProfit";
            else return "Loss";
        }
    }
}

using StackExchange.Redis.Branch.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Redis.Branch.IntegrationTest.Fakes
{
    public class StockRepository : RedisRepositoryBase<StockEntity>
    {
        public const string BRANCH_GROUPALL = "BRANCH_GROUPALL";
        public const string BRANCH_GROUPBY_SECTOR = "BRANCH_GROUPBY_SECTOR";
        public const string BRANCH_SORTBY_CREATEDDATETIME = "BRANCH_SORTBY_CREATEDDATETIME";
        public const string BRANCH_GROUPBY_SECTOR_SORTBY_PRICE = "BRANCH_GROUPBY_SECTOR_SORTBY_PRICE";
        public const string BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE = "BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE";
        public const string BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = "BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE";
        public const string BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = "BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE";
        public const string BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE = "BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE";

        public StockRepository(IConnectionMultiplexer redisConnectionMultiplexer) : base(redisConnectionMultiplexer)
        {
        }

        public override void CreateBranches()
        {
            //Basic filter for Active stocks
            Expression<Func<StockEntity, bool>> activeFilter = i => i.IsActive;

            //GroupAll
            RedisBranch<StockEntity> groupAll = new RedisBranch<StockEntity>();
            groupAll.SetBranchId(BRANCH_GROUPALL);
            groupAll.FilterBy(activeFilter).GroupBy("", x => "All");
            AddBranch(groupAll);

            //GroupBySector
            RedisBranch<StockEntity> groupBySectorBranch = new RedisBranch<StockEntity>();
            groupBySectorBranch.SetBranchId(BRANCH_GROUPBY_SECTOR);
            groupBySectorBranch.FilterBy(activeFilter).GroupBy("Sector");
            AddBranch(groupBySectorBranch);

            //SortByCreatedDateTime
            RedisBranch<StockEntity> sortByCreatedDateTimeBranch = new RedisBranch<StockEntity>();
            sortByCreatedDateTimeBranch.SetBranchId(BRANCH_SORTBY_CREATEDDATETIME);
            sortByCreatedDateTimeBranch.FilterBy(activeFilter).SortBy("CreatedDateTime", x => x.CreatedDateTime.Ticks);
            AddBranch(sortByCreatedDateTimeBranch);

            //GroupBySector SortByPrice
            RedisBranch<StockEntity> groupBySectorSortByPriceBranch = new RedisBranch<StockEntity>();
            groupBySectorSortByPriceBranch.SetBranchId(BRANCH_GROUPBY_SECTOR_SORTBY_PRICE);
            groupBySectorSortByPriceBranch.FilterBy(activeFilter).GroupBy("Sector").SortBy("Price");
            AddBranch(groupBySectorSortByPriceBranch);

            //GroupBySector SortByPriceChangeRate
            RedisBranch<StockEntity> groupBySectorSortByPriceChangeRateBranch = new RedisBranch<StockEntity>();
            groupBySectorSortByPriceChangeRateBranch.SetBranchId(BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE);
            groupBySectorSortByPriceChangeRateBranch.FilterBy(activeFilter).GroupBy("Sector").SortBy("PriceChangeRate");
            AddBranch(groupBySectorSortByPriceChangeRateBranch);

            //GroupByProfitLevel SortByPriceChangeRate
            RedisBranch<StockEntity> groupByProfitLevelSortByPriceChangeRateBranch = new RedisBranch<StockEntity>();
            groupByProfitLevelSortByPriceChangeRateBranch.SetBranchId(BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE);
            groupByProfitLevelSortByPriceChangeRateBranch.FilterBy(activeFilter).GroupBy("ProfitLevel", x => GetProfitLevel(x).ToString()).SortBy("PriceChangeRate");
            AddBranch(groupByProfitLevelSortByPriceChangeRateBranch);

            //GroupBySector GroupByProfitLevel SortByPriceChangeRate
            RedisBranch<StockEntity> groupBySectorGroupByProfitLevelSortByPriceChangeRateBranch = new RedisBranch<StockEntity>();
            groupBySectorGroupByProfitLevelSortByPriceChangeRateBranch.SetBranchId(BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE);
            groupBySectorGroupByProfitLevelSortByPriceChangeRateBranch.FilterBy(activeFilter).GroupBy("Sector").GroupBy("ProfitLevel", x => GetProfitLevel(x).ToString()).SortBy("PriceChangeRate");
            AddBranch(groupBySectorGroupByProfitLevelSortByPriceChangeRateBranch);

            //GroupByCountry SortByPriceChangeRate
            RedisBranch<StockEntity> groupByCountrySortByPriceChangeRateBranch = new RedisBranch<StockEntity>();
            groupByCountrySortByPriceChangeRateBranch.SetBranchId(BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE);
            groupByCountrySortByPriceChangeRateBranch.FilterBy(activeFilter).GroupBy("Country", x => x.MetaData.Country).SortBy("PriceChangeRate");
            AddBranch(groupByCountrySortByPriceChangeRateBranch);
        }

        public ProfitLevel GetProfitLevel(StockEntity stock)
        {
            if (stock.PriceChangeRate > 10) return ProfitLevel.Great;
            else if (stock.PriceChangeRate > 0) return ProfitLevel.Normal;
            else return ProfitLevel.Loss;
        }
    }
}

# StackExchange.Redis.Branch

[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<br />
<p align="center">
  <a href="https://github.com/dogabariscakmak/StackExchange.Redis.Branch">
    <img src="logo.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">StackExchange.Redis.Branch</h3>

  <p align="center">
    Lightweight library to query Redis by predefined queries on top of StackExchange.Redis.
    <br />
    <a href="https://github.com/dogabariscakmak/StackExchange.Redis.Branch/issues">Report Bug</a>
    ·
    <a href="https://github.com/dogabariscakmak/StackExchange.Redis.Branch/issues">Request Feature</a>
  </p>
</p>

<!-- TABLE OF CONTENTS -->
## Table of Contents

* [About the Project](#about-the-project)
* [Getting Started](#getting-started)
    * [Usage](#usage)
* [Roadmap](#roadmap)
* [License](#license)
* [Contact](#contact)


<!-- ABOUT THE PROJECT -->
## About The Project 

![StackExchange.Redis.Branch][product-screenshot]

This library enables that you can use redis database with predefined queries. Queries are seen like pipelines which are executed when data is writing/removing on redis.  

<!-- GETTING STARTED -->
## Getting Started

To get start with StackExchange.Redis.Branch you can clone repository or add as a reference to your project from nuget.

#### Package Manager
```Install-Package StackExchange.Redis.Branch -Version 1.0.0```

#### .NET CLI
```dotnet add package StackExchange.Redis.Branch --version 1.0.0```

#### ```PackageReference```
```<PackageReference Include="StackExchange.Redis.Branch" Version="1.0.0" />```


<!-- USAGE EXAMPLES -->
## Usage

StackExchange.Redis.Branch manages all data operations via by Repository classes. This repository classes are bascically data stores for entities which are inhereted by ```RedisEntity```. You need to implement a repository inhereted by ```RedisRepositoryBase``` for each entity you want store in redis. 

- First we implement a basic entity. Redis entity is just a marker class and has only ```Id``` property. ```Id``` property should be set by developer. If you mark a property with ```IgnoreDataMember``` attribute, this property is not written to redis.

```csharp
    public class StockEntity : RedisEntity
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public StockSector Sector { get; set; }
        public double Price { get; set; }
        public double PriceChangeRate { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsActive { get; set; }
        public StockMetaData MetaData { get; set; }
        [IgnoreDataMember]
        public string DummyString { get; set; }

        public StockEntity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public class StockMetaData
    {
        public string Country { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public CurrencyCode Currency { get; set; }

        public StockMetaData() { }
    }

    public enum StockSector
    {
        None = 0,
        Technology = 1,
        Agriculture = 2,
        Energy = 3,
        Insurance = 4
    };

    public enum CurrencyCode
    {
        None = 0,
        USD = 1,
        EURO = 2
    };
```

- Then we implement the repository for ```StockEntity```. When you implement a repository for a entity you need to override CreateBranches method to define pipelines for queries. 

```csharp
    public class StockRepository : RedisRepositoryBase<StockEntity>
    {
        ...

        public StockRepository(IConnectionMultiplexer redisConnectionMultiplexer) : base(redisConnectionMultiplexer)
        {

        }

        public override void CreateBranches()
        {
            ...
        }

        ...
    }
```

- When you override ```CreateBranches``` method, you need to define branches in other words predefined queries. Each branch has a set of rules which are determine entity meets the conditions for predefined query. A branch can contain several filter functions and group statements. A branch can contain only one or none sort statement. Branch class has Fluent API to make things easy for you.

```csharp
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
```

- Then you need to register your services.

```csharp
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			...
            // First you need add Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect("localhost:6347");
            });
            // Then add RedisBranch with assemblies that contains your repository classes. 
            services.AddRedisBranch(Assembly.GetExecutingAssembly());
			...
		}
    }
```

- After that you can use your Redis Repositories.

```csharp
    public class HomeController : ControllerBase
    {
        private readonly IRedisRepository<StockEntity> stockRepository;

        public HomeController(IRedisRepository<StockEntity> stockRepository)
        {
            this.stockRepository = stockRepository;
        }

        public async Task HowToUse()
        {            
            // Adds or updates entity
            await stockRepository.AddAsync(entity);
            // Adds or updates entity
            await stockRepository.UpdateAsync(entity);
            // Try find and delete entity. If so returns true 
            bool isDeleted = await stockRepository.DeleteAsync(entity);
            // Try find and delete entity. If so returns true 
            bool isDeleted = await stockRepository.DeleteAsync(entity.Id);
            // Get entity by id
            StockEntity se = await stockRepository.DeleteAsync("0daae400-9e7d-4985-acc2-9c144cfd4ed3");
            // Get stock entities which are in Technolohy Sector
            IEnumerable<StockEntity> technologyEntities = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.Technology);
            // Get stock entities which are sorted by CreatedDateTime
            IEnumerable<StockEntity> sortedCDTEntities = await stockRepository.GetAsync(StockRepository.BRANCH_SORTBY_CREATEDDATETIME);
            // Get stock entities which are in Technolohy Sector and sorted by Price
            IEnumerable<StockEntity> technologySortedPriceEntities = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICE, StockSector.Technology);
            // Get stock entities which are in Technolohy Sector and PriceChangeRate between 0.05-0.10 and sorted by PriceChangeRate
            IEnumerable<StockEntity> technologEntities = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE, StockSector.Technology);
            // Count stock entities which are in Technolohy Sector and has Great profit level
            IEnumerable<StockEntity> technologySortedPriceEntities = await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, StockSector.Technology, "Great");
            // Count stock entities which are in Technolohy Sector and has Great profit level and their prices between 50.00 and 150.00
            IEnumerable<StockEntity> technologySortedPriceEntities = await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, 50.00, 150.00, StockSector.Technology, "Great");
            
        }
    }
```

<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/dogabariscakmak/StackExchange.Redis.Branch/issues) for a list of proposed features (and known issues).

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.



<!-- CONTACT -->
## Contact

Doğa Barış Çakmak - dogabaris.cakmak@gmail.com

Project Link: [https://github.com/dogabariscakmak/StackExchange.Redis.Branch](https://github.com/dogabariscakmak/StackExchange.Redis.Branch)

*Created my free logo at LogoMakr.com*

[issues-shield]: https://img.shields.io/github/issues/dogabariscakmak/StackExchange.Redis.Branch.svg?style=flat-square
[issues-url]: https://github.com/dogabariscakmak/StackExchange.Redis.Branch/issues
[license-shield]: https://img.shields.io/github/license/dogabariscakmak/StackExchange.Redis.Branch.svg?style=flat-square
[license-url]: https://github.com/dogabariscakmak/StackExchange.Redis.Branch/blob/master/LICENSE
[product-screenshot]: usage.gif

using StackExchange.Redis.Branch.Repository;
using StackExchange.Redis.Branch.UnitTest.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StackExchange.Redis.Branch.UnitTest
{
    public class RedisRepositoryTest
    {
        [Fact]
        public void AddBranch_AddSingleBranch_ReturnCreatedBranchAndDefaultDataBranch()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            //Act
            IBranch<StockEntity> allBranch = new RedisBranch<StockEntity>();
            allBranch.SetBranchId("BRANCH_ALL");
            allBranch.FilterBy(i => i.IsActive).GroupBy("All", i => "All");
            stubStockRepository.AddBranch(allBranch);

            //Assert 
            Assert.Equal(2, stubStockRepository.GetBranches().Count());
        }

        [Fact]
        public async void AddGetEntity_AddEnttiy_ReturnEntityById()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> allBranch = new RedisBranch<StockEntity>();
            allBranch.SetBranchId("BRANCH_ALL");
            allBranch.FilterBy(i => i.IsActive).GroupBy("All", i => "All");
            stubStockRepository.AddBranch(allBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            StockEntity expectedTeslaEntity = await stubStockRepository.GetByIdAsync(teslaEntity.Id);

            //Assert 
            Assert.Equal(expectedTeslaEntity.Id, teslaEntity.Id);
            Assert.Equal(expectedTeslaEntity.Name, teslaEntity.Name);
            Assert.Equal(expectedTeslaEntity.Sector, teslaEntity.Sector);
            Assert.Equal(expectedTeslaEntity.Price, teslaEntity.Price);
            Assert.Equal(expectedTeslaEntity.PriceChangeRate, teslaEntity.PriceChangeRate);
            Assert.Equal(expectedTeslaEntity.CreatedDateTime, teslaEntity.CreatedDateTime);
            Assert.Equal(expectedTeslaEntity.IsActive, teslaEntity.IsActive);
        }

        [Fact]
        public async void AddGetEntity_AddEnttiy_ReturnEntityByPropertyGroupBranch()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sectorBranch = new RedisBranch<StockEntity>();
            sectorBranch.SetBranchId("BRANCH_SECTOR");
            sectorBranch.FilterBy(i => i.IsActive).GroupBy("Sector");
            stubStockRepository.AddBranch(sectorBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            IEnumerable<StockEntity> expectedEntities = await stubStockRepository.GetByBranchAsync("BRANCH_SECTOR", "Technology");

            //Assert 
            Assert.Single(expectedEntities);
            Assert.Equal(expectedEntities.ElementAt(0).Id, teslaEntity.Id);
            Assert.Equal(expectedEntities.ElementAt(0).Name, teslaEntity.Name);
            Assert.Equal(expectedEntities.ElementAt(0).Sector, teslaEntity.Sector);
            Assert.Equal(expectedEntities.ElementAt(0).Price, teslaEntity.Price);
            Assert.Equal(expectedEntities.ElementAt(0).PriceChangeRate, teslaEntity.PriceChangeRate);
            Assert.Equal(expectedEntities.ElementAt(0).CreatedDateTime, teslaEntity.CreatedDateTime);
            Assert.Equal(expectedEntities.ElementAt(0).IsActive, teslaEntity.IsActive);
        }

        [Fact]
        public async void AddCountEntity_AddEnttiy_ReturnCountByBranch()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sectorBranch = new RedisBranch<StockEntity>();
            sectorBranch.SetBranchId("BRANCH_SECTOR");
            sectorBranch.FilterBy(i => i.IsActive).GroupBy("Sector");
            stubStockRepository.AddBranch(sectorBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);
            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);
            long actualCount = await stubStockRepository.CountByBranchAsync("BRANCH_SECTOR", "Technology");

            //Assert 
            Assert.Equal(3, actualCount);
        }

        [Fact]
        public async void AddGetEntity_AddEnttiy_ReturnEntityByPropertySortBranch()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sortByPriceChangeRateBranch = new RedisBranch<StockEntity>();
            sortByPriceChangeRateBranch.SetBranchId("BRANCH_SORT_PRICE_CHANGE_RATE");
            sortByPriceChangeRateBranch.FilterBy(i => i.IsActive).SortBy("PriceChangeRate");
            stubStockRepository.AddBranch(sortByPriceChangeRateBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);
            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            IEnumerable<StockEntity> expectedEntities = await stubStockRepository.GetBySortedBranchAsync("BRANCH_SORT_PRICE_CHANGE_RATE", (long)9.5);

            //Assert 
            Assert.Equal(2, expectedEntities.Count());

            Assert.Equal(expectedEntities.ElementAt(0).Id, microsoftEntity.Id);
            Assert.Equal(expectedEntities.ElementAt(0).Name, microsoftEntity.Name);
            Assert.Equal(expectedEntities.ElementAt(0).Sector, microsoftEntity.Sector);
            Assert.Equal(expectedEntities.ElementAt(0).Price, microsoftEntity.Price);
            Assert.Equal(expectedEntities.ElementAt(0).PriceChangeRate, microsoftEntity.PriceChangeRate);
            Assert.Equal(expectedEntities.ElementAt(0).CreatedDateTime, microsoftEntity.CreatedDateTime);
            Assert.Equal(expectedEntities.ElementAt(0).IsActive, microsoftEntity.IsActive);

            Assert.Equal(expectedEntities.ElementAt(1).Id, teslaEntity.Id);
            Assert.Equal(expectedEntities.ElementAt(1).Name, teslaEntity.Name);
            Assert.Equal(expectedEntities.ElementAt(1).Sector, teslaEntity.Sector);
            Assert.Equal(expectedEntities.ElementAt(1).Price, teslaEntity.Price);
            Assert.Equal(expectedEntities.ElementAt(1).PriceChangeRate, teslaEntity.PriceChangeRate);
            Assert.Equal(expectedEntities.ElementAt(1).CreatedDateTime, teslaEntity.CreatedDateTime);
            Assert.Equal(expectedEntities.ElementAt(1).IsActive, teslaEntity.IsActive);
        }

        [Fact]
        public async void AddCountEntity_AddEnttiy_ReturnCountBySortedBranch()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sectorAndSortedCreatedDateTimeBranch = new RedisBranch<StockEntity>();
            sectorAndSortedCreatedDateTimeBranch.SetBranchId("BRANCH_SECTOR_SORT_PRICE");
            sectorAndSortedCreatedDateTimeBranch.FilterBy(i => i.IsActive).GroupBy("Sector").SortBy("Price");
            stubStockRepository.AddBranch(sectorAndSortedCreatedDateTimeBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            teslaEntity.CreatedDateTime = DateTimeOffset.UtcNow.AddSeconds(-15).DateTime;
            await stubStockRepository.AddAsync(teslaEntity);
            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);
            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            long actualCount = await stubStockRepository.CountBySortedBranchAsync("BRANCH_SECTOR_SORT_PRICE", 210, 250, "Technology");

            //Assert 
            Assert.Equal(1, actualCount);
        }

        [Fact]
        public async void AddGetEntity_AddEnttiy_ReturnEntityByFunctionSortBranch()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sectorAndSortedCreatedDateTimeBranch = new RedisBranch<StockEntity>();
            sectorAndSortedCreatedDateTimeBranch.SetBranchId("BRANCH_SECTOR_SORT_CREATED_DATETIME");
            sectorAndSortedCreatedDateTimeBranch.FilterBy(i => i.IsActive).GroupBy("Sector").SortBy("CreatedDateTimeSort", x => x.CreatedDateTime.Ticks);
            stubStockRepository.AddBranch(sectorAndSortedCreatedDateTimeBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            teslaEntity.CreatedDateTime = DateTimeOffset.UtcNow.AddSeconds(-15).DateTime;
            await stubStockRepository.AddAsync(teslaEntity);
            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);
            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            IEnumerable<StockEntity> expectedEntities = await stubStockRepository.GetBySortedBranchAsync("BRANCH_SECTOR_SORT_CREATED_DATETIME", DateTimeOffset.UtcNow.AddSeconds(-10).Ticks, StockSector.Technology.ToString());

            //Assert 
            Assert.Equal(2, expectedEntities.Count());

            Assert.Equal(expectedEntities.ElementAt(0).Id, microsoftEntity.Id);
            Assert.Equal(expectedEntities.ElementAt(0).Name, microsoftEntity.Name);
            Assert.Equal(expectedEntities.ElementAt(0).Sector, microsoftEntity.Sector);
            Assert.Equal(expectedEntities.ElementAt(0).Price, microsoftEntity.Price);
            Assert.Equal(expectedEntities.ElementAt(0).PriceChangeRate, microsoftEntity.PriceChangeRate);
            Assert.Equal(expectedEntities.ElementAt(0).CreatedDateTime, microsoftEntity.CreatedDateTime);
            Assert.Equal(expectedEntities.ElementAt(0).IsActive, microsoftEntity.IsActive);

            Assert.Equal(expectedEntities.ElementAt(1).Id, appleEntity.Id);
            Assert.Equal(expectedEntities.ElementAt(1).Name, appleEntity.Name);
            Assert.Equal(expectedEntities.ElementAt(1).Sector, appleEntity.Sector);
            Assert.Equal(expectedEntities.ElementAt(1).Price, appleEntity.Price);
            Assert.Equal(expectedEntities.ElementAt(1).PriceChangeRate, appleEntity.PriceChangeRate);
            Assert.Equal(expectedEntities.ElementAt(1).CreatedDateTime, appleEntity.CreatedDateTime);
            Assert.Equal(expectedEntities.ElementAt(1).IsActive, appleEntity.IsActive);
        }

        [Fact]
        public async void AddGetEntity_AddEnttiy_ReturnEntityByFunctionGroupSortBranch()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> groupFunctionProfitLevelSortedPriceRateBranch = new RedisBranch<StockEntity>();
            groupFunctionProfitLevelSortedPriceRateBranch.SetBranchId("BRANCH_PROFIT_LEVEL_SORT_PRICE_RATE");
            groupFunctionProfitLevelSortedPriceRateBranch.FilterBy(i => i.IsActive).GroupBy("Profit", x => stubStockRepository.GetProfitLevel(x)).SortBy("CreatedDateTimeSort", x => x.CreatedDateTime.Ticks);
            stubStockRepository.AddBranch(groupFunctionProfitLevelSortedPriceRateBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            StockEntity amazonEntity = new StockEntity("AMAZON", StockSector.Technology, 329.00, 11.5);
            await stubStockRepository.AddAsync(amazonEntity);
            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);
            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, -0.5);
            await stubStockRepository.AddAsync(appleEntity);

            IEnumerable<StockEntity> expectedGreatProfitEntities = await stubStockRepository.GetBySortedBranchAsync("BRANCH_PROFIT_LEVEL_SORT_PRICE_RATE", "GreatProfit");
            IEnumerable<StockEntity> expectedProfitEntities = await stubStockRepository.GetBySortedBranchAsync("BRANCH_PROFIT_LEVEL_SORT_PRICE_RATE", "NormalProfit");
            IEnumerable<StockEntity> expectedLossProfitEntities = await stubStockRepository.GetBySortedBranchAsync("BRANCH_PROFIT_LEVEL_SORT_PRICE_RATE", "Loss");

            //Assert 
            Assert.Equal(2, expectedGreatProfitEntities.Count());

            Assert.Equal(expectedGreatProfitEntities.ElementAt(0).Id, teslaEntity.Id);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(0).Name, teslaEntity.Name);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(0).Sector, teslaEntity.Sector);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(0).Price, teslaEntity.Price);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(0).PriceChangeRate, teslaEntity.PriceChangeRate);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(0).CreatedDateTime, teslaEntity.CreatedDateTime);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(0).IsActive, teslaEntity.IsActive);

            Assert.Equal(expectedGreatProfitEntities.ElementAt(1).Id, amazonEntity.Id);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(1).Name, amazonEntity.Name);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(1).Sector, amazonEntity.Sector);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(1).Price, amazonEntity.Price);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(1).PriceChangeRate, amazonEntity.PriceChangeRate);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(1).CreatedDateTime, amazonEntity.CreatedDateTime);
            Assert.Equal(expectedGreatProfitEntities.ElementAt(1).IsActive, amazonEntity.IsActive);


            Assert.Single(expectedProfitEntities);

            Assert.Equal(expectedProfitEntities.ElementAt(0).Id, microsoftEntity.Id);
            Assert.Equal(expectedProfitEntities.ElementAt(0).Name, microsoftEntity.Name);
            Assert.Equal(expectedProfitEntities.ElementAt(0).Sector, microsoftEntity.Sector);
            Assert.Equal(expectedProfitEntities.ElementAt(0).Price, microsoftEntity.Price);
            Assert.Equal(expectedProfitEntities.ElementAt(0).PriceChangeRate, microsoftEntity.PriceChangeRate);
            Assert.Equal(expectedProfitEntities.ElementAt(0).CreatedDateTime, microsoftEntity.CreatedDateTime);
            Assert.Equal(expectedProfitEntities.ElementAt(0).IsActive, microsoftEntity.IsActive);


            Assert.Single(expectedLossProfitEntities);

            Assert.Equal(expectedLossProfitEntities.ElementAt(0).Id, appleEntity.Id);
            Assert.Equal(expectedLossProfitEntities.ElementAt(0).Name, appleEntity.Name);
            Assert.Equal(expectedLossProfitEntities.ElementAt(0).Sector, appleEntity.Sector);
            Assert.Equal(expectedLossProfitEntities.ElementAt(0).Price, appleEntity.Price);
            Assert.Equal(expectedLossProfitEntities.ElementAt(0).PriceChangeRate, appleEntity.PriceChangeRate);
            Assert.Equal(expectedLossProfitEntities.ElementAt(0).CreatedDateTime, appleEntity.CreatedDateTime);
            Assert.Equal(expectedLossProfitEntities.ElementAt(0).IsActive, appleEntity.IsActive);
        }

        [Fact]
        public async void AddGetEntity_AddEnttiy_ReturnEntityByIdAndNullForIgnoreDataMemberProperty()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            string dummyString = teslaEntity.DummyString;
            string entityId = teslaEntity.Id;
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity expectedEnty = await stubStockRepository.GetByIdAsync(entityId);

            //Assert 
            Assert.NotNull(expectedEnty);

            Assert.Equal(expectedEnty.Id, teslaEntity.Id);
            Assert.Equal(expectedEnty.Name, teslaEntity.Name);
            Assert.Equal(expectedEnty.Sector, teslaEntity.Sector);
            Assert.Equal(expectedEnty.Price, teslaEntity.Price);
            Assert.Equal(expectedEnty.PriceChangeRate, teslaEntity.PriceChangeRate);
            Assert.Equal(expectedEnty.CreatedDateTime, teslaEntity.CreatedDateTime);
            Assert.Equal(expectedEnty.IsActive, teslaEntity.IsActive);
            Assert.NotNull(dummyString);
            Assert.True(expectedEnty.DummyString == default);
        }

        [Fact]
        public async void AddGetEntity_AddEnttiy_ReturnEntityByIdAndClassProperty()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            //Act
            StockMetaData metaData = new StockMetaData("USA", CurrencyCode.USD);
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5, metaData);
            string dummyString = teslaEntity.DummyString;
            string entityId = teslaEntity.Id;
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity expectedEntity = await stubStockRepository.GetByIdAsync(entityId);

            //Assert 
            Assert.NotNull(expectedEntity);

            Assert.Equal(expectedEntity.Id, teslaEntity.Id);
            Assert.Equal(expectedEntity.Name, teslaEntity.Name);
            Assert.Equal(expectedEntity.Sector, teslaEntity.Sector);
            Assert.Equal(expectedEntity.Price, teslaEntity.Price);
            Assert.Equal(expectedEntity.PriceChangeRate, teslaEntity.PriceChangeRate);
            Assert.Equal(expectedEntity.CreatedDateTime, teslaEntity.CreatedDateTime);
            Assert.Equal(expectedEntity.IsActive, teslaEntity.IsActive);
            Assert.NotNull(dummyString);
            Assert.True(expectedEntity.DummyString == default);

            Assert.Equal(expectedEntity.MetaData.Currency, metaData.Currency);
            Assert.Equal(expectedEntity.MetaData.Country, metaData.Country);
            Assert.Equal(expectedEntity.MetaData.UpdateDateTime, metaData.UpdateDateTime);
        }

        [Fact]
        public async void AddSetKeyExpirationForKeyGetEntity_AddEnttiy_ReturnEntityById()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sectorBranch = new RedisBranch<StockEntity>();
            sectorBranch.SetBranchId("BRANCH_SECTOR");
            sectorBranch.FilterBy(i => i.IsActive).GroupBy("Sector");
            stubStockRepository.AddBranch(sectorBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            StockEntity amazonEntity = new StockEntity("AMAZON", StockSector.Technology, 329.00, 11.5);
            await stubStockRepository.AddAsync(amazonEntity);

            await stubStockRepository.SetKeyExpireAsync(teslaEntity.Id, new TimeSpan(0, 0, 1));

            Thread.Sleep(1000 * 2);

            StockEntity expectedNullEntity = await stubStockRepository.GetByIdAsync(teslaEntity.Id);
            StockEntity expectedAmazonEntity = await stubStockRepository.GetByIdAsync(amazonEntity.Id);

            //Assert 
            Assert.Null(expectedNullEntity);

            Assert.NotNull(expectedAmazonEntity);

            Assert.Equal(expectedAmazonEntity.Id, amazonEntity.Id);
            Assert.Equal(expectedAmazonEntity.Name, amazonEntity.Name);
            Assert.Equal(expectedAmazonEntity.Sector, amazonEntity.Sector);
            Assert.Equal(expectedAmazonEntity.Price, amazonEntity.Price);
            Assert.Equal(expectedAmazonEntity.PriceChangeRate, amazonEntity.PriceChangeRate);
            Assert.Equal(expectedAmazonEntity.CreatedDateTime, amazonEntity.CreatedDateTime);
            Assert.Equal(expectedAmazonEntity.IsActive, amazonEntity.IsActive);
        }

        [Fact]
        public async void AddSetKeyExpirationForKeyGetEntity_AddEnttiy_ReturnEntityByPropertyGroupBranch()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sectorBranch = new RedisBranch<StockEntity>();
            sectorBranch.SetBranchId("BRANCH_SECTOR");
            sectorBranch.FilterBy(i => i.IsActive).GroupBy("Sector");
            stubStockRepository.AddBranch(sectorBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            StockEntity amazonEntity = new StockEntity("AMAZON", StockSector.Technology, 329.00, 11.5);
            await stubStockRepository.AddAsync(amazonEntity);

            await stubStockRepository.SetKeyExpireAsync(teslaEntity.Id, new TimeSpan(0, 0, 1));

            Thread.Sleep(1000 * 2);

            IEnumerable<StockEntity> expectedEntites = await stubStockRepository.GetByBranchAsync("BRANCH_SECTOR", StockSector.Technology.ToString());


            //Assert 
            Assert.Single(expectedEntites);

            Assert.Equal(expectedEntites.ElementAt(0).Id, amazonEntity.Id);
            Assert.Equal(expectedEntites.ElementAt(0).Name, amazonEntity.Name);
            Assert.Equal(expectedEntites.ElementAt(0).Sector, amazonEntity.Sector);
            Assert.Equal(expectedEntites.ElementAt(0).Price, amazonEntity.Price);
            Assert.Equal(expectedEntites.ElementAt(0).PriceChangeRate, amazonEntity.PriceChangeRate);
            Assert.Equal(expectedEntites.ElementAt(0).CreatedDateTime, amazonEntity.CreatedDateTime);
            Assert.Equal(expectedEntites.ElementAt(0).IsActive, amazonEntity.IsActive);
        }

        [Fact]
        public async void AddSetKeyExpirationForEntityGetEntity_AddEnttiy_ReturnEntityByPropertyGroupBranch()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sectorBranch = new RedisBranch<StockEntity>();
            sectorBranch.SetBranchId("BRANCH_SECTOR");
            sectorBranch.FilterBy(i => i.IsActive).GroupBy("Sector");
            stubStockRepository.AddBranch(sectorBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            StockEntity amazonEntity = new StockEntity("AMAZON", StockSector.Technology, 329.00, 11.5);
            await stubStockRepository.AddAsync(amazonEntity);

            await stubStockRepository.SetKeyExpireAsync(teslaEntity, new TimeSpan(0, 0, 1));

            Thread.Sleep(1000 * 2);

            IEnumerable<StockEntity> expectedEntites = await stubStockRepository.GetByBranchAsync("BRANCH_SECTOR", StockSector.Technology.ToString());


            //Assert 
            Assert.Single(expectedEntites);

            Assert.Equal(expectedEntites.ElementAt(0).Id, amazonEntity.Id);
            Assert.Equal(expectedEntites.ElementAt(0).Name, amazonEntity.Name);
            Assert.Equal(expectedEntites.ElementAt(0).Sector, amazonEntity.Sector);
            Assert.Equal(expectedEntites.ElementAt(0).Price, amazonEntity.Price);
            Assert.Equal(expectedEntites.ElementAt(0).PriceChangeRate, amazonEntity.PriceChangeRate);
            Assert.Equal(expectedEntites.ElementAt(0).CreatedDateTime, amazonEntity.CreatedDateTime);
            Assert.Equal(expectedEntites.ElementAt(0).IsActive, amazonEntity.IsActive);
        }

        [Fact]
        public async void AddUpdateGetEntity_AddUpdateEntiy_ReturnEntityByIdWithUpdatedValues()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            //Act
            StockMetaData metaData = new StockMetaData("USA", CurrencyCode.USD);
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5, metaData);
            string entityId = teslaEntity.Id;
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity updatedEntity = await stubStockRepository.GetByIdAsync(entityId);
            updatedEntity.Sector = StockSector.Energy;
            updatedEntity.Name = "Rhein Energy";
            updatedEntity.CreatedDateTime = DateTime.UtcNow.AddDays(-1);
            updatedEntity.Price = 119.00;
            updatedEntity.PriceChangeRate = 2.7;
            updatedEntity.MetaData.Country = "Germany";
            updatedEntity.MetaData.Currency = CurrencyCode.EURO;
            updatedEntity.MetaData.UpdateDateTime = DateTime.UtcNow;

            await stubStockRepository.UpdateAsync(updatedEntity);
            StockEntity reEntity = await stubStockRepository.GetByIdAsync(entityId);

            //Assert 
            Assert.NotNull(reEntity);

            Assert.Equal(updatedEntity.Id, reEntity.Id);
            Assert.Equal(updatedEntity.Name, reEntity.Name);
            Assert.Equal(updatedEntity.Sector, reEntity.Sector);
            Assert.Equal(updatedEntity.Price, reEntity.Price);
            Assert.Equal(updatedEntity.PriceChangeRate, reEntity.PriceChangeRate);
            Assert.Equal(updatedEntity.CreatedDateTime, reEntity.CreatedDateTime);
            Assert.Equal(updatedEntity.IsActive, reEntity.IsActive);
            Assert.True(updatedEntity.DummyString == default);

            Assert.Equal(updatedEntity.MetaData.Currency, reEntity.MetaData.Currency);
            Assert.Equal(updatedEntity.MetaData.Country, reEntity.MetaData.Country);
            Assert.Equal(updatedEntity.MetaData.UpdateDateTime, reEntity.MetaData.UpdateDateTime);
        }

        [Fact]
        public async void AddDeleteGetEntity_AddDeleteEntiyById_ReturnEntityByIdWithNull()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            //Act
            StockMetaData metaData = new StockMetaData("USA", CurrencyCode.USD);
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5, metaData);
            string entityId = teslaEntity.Id;
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity savedEntity = await stubStockRepository.GetByIdAsync(entityId);

            await stubStockRepository.DeleteAsync(savedEntity.Id);
            StockEntity deletedEntity = await stubStockRepository.GetByIdAsync(entityId);

            //Assert 
            Assert.Null(deletedEntity);
        }

        [Fact]
        public async void AddDeleteGetEntity_AddDeleteEntiy_ReturnEntityByIdWithNull()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            //Act
            StockMetaData metaData = new StockMetaData("USA", CurrencyCode.USD);
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5, metaData);
            string entityId = teslaEntity.Id;
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity savedEntity = await stubStockRepository.GetByIdAsync(entityId);

            await stubStockRepository.DeleteAsync(savedEntity);
            StockEntity deletedEntity = await stubStockRepository.GetByIdAsync(entityId);

            //Assert 
            Assert.Null(deletedEntity);
        }

        [Fact]
        public async void AddGetEntity_AddEntiy_ReturnEntityByPropertyInvalidBranchId_ThrowsKeyNotFoundException()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sectorBranch = new RedisBranch<StockEntity>();
            sectorBranch.SetBranchId("BRANCH_SECTOR");
            sectorBranch.FilterBy(i => i.IsActive).GroupBy("Sector");
            stubStockRepository.AddBranch(sectorBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            Func<Task> act  = async () => await stubStockRepository.GetByBranchAsync("NotExist", "");

            //Assert 
            KeyNotFoundException exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
            Assert.StartsWith("branchId not found: NotExist.", exception.Message);
        }

        [Fact]
        public async void AddGetEntity_AddEnttiy_ReturnEntityByPropertySortBranch_ThrowsKeyNotFoundException()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sortByPriceChangeRateBranch = new RedisBranch<StockEntity>();
            sortByPriceChangeRateBranch.SetBranchId("BRANCH_SORT_PRICE_CHANGE_RATE");
            sortByPriceChangeRateBranch.FilterBy(i => i.IsActive).SortBy("PriceChangeRate");
            stubStockRepository.AddBranch(sortByPriceChangeRateBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            Func<Task> act = async () => await stubStockRepository.GetByBranchAsync("NotExist", "");

            //Assert 
            KeyNotFoundException exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
            Assert.StartsWith("branchId not found: NotExist.", exception.Message);
        }

        [Fact]
        public async void AddCountEntity_AddEnttiy_ReturnCountByBranch_ThrowsKeyNotFoundException()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sortByPriceChangeRateBranch = new RedisBranch<StockEntity>();
            sortByPriceChangeRateBranch.SetBranchId("BRANCH_SORT_PRICE_CHANGE_RATE");
            sortByPriceChangeRateBranch.FilterBy(i => i.IsActive).SortBy("PriceChangeRate");
            stubStockRepository.AddBranch(sortByPriceChangeRateBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            Func<Task> act = async () => await stubStockRepository.CountByBranchAsync("NotExist", "");

            //Assert 
            KeyNotFoundException exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
            Assert.StartsWith("branchId not found: NotExist.", exception.Message);
        }

        [Fact]
        public async void AddCountEntity_AddEnttiy_ReturnCountBySortedBranch_ThrowsKeyNotFoundException()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();

            IBranch<StockEntity> sortByPriceChangeRateBranch = new RedisBranch<StockEntity>();
            sortByPriceChangeRateBranch.SetBranchId("BRANCH_SORT_PRICE_CHANGE_RATE");
            sortByPriceChangeRateBranch.FilterBy(i => i.IsActive).SortBy("PriceChangeRate");
            stubStockRepository.AddBranch(sortByPriceChangeRateBranch);

            //Act
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            Func<Task> act = async () => await stubStockRepository.CountByBranchAsync("NotExist", "");

            //Assert 
            KeyNotFoundException exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
            Assert.StartsWith("branchId not found: NotExist.", exception.Message);
        }
    }
}

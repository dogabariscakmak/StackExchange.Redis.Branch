using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Branch.IntegrationTest.Fakes;
using StackExchange.Redis.Branch.IntegrationTest.Helpers;
using StackExchange.Redis.Branch.Repository;
using System;
using System.Collections.Generic;
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

        [Fact]
        public async Task Check_DataShouldBeAdded()
        {
            //Arrange and Act
            await fixture.ReloadTestDataAsync();

            //Assert
            Assert.Equal(9, stockRepository.GetBranches().Count());

            #region BRANCH_GROUPALL
            Assert.Equal(fixture.TestData.Count(i => i.IsActive),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPALL, "All"));
            #endregion

            #region BRANCH_GROUPBY_SECTOR
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.CommunicationServices),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.CommunicationServices.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.ConsumerDiscretionary),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.ConsumerDiscretionary.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.ConsumerStaples),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.ConsumerStaples.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.Energy),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.Energy.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.Financials),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.Financials.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.HealthCare),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.HealthCare.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.Industrials),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.Industrials.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.InformationTechnology),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.InformationTechnology.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.Materials),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.Materials.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.None),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.None.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.RealEstate),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.RealEstate.ToString()));
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.Utilities),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, StockSector.Utilities.ToString()));
            #endregion

            #region BRANCH_SORTBY_CREATEDDATETIME
            long fromCreateDateTimeTicks = DateTime.UtcNow.AddMinutes(-60).Ticks;
            long toCreateDateTimeTicks = DateTime.UtcNow.AddMinutes(60).Ticks;
            IEnumerable<StockEntity> BRANCH_SORTBY_CREATEDDATETIME = await stockRepository.GetAsync(StockRepository.BRANCH_SORTBY_CREATEDDATETIME, fromCreateDateTimeTicks, toCreateDateTimeTicks);
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.CreatedDateTime.Ticks >= fromCreateDateTimeTicks && i.CreatedDateTime.Ticks <= toCreateDateTimeTicks),
                         BRANCH_SORTBY_CREATEDDATETIME.Count());
            foreach (StockEntity stockEntity in BRANCH_SORTBY_CREATEDDATETIME)
            {
                Assert.InRange(stockEntity.CreatedDateTime.Ticks, fromCreateDateTimeTicks, toCreateDateTimeTicks);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_SORTBY_PRICE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_SORTBY_PRICE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICE, 88.15, 136.8, StockSector.InformationTechnology.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.InformationTechnology && i.Price >= 88.15 && i.Price <= 136.8),
                         BRANCH_GROUPBY_SECTOR_SORTBY_PRICE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_SORTBY_PRICE)
            {
                Assert.InRange(stockEntity.Price, 88.15, 136.8);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE, 0.10, StockSector.InformationTechnology.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.InformationTechnology && i.PriceChangeRate >= 0.10),
                         BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE)
            {
                Assert.True(stockEntity.PriceChangeRate >= 0.10);
            }
            #endregion

            #region BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, 0.10, 0.15, ProfitLevel.Great.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && stockRepository.GetProfitLevel(i) == ProfitLevel.Great && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, 0.10, 0.15, StockSector.InformationTechnology.ToString(), ProfitLevel.Great.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && stockRepository.GetProfitLevel(i) == ProfitLevel.Great && i.Sector == StockSector.InformationTechnology && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion

            #region BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE, 0.10, 0.15, "GERMANY");
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.MetaData.Country == "GERMANY" && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion
        }

        [Fact]
        public async Task DeleteById_EntityShouldBeDeletedFromAllBranches()
        {
            //Arrange
            await fixture.ReloadTestDataAsync();

            long fromCreateDateTimeTicks = DateTime.UtcNow.AddMinutes(-720).Ticks;
            long toCreateDateTimeTicks = DateTime.UtcNow.AddMinutes(720).Ticks;
            StockEntity deletedEntity = fixture.TestData.FirstOrDefault(i => i.IsActive &&
                                                                             i.Sector == StockSector.InformationTechnology &&
                                                                             i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15 &&
                                                                             stockRepository.GetProfitLevel(i) == ProfitLevel.Great &&
                                                                             i.CreatedDateTime.Ticks >= fromCreateDateTimeTicks && i.CreatedDateTime.Ticks <= toCreateDateTimeTicks &&
                                                                             i.Price >= 88.15 && i.Price <= 283.84 && 
                                                                             i.MetaData.Country == "GERMANY");
            fixture.TestData.Remove(deletedEntity);
            
            //Act
            bool result = await stockRepository.DeleteAsync(deletedEntity.Id);

            //Assert
            Assert.True(result);

            #region BRANCH_GROUPALL
            Assert.Equal(fixture.TestData.Count(i => i.IsActive),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPALL, "All"));
            #endregion

            #region BRANCH_GROUPBY_SECTOR
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.InformationTechnology),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, deletedEntity.Sector.ToString()));
            #endregion

            #region BRANCH_SORTBY_CREATEDDATETIME
            IEnumerable<StockEntity> BRANCH_SORTBY_CREATEDDATETIME = await stockRepository.GetAsync(StockRepository.BRANCH_SORTBY_CREATEDDATETIME, fromCreateDateTimeTicks, toCreateDateTimeTicks);
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.CreatedDateTime.Ticks >= fromCreateDateTimeTicks && i.CreatedDateTime.Ticks <= toCreateDateTimeTicks),
                         BRANCH_SORTBY_CREATEDDATETIME.Count());
            foreach (StockEntity stockEntity in BRANCH_SORTBY_CREATEDDATETIME)
            {
                Assert.InRange(stockEntity.CreatedDateTime.Ticks, fromCreateDateTimeTicks, toCreateDateTimeTicks);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_SORTBY_PRICE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_SORTBY_PRICE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICE, 88.15, 283.84, StockSector.InformationTechnology.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.InformationTechnology && i.Price >= 88.15 && i.Price <= 283.84),
                         BRANCH_GROUPBY_SECTOR_SORTBY_PRICE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_SORTBY_PRICE)
            {
                Assert.InRange(stockEntity.Price, 88.15, 283.84);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE, 0.10, StockSector.InformationTechnology.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.InformationTechnology && i.PriceChangeRate >= 0.10),
                         BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE)
            {
                Assert.True(stockEntity.PriceChangeRate >= 0.10);
            }
            #endregion

            #region BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, 0.10, 0.15, ProfitLevel.Great.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && stockRepository.GetProfitLevel(i) == ProfitLevel.Great && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, 0.10, 0.15, StockSector.InformationTechnology.ToString(), ProfitLevel.Great.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && stockRepository.GetProfitLevel(i) == ProfitLevel.Great && i.Sector == StockSector.InformationTechnology && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion

            #region BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE, 0.10, 0.15, "GERMANY");
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.MetaData.Country == "GERMANY" && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion
        }

        [Fact]
        public async Task Delete_EntityShouldBeDeletedFromAllBranches()
        {
            //Arrange
            await fixture.ReloadTestDataAsync();

            long fromCreateDateTimeTicks = DateTime.UtcNow.AddMinutes(-720).Ticks;
            long toCreateDateTimeTicks = DateTime.UtcNow.AddMinutes(720).Ticks;
            StockEntity deletedEntity = fixture.TestData.FirstOrDefault(i => i.IsActive &&
                                                                             i.Sector == StockSector.InformationTechnology &&
                                                                             i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15 &&
                                                                             stockRepository.GetProfitLevel(i) == ProfitLevel.Great &&
                                                                             i.CreatedDateTime.Ticks >= fromCreateDateTimeTicks && i.CreatedDateTime.Ticks <= toCreateDateTimeTicks &&
                                                                             i.Price >= 88.15 && i.Price <= 300.86 &&
                                                                             i.MetaData.Country == "GERMANY");
            fixture.TestData.Remove(deletedEntity);
            //Act
            bool result = await stockRepository.DeleteAsync(deletedEntity);

            //Assert
            Assert.True(result);

            #region BRANCH_GROUPALL
            Assert.Equal(fixture.TestData.Count(i => i.IsActive),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPALL, "All"));
            #endregion

            #region BRANCH_GROUPBY_SECTOR
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.InformationTechnology),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, deletedEntity.Sector.ToString()));
            #endregion

            #region BRANCH_SORTBY_CREATEDDATETIME
            IEnumerable<StockEntity> BRANCH_SORTBY_CREATEDDATETIME = await stockRepository.GetAsync(StockRepository.BRANCH_SORTBY_CREATEDDATETIME, fromCreateDateTimeTicks, toCreateDateTimeTicks);
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.CreatedDateTime.Ticks >= fromCreateDateTimeTicks && i.CreatedDateTime.Ticks <= toCreateDateTimeTicks),
                         BRANCH_SORTBY_CREATEDDATETIME.Count());
            foreach (StockEntity stockEntity in BRANCH_SORTBY_CREATEDDATETIME)
            {
                Assert.InRange(stockEntity.CreatedDateTime.Ticks, fromCreateDateTimeTicks, toCreateDateTimeTicks);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_SORTBY_PRICE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_SORTBY_PRICE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICE, 88.15, 300.86, StockSector.InformationTechnology.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.InformationTechnology && i.Price >= 88.15 && i.Price <= 300.86),
                         BRANCH_GROUPBY_SECTOR_SORTBY_PRICE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_SORTBY_PRICE)
            {
                Assert.InRange(stockEntity.Price, 88.15, 300.86);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE, 0.10, StockSector.InformationTechnology.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == StockSector.InformationTechnology && i.PriceChangeRate >= 0.10),
                         BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE)
            {
                Assert.True(stockEntity.PriceChangeRate >= 0.10);
            }
            #endregion

            #region BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, 0.10, 0.15, ProfitLevel.Great.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && stockRepository.GetProfitLevel(i) == ProfitLevel.Great && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, 0.10, 0.15, StockSector.InformationTechnology.ToString(), ProfitLevel.Great.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && stockRepository.GetProfitLevel(i) == ProfitLevel.Great && i.Sector == StockSector.InformationTechnology && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion

            #region BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE, 0.10, 0.15, "GERMANY");
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.MetaData.Country == "GERMANY" && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion
        }

        [Fact]
        public async Task UpdateEntitySoThatNotMeetAnyBranchCriteria_EntityShouldBeUpdatedAndAllBranchesShouldNotReturnEntity()
        {
            //Arrange
            await fixture.ReloadTestDataAsync();

            long fromCreateDateTimeTicks = DateTime.UtcNow.AddMinutes(-720).Ticks;
            long toCreateDateTimeTicks = DateTime.UtcNow.AddMinutes(720).Ticks;
            StockEntity updatedEntity = fixture.TestData.FirstOrDefault(i => i.IsActive &&
                                                                             i.Sector == StockSector.Financials &&
                                                                             i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15 &&
                                                                             stockRepository.GetProfitLevel(i) == ProfitLevel.Great &&
                                                                             i.CreatedDateTime.Ticks >= fromCreateDateTimeTicks && i.CreatedDateTime.Ticks <= toCreateDateTimeTicks &&
                                                                             i.Price >= 92.74 && i.Price <= 278.92 &&
                                                                             i.MetaData.Country == "GERMANY");
            fixture.TestData.Remove(updatedEntity);
            updatedEntity.Sector = StockSector.HealthCare;
            updatedEntity.PriceChangeRate = 0.086;
            updatedEntity.CreatedDateTime = DateTime.UtcNow.AddDays(-2);
            updatedEntity.Price = 521.53;
            updatedEntity.MetaData.Country = "USD";
            updatedEntity.MetaData.Currency = CurrencyCode.USD;
            fixture.TestData.Add(updatedEntity);

            //Act
            await stockRepository.UpdateAsync(updatedEntity);

            //Assert
            #region BRANCH_GROUPALL
            Assert.Equal(fixture.TestData.Count(i => i.IsActive),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPALL, "All"));
            #endregion

            #region BRANCH_GROUPBY_SECTOR
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == updatedEntity.Sector),
                         await stockRepository.CountAsync(StockRepository.BRANCH_GROUPBY_SECTOR, updatedEntity.Sector.ToString()));
            #endregion

            #region BRANCH_SORTBY_CREATEDDATETIME
            IEnumerable<StockEntity> BRANCH_SORTBY_CREATEDDATETIME = await stockRepository.GetAsync(StockRepository.BRANCH_SORTBY_CREATEDDATETIME, fromCreateDateTimeTicks, toCreateDateTimeTicks);
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.CreatedDateTime.Ticks >= fromCreateDateTimeTicks && i.CreatedDateTime.Ticks <= toCreateDateTimeTicks),
                         BRANCH_SORTBY_CREATEDDATETIME.Count());
            foreach (StockEntity stockEntity in BRANCH_SORTBY_CREATEDDATETIME)
            {
                Assert.InRange(stockEntity.CreatedDateTime.Ticks, fromCreateDateTimeTicks, toCreateDateTimeTicks);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_SORTBY_PRICE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_SORTBY_PRICE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICE, 92.74, 278.92, updatedEntity.Sector.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == updatedEntity.Sector && i.Price >= 92.74 && i.Price <= 278.92),
                         BRANCH_GROUPBY_SECTOR_SORTBY_PRICE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_SORTBY_PRICE)
            {
                Assert.InRange(stockEntity.Price, 92.74, 278.92);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE, 0.10, updatedEntity.Sector.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.Sector == updatedEntity.Sector && i.PriceChangeRate >= 0.10),
                         BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_SORTBY_PRICECHANGERATE)
            {
                Assert.True(stockEntity.PriceChangeRate >= 0.10);
            }
            #endregion

            #region BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, 0.10, 0.15, ProfitLevel.Great.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && stockRepository.GetProfitLevel(i) == ProfitLevel.Great && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion

            #region BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE, 0.10, 0.15, updatedEntity.Sector.ToString(), ProfitLevel.Great.ToString());
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && stockRepository.GetProfitLevel(i) == ProfitLevel.Great && i.Sector == updatedEntity.Sector && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_SECTOR_GROUPBY_PROFITLEVEL_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion

            #region BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE
            IEnumerable<StockEntity> BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE = await stockRepository.GetAsync(StockRepository.BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE, 0.10, 0.15, updatedEntity.MetaData.Country);
            Assert.Equal(fixture.TestData.Count(i => i.IsActive && i.MetaData.Country == updatedEntity.MetaData.Country && i.PriceChangeRate >= 0.10 && i.PriceChangeRate <= 0.15),
                         BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE.Count());
            foreach (StockEntity stockEntity in BRANCH_GROUPBY_COUNTRY_SORTBY_PRICECHANGERATE)
            {
                Assert.InRange(stockEntity.PriceChangeRate, 0.10, 0.15);
            }
            #endregion
        }
    }
}

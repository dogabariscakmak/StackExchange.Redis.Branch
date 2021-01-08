using Newtonsoft.Json;
using StackExchange.Redis.Branch.UnitTest.Fakes;
using System;
using System.Linq;
using Xunit;

namespace StackExchange.Redis.Branch.UnitTest
{
    public class RedisDatabaseExtensionsTest
    {
        [Fact]
        public async void HashSetAndHashGetAll_HashSet_ReturnHashGetAllWithValues()
        {
            //Arrange 
            IDatabase fakeDatabase = FakesFactory.CreateDatabaseFake();

            //Act
            StockMetaData metaData = new StockMetaData("USA", CurrencyCode.USD);
            StockEntity expectedTeslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5, metaData);
            await fakeDatabase.HashSetAsync(expectedTeslaEntity);
            HashEntry[] hashEntries = await fakeDatabase.HashGetAllAsync(expectedTeslaEntity.GetRedisKey());

            //Assert 
            Assert.Equal(20, hashEntries.Count());

            Assert.Equal("Name", hashEntries[0].Name);
            Assert.Equal(expectedTeslaEntity.Name, hashEntries[0].Value);

            Assert.Equal("Symbol", hashEntries[1].Name);
            Assert.Equal(expectedTeslaEntity.Symbol, hashEntries[1].Value);

            Assert.Equal("Sector", hashEntries[2].Name);
            Assert.Equal(((int)expectedTeslaEntity.Sector).ToString(), hashEntries[2].Value);

            Assert.Equal("Price", hashEntries[3].Name);
            Assert.Equal(expectedTeslaEntity.Price.ToString(), hashEntries[3].Value);

            Assert.Equal("PriceChangeRate", hashEntries[4].Name);
            Assert.Equal(expectedTeslaEntity.PriceChangeRate.ToString(), hashEntries[4].Value);

            Assert.Equal("CreatedDateTime", hashEntries[5].Name);
            Assert.Equal(expectedTeslaEntity.CreatedDateTime.Kind == DateTimeKind.Utc ? 
                            $"{expectedTeslaEntity.CreatedDateTime.Ticks}|UTC": $"{expectedTeslaEntity.CreatedDateTime.Ticks}|LOC", 
                         hashEntries[5].Value);

            Assert.Equal("IsActive", hashEntries[6].Name);
            Assert.Equal(expectedTeslaEntity.IsActive.ToString(), hashEntries[6].Value);

            Assert.Equal("FirstLetterOfName", hashEntries[7].Name);
            Assert.Equal(expectedTeslaEntity.FirstLetterOfName.ToString(), hashEntries[7].Value);

            Assert.Equal("LastByteOfName", hashEntries[8].Name);
            Assert.Equal(expectedTeslaEntity.LastByteOfName.ToString(), hashEntries[8].Value);
            
            Assert.Equal("LengthOfNameShort", hashEntries[9].Name);
            Assert.Equal(expectedTeslaEntity.LengthOfNameShort.ToString(), hashEntries[9].Value);

            Assert.Equal("LengthOfNameUshort", hashEntries[10].Name);
            Assert.Equal(expectedTeslaEntity.LengthOfNameUshort.ToString(), hashEntries[10].Value);

            Assert.Equal("LengthOfNameInt", hashEntries[11].Name);
            Assert.Equal(expectedTeslaEntity.LengthOfNameInt.ToString(), hashEntries[11].Value);

            Assert.Equal("LengthOfNameUint", hashEntries[12].Name);
            Assert.Equal(expectedTeslaEntity.LengthOfNameUint.ToString(), hashEntries[12].Value);

            Assert.Equal("LengthOfNameLong", hashEntries[13].Name);
            Assert.Equal(expectedTeslaEntity.LengthOfNameLong.ToString(), hashEntries[13].Value);

            Assert.Equal("LengthOfNameUlong", hashEntries[14].Name);
            Assert.Equal(expectedTeslaEntity.LengthOfNameUlong.ToString(), hashEntries[14].Value);

            Assert.Equal("LengthOfNameFloat", hashEntries[15].Name);
            Assert.Equal(expectedTeslaEntity.LengthOfNameFloat.ToString(), hashEntries[15].Value);

            Assert.Equal("LengthOfNameDouble", hashEntries[16].Name);
            Assert.Equal(expectedTeslaEntity.LengthOfNameDouble.ToString(), hashEntries[16].Value);

            Assert.Equal("LengthOfNameDecimal", hashEntries[17].Name);
            Assert.Equal(expectedTeslaEntity.LengthOfNameDecimal.ToString(), hashEntries[17].Value);

            Assert.Equal("MetaData", hashEntries[18].Name);
            Assert.Equal(JsonConvert.SerializeObject(expectedTeslaEntity.MetaData, Formatting.None), hashEntries[18].Value);
            
            Assert.Equal("Id", hashEntries[19].Name);
            Assert.Equal(expectedTeslaEntity.Id.ToString(), hashEntries[19].Value);
        }


        [Fact]
        public async void HashSetHashGetAllAndConvertFromHashEntryList_HashSet_ReturnEntityWithValues()
        {
            //Arrange 
            IDatabase fakeDatabase = FakesFactory.CreateDatabaseFake();

            //Act
            StockMetaData metaData = new StockMetaData("USA", CurrencyCode.USD);
            StockEntity expectedTeslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5, metaData);
            await fakeDatabase.HashSetAsync(expectedTeslaEntity);
            HashEntry[] hashEntries = await fakeDatabase.HashGetAllAsync(expectedTeslaEntity.GetRedisKey());
            StockEntity teslaEntity = hashEntries.ConvertFromHashEntryList<StockEntity>();

            //Assert 
            Assert.Equal(expectedTeslaEntity.Id, teslaEntity.Id);
            Assert.Equal(expectedTeslaEntity.Name, teslaEntity.Name);
            Assert.Equal(expectedTeslaEntity.Symbol, teslaEntity.Symbol);
            Assert.Equal(expectedTeslaEntity.Sector, teslaEntity.Sector);
            Assert.Equal(expectedTeslaEntity.Price, teslaEntity.Price);
            Assert.Equal(expectedTeslaEntity.PriceChangeRate, teslaEntity.PriceChangeRate);
            Assert.Equal(expectedTeslaEntity.CreatedDateTime, teslaEntity.CreatedDateTime);
            Assert.Equal(expectedTeslaEntity.IsActive, teslaEntity.IsActive);
            Assert.Equal(expectedTeslaEntity.MetaData.Country, teslaEntity.MetaData.Country);
            Assert.Equal(expectedTeslaEntity.MetaData.Currency, teslaEntity.MetaData.Currency);
            Assert.Equal(expectedTeslaEntity.MetaData.UpdateDateTime, teslaEntity.MetaData.UpdateDateTime);
        }

        [Fact]
        public async void HashSetHashGetAllAndConvertFromHashEntryList_HashSet_ReturnEntityWithValuesDateTimeKindUTC()
        {
            //Arrange 
            IDatabase fakeDatabase = FakesFactory.CreateDatabaseFake();

            //Act
            StockMetaData metaData = new StockMetaData("USA", CurrencyCode.USD);
            StockEntity expectedTeslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5, metaData);
            await fakeDatabase.HashSetAsync(expectedTeslaEntity);
            HashEntry[] hashEntries = await fakeDatabase.HashGetAllAsync(expectedTeslaEntity.GetRedisKey());
            StockEntity teslaEntity = hashEntries.ConvertFromHashEntryList<StockEntity>();

            //Assert 
            Assert.Equal(expectedTeslaEntity.Id, teslaEntity.Id);
            Assert.Equal(expectedTeslaEntity.Name, teslaEntity.Name);
            Assert.Equal(expectedTeslaEntity.Symbol, teslaEntity.Symbol);
            Assert.Equal(expectedTeslaEntity.Sector, teslaEntity.Sector);
            Assert.Equal(expectedTeslaEntity.Price, teslaEntity.Price);
            Assert.Equal(expectedTeslaEntity.PriceChangeRate, teslaEntity.PriceChangeRate);
            Assert.Equal(expectedTeslaEntity.CreatedDateTime, teslaEntity.CreatedDateTime);
            Assert.Equal(expectedTeslaEntity.IsActive, teslaEntity.IsActive);
            Assert.Equal(expectedTeslaEntity.MetaData.Country, teslaEntity.MetaData.Country);
            Assert.Equal(expectedTeslaEntity.MetaData.Currency, teslaEntity.MetaData.Currency);
            Assert.Equal(expectedTeslaEntity.MetaData.UpdateDateTime, teslaEntity.MetaData.UpdateDateTime);
        }
    }
}

using StackExchange.Redis.Branch.Query;
using StackExchange.Redis.Branch.UnitTest.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StackExchange.Redis.Branch.UnitTest
{
    public class RedisQueryTest
    {
        [Fact]
        public async Task QueryBy_Boolean()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            teslaEntity.IsActive = false;
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.IsActive == true));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_Char()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.FirstLetterOfName == 'T'));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task QueryBy_Byte()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LastByteOfName == 'T'));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task QueryBy_Int16()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameShort == 5));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_UInt16()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameUshort == 5));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_Int32()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameInt == 5));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_UInt32()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameUint == 5));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_Int64()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameLong == 5));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_UInt64()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameUlong == 5));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_Single()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameFloat == 5.0));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_Double()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameDouble == 5.0));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_Decimal()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameDecimal == (decimal)5));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_String()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);
            
            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);
            
            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);
            
            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.Name == "TESLA"));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task QueryBy_DateTime()
        {
            //Arrange
            DateTime dateTime = new DateTime(2021, 1, 5, 21, 10, 23);
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            teslaEntity.CreatedDateTime = dateTime;
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            microsoftEntity.CreatedDateTime = dateTime.AddSeconds(60);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity appleEntity = new StockEntity("APPLE", StockSector.Technology, 294.21, 8.5);
            appleEntity.CreatedDateTime = dateTime.AddSeconds(120);
            await stubStockRepository.AddAsync(appleEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.CreatedDateTime < dateTime.AddSeconds(90)));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_Enum()
        {
            //Arrange
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity rheinEnergyEntity = new StockEntity("Rhein Energy", StockSector.Energy, 294.21, 8.5);
            await stubStockRepository.AddAsync(rheinEnergyEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.Sector == StockSector.Technology));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task QueryBy_Complex()
        {
            //Arrange
            DateTime dateTime = new DateTime(2021, 1, 5, 21, 10, 23);
            IConnectionMultiplexer connectionMultiplexer = FakesFactory.CreateConnectionMultiplexerFake();
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake(connectionMultiplexer);

            StockEntity teslaEntity = new StockEntity("TESLA", StockSector.Technology, 229.00, 12.5);
            teslaEntity.IsActive = false; 
            teslaEntity.CreatedDateTime = dateTime;
            await stubStockRepository.AddAsync(teslaEntity);

            StockEntity microsoftEntity = new StockEntity("MICROSOFT", StockSector.Technology, 204.00, 9.5);
            microsoftEntity.CreatedDateTime = dateTime.AddSeconds(60);
            await stubStockRepository.AddAsync(microsoftEntity);

            StockEntity rheinEnergyEntity = new StockEntity("Rhein Energy", StockSector.Energy, 294.21, 8.5);
            rheinEnergyEntity.CreatedDateTime = dateTime.AddSeconds(120);
            await stubStockRepository.AddAsync(rheinEnergyEntity);

            //Act
            QueryProvider provider = new RedisQueryProvider(connectionMultiplexer);
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s =>    (s.IsActive == false && s.FirstLetterOfName == 'T' && s.CreatedDateTime == dateTime && (s.LastByteOfName == (byte)'L' || s.LengthOfNameInt < 6))
                                                                  || (s.CreatedDateTime > dateTime.AddSeconds(90) && s.LengthOfNameUlong == (ulong)"Rhein Energy".Length));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(" {{StockEntity:IsActive-Boolean:False}}  {{StockEntity:FirstLetterOfName-Int32:84:=}} {{Intersection}} {{StockEntity:CreatedDateTime-DateTime:637454778230000000:=}} {{Intersection}} {{StockEntity:LastByteOfName-Int32:76:=}}  {{StockEntity:LengthOfNameInt-Int32:6:<}} {{Union}}{{Intersection}} {{StockEntity:CreatedDateTime-DateTime:637454779130000000:>}}  {{StockEntity:LengthOfNameUlong-UInt64:12:=}} {{Intersection}}{{Union}}", query.ToString());
        }
    }
}

using StackExchange.Redis.Branch.IntegrationTest.Fakes;
using StackExchange.Redis.Branch.IntegrationTest.Helpers;
using StackExchange.Redis.Branch.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace StackExchange.Redis.Branch.IntegrationTest
{
    [Collection("Redis")]
    [TestCaseOrderer("StackExchange.Redis.Branch.IntegrationTest.PriorityOrderer", "StackExchange.Redis.Branch.IntegrationTest")]
    public class RedisQueryTest
    {
        private readonly RedisFixture fixture;
        private static bool isArranged = false;

        public RedisQueryTest(RedisFixture fixture)
        {
            this.fixture = fixture;
            if (!isArranged)
            {
                fixture.ReloadTestDataAsync().Wait();
                isArranged = true;
            }
        }

        [Fact]
        public void QueryBy_Boolean()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.IsActive == true));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.IsActive == true)), result.Count);
        }

        [Fact]
        public void QueryBy_Char()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.FirstLetterOfName == 'A'));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.FirstLetterOfName == 'A')), result.Count);
        }

        [Fact]
        public void QueryBy_Byte()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LastByteOfName == 's'));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LastByteOfName == 's')), result.Count);
        }

        [Fact]
        public void QueryBy_Int16()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameShort == 10));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LengthOfNameShort == 10)), result.Count);
        }

        [Fact]
        public void QueryBy_UInt16()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameUshort == 10));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LengthOfNameUshort == 10)), result.Count);
        }

        [Fact]
        public void QueryBy_Int32()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameInt == 10));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LengthOfNameInt == 10)), result.Count);
        }

        [Fact]
        public void QueryBy_UInt32()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameUint == 10));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LengthOfNameUint == 10)), result.Count);
        }

        [Fact]
        public void QueryBy_Int64()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameLong == 10));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LengthOfNameLong == 10)), result.Count);
        }

        [Fact]
        public void QueryBy_UInt64()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameUlong == 10));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LengthOfNameUlong == 10)), result.Count);
        }

        [Fact]
        public void QueryBy_Single()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameFloat == 10.0));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LengthOfNameFloat == 10)), result.Count);
        }

        [Fact]
        public void QueryBy_Double()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameDouble == 10.0));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LengthOfNameDouble == 10.0)), result.Count);
        }

        [Fact]
        public void QueryBy_Decimal()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.LengthOfNameDecimal == (decimal)10));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.LengthOfNameDecimal == (decimal)10)), result.Count);
        }

        [Fact]
        public void QueryBy_String()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.Name == "Abbott Laboratories"));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.Name == "Abbott Laboratories")), result.Count);
        }

        [Fact]
        public void QueryBy_DateTime()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.CreatedDateTime < DateTime.Now.AddMilliseconds(245)));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.CreatedDateTime < DateTime.Now.AddMinutes(245))), result.Count);
        }

        [Fact]
        public void QueryBy_Enum()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.Sector == StockSector.CommunicationServices));
            List<StockEntity> result = query.ToList();

            //Assert
            Assert.Equal(fixture.TestData.Count(s => (s.Sector == StockSector.CommunicationServices)), result.Count);
        }

        [Fact]
        public void QueryBy_Complex()
        {
            //Arrange and Act
            QueryProvider provider = new RedisQueryProvider((IConnectionMultiplexer)fixture.DI.GetService(typeof(IConnectionMultiplexer)));
            RedisQuery<StockEntity> redisQuery = new RedisQuery<StockEntity>(provider);
            IQueryable<StockEntity> query = redisQuery.Where(s => (s.IsActive == true && s.FirstLetterOfName == 'A' && s.CreatedDateTime < DateTime.Now.AddMilliseconds(-1)
                                                               && (s.LastByteOfName == (byte)'.' || s.LengthOfNameInt < 12) || (s.CreatedDateTime == DateTime.Now
                                                               && s.LengthOfNameUlong == (ulong)"Accenture plc".Length)));
            List<StockEntity> result = query.ToList();
            var count = fixture.TestData.Count(s => (s.IsActive == true && s.FirstLetterOfName == 'A' && s.CreatedDateTime < DateTime.Now.AddMilliseconds(-1)
                                                 && (s.LastByteOfName == (byte)'.' || s.LengthOfNameInt < 12) || (s.CreatedDateTime == DateTime.Now 
                                                 && s.LengthOfNameUlong == (ulong)"Accenture plc".Length)));
            //Assert
            Assert.Equal(count, result.Count);
        }

    }
}

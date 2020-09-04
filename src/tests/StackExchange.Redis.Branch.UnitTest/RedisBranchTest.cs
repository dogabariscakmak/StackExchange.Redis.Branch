using StackExchange.Redis.Branch.Repository;
using StackExchange.Redis.Branch.UnitTest.Fakes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StackExchange.Redis.Branch.UnitTest
{
    public class RedisBranchTest
    {
        [Fact]
        public void GroupBy_GroupByNotExistProperty_ThrowsArgumentException()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();
            IBranch<StockEntity> technologyBranch = new RedisBranch<StockEntity>();
            technologyBranch.SetBranchId("BRANCH_NOTVALID");

            //Act
            Action act = () => technologyBranch.GroupBy("NotExist");

            //Assert 
            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal($"NotExist is not member of {typeof(StockEntity).Name}.", exception.Message);
        }

        [Fact]
        public void GroupBy_GroupByInvalidProperty_ThrowsArgumentException()
        {
            //Arrange 
            StockRepository stubStockRepository = FakesFactory.CreateStockRepositoryFake();
            IBranch<StockEntity> technologyBranch = new RedisBranch<StockEntity>();
            technologyBranch.SetBranchId("BRANCH_NOTVALID");

            //Act
            Action act = () => technologyBranch.GroupBy("MetaData");

            //Assert 
            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal($"MetaData is StockMetaData. GroupByProperty only applied on value types and string.", exception.Message);
        }
    }
}

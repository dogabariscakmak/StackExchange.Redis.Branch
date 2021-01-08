namespace StackExchange.Redis.Branch.UnitTest.Fakes
{
    public static class FakesFactory
    {
        public static StockRepository CreateStockRepositoryFake()
        {
            return new StockRepository(CreateConnectionMultiplexerFake());
        }

        public static StockRepository CreateStockRepositoryFake(IConnectionMultiplexer connectionMultiplexer)
        {
            return new StockRepository(connectionMultiplexer);
        }

        public static IConnectionMultiplexer CreateConnectionMultiplexerFake()
        {
            return new FakeConnectionMultiplexer(CreateDatabaseFake());
        }

        public static IDatabase CreateDatabaseFake()
        {
            return new FakeDatabase();
        }
    }
}

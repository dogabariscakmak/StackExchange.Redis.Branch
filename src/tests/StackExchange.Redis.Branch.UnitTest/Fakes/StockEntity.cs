using StackExchange.Redis.Branch.Entity;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace StackExchange.Redis.Branch.UnitTest.Fakes
{
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

        public StockEntity() { }

        public StockEntity(string name, StockSector sector, double price, double priceChangeRate)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Symbol = name.Substring(0, name.Length > 3 ? 3 : name.Length);
            Sector = sector;
            Price = price;
            PriceChangeRate = priceChangeRate;
            CreatedDateTime = DateTimeOffset.UtcNow.DateTime;
            IsActive = true;

            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            DummyString = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public StockEntity(string name, StockSector sector, double price, double priceChangeRate, StockMetaData metaData)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Symbol = name.Substring(0, name.Length > 3 ? 3 : name.Length);
            Sector = sector;
            Price = price;
            PriceChangeRate = priceChangeRate;
            CreatedDateTime = DateTimeOffset.UtcNow.DateTime;
            MetaData = metaData;
            IsActive = true;

            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            DummyString = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }
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

    public class StockMetaData
    {
        public string Country { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public CurrencyCode Currency { get; set; }

        public StockMetaData() { }

        public StockMetaData(string country, CurrencyCode currency)
        {
            Country = country;
            Currency = currency;
            UpdateDateTime = DateTimeOffset.UtcNow.DateTime;
        }
    }
}

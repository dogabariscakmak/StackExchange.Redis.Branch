using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Redis.Branch.IntegrationTest.Fakes
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

        public StockEntity()
        {
            CreatedDateTime = DateTimeOffset.UtcNow.DateTime;
        }

        public StockEntity(string id, string name, StockSector sector, double price, double priceChangeRate)
        {
            Random random = new Random();

            Id = id;
            Name = name;
            Symbol = name.Substring(0, name.Length > 3 ? 3 : name.Length);
            Sector = sector;
            Price = price;
            PriceChangeRate = priceChangeRate;
            CreatedDateTime = DateTimeOffset.UtcNow.DateTime.AddMinutes(random.Next(-720, 720));
            IsActive = true;

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            DummyString = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public StockEntity(string id, string name, StockSector sector, double price, double priceChangeRate, StockMetaData metaData)
        {
            Id = id;
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
        Industrials = 1,
        HealthCare = 2,
        InformationTechnology = 3,
        CommunicationServices = 4,
        ConsumerDiscretionary = 5,
        Utilities = 6,
        Financials = 7,
        Materials = 8,
        RealEstate = 9,
        ConsumerStaples = 10,
        Energy = 11
    };

    public enum ProfitLevel
    {
        None = 0,
        Great = 1,
        Normal = 2,
        Loss = 3
    };

    public enum CurrencyCode
    {
        None = 0,
        USD = 1,
        EUR = 2
    };

    public class StockMetaData
    {
        public string Country { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public CurrencyCode Currency { get; set; }

        public StockMetaData()
        {
            UpdateDateTime = DateTimeOffset.UtcNow.DateTime;
        }

        public StockMetaData(string country, CurrencyCode currency)
        {
            Country = country;
            Currency = currency;
            UpdateDateTime = DateTimeOffset.UtcNow.DateTime;
        }
    }
}

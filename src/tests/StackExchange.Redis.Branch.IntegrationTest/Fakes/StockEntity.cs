using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Redis.Branch.IntegrationTest.Fakes
{
    [RedisQueryable]
    public class StockEntity : RedisEntity
    {
        public string Name { get; set; }

        public string Symbol { get; set; }

        public StockSector Sector { get; set; }

        public double Price { get; set; }

        public double PriceChangeRate { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public bool IsActive { get; set; }

        public char FirstLetterOfName { get; set; }

        public byte LastByteOfName { get; set; }

        public short LengthOfNameShort { get; set; }

        public ushort LengthOfNameUshort { get; set; }

        public int LengthOfNameInt { get; set; }

        public uint LengthOfNameUint { get; set; }

        public long LengthOfNameLong { get; set; }

        public ulong LengthOfNameUlong { get; set; }

        public float LengthOfNameFloat { get; set; }

        public double LengthOfNameDouble { get; set; }

        public decimal LengthOfNameDecimal { get; set; }

        public StockMetaData MetaData { get; set; }

        [IgnoreDataMember]
        public string DummyString { get; set; }

        public StockEntity()
        {
            CreatedDateTime = DateTimeOffset.UtcNow.DateTime;
        }

        public StockEntity(string id, string name, StockSector sector, double price, double priceChangeRate)
        {
            Id = id;
            Name = name;
            Symbol = name.Substring(0, name.Length > 3 ? 3 : name.Length);
            Sector = sector;
            Price = price;
            PriceChangeRate = priceChangeRate;
            CreatedDateTime = DateTimeOffset.UtcNow.DateTime;
            IsActive = true;
            FirstLetterOfName = name.ToCharArray()[0];
            LastByteOfName = (byte)name.ToCharArray()[name.Length - 1];
            LengthOfNameShort = (short)name.Length;
            LengthOfNameUshort = (ushort)name.Length;
            LengthOfNameInt = name.Length;
            LengthOfNameUint = (uint)name.Length;
            LengthOfNameLong = name.Length;
            LengthOfNameUlong = (ulong)name.Length;
            LengthOfNameFloat = (float)name.Length;
            LengthOfNameDouble = (double)name.Length;
            LengthOfNameDecimal = (decimal)name.Length;

            Random random = new Random();
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
            FirstLetterOfName = name.ToCharArray()[0];
            LastByteOfName = (byte)name.ToCharArray()[name.Length - 1];
            LengthOfNameShort = (short)name.Length;
            LengthOfNameUshort = (ushort)name.Length;
            LengthOfNameInt = name.Length;
            LengthOfNameUint = (uint)name.Length;
            LengthOfNameLong = name.Length;
            LengthOfNameUlong = (ulong)name.Length;
            LengthOfNameFloat = (float)name.Length;
            LengthOfNameDouble = (double)name.Length;
            LengthOfNameDecimal = (decimal)name.Length;

            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            DummyString = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void FillCalculatedProperties()
        {
            FirstLetterOfName = Name.ToCharArray()[0];
            LastByteOfName = (byte)Name.ToCharArray()[Name.Length - 1];
            LengthOfNameShort = (short)Name.Length;
            LengthOfNameUshort = (ushort)Name.Length;
            LengthOfNameInt = Name.Length;
            LengthOfNameUint = (uint)Name.Length;
            LengthOfNameLong = Name.Length;
            LengthOfNameUlong = (ulong)Name.Length;
            LengthOfNameFloat = (float)Name.Length;
            LengthOfNameDouble = (double)Name.Length;
            LengthOfNameDecimal = (decimal)Name.Length;
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

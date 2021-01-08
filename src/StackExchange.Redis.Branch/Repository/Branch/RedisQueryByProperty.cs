using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StackExchange.Redis.Branch.Repository
{
    /// <summary>
    /// Redis Query. It creates branches for each property in Redis Entity. For now, supported types: string, double, bool, DateTime.
    /// If property is double or DateTime, it is stored in sorted set created. double value is used as score. For DateTime, first, it is converted to UTC and Ticks value is used as score.
    /// If property is string or bool, it is stored in set. For each value a new Redis Key created and belonging ids are stored.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    internal class RedisQueryByProperty<T> : IQuery<T> where T : RedisEntity, new()
    {
        private string _propertyName { get; set; }
        private BranchRedisKey _redisKey { get; set; }

        public RedisQueryByProperty(string propertyName)
        {
            if (!typeof(T).GetProperties().Any(x => x.Name == propertyName))
            {
                throw new ArgumentException($"{propertyName} is not member of {typeof(T).Name}.");
            }

            _propertyName = propertyName;
        }

        public BranchRedisKey GetKey(T entity)
        {
            PropertyInfo property = entity.GetType().GetProperty(_propertyName);
            TypeCode typeCode = Type.GetTypeCode(property.PropertyType);

            switch (typeCode)
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    return new BranchRedisKey(BranchRedisKeyEnum.Query, $"properties:{property.Name}");
                case TypeCode.Boolean:
                case TypeCode.String:
                    return new BranchRedisKey(BranchRedisKeyEnum.Query, $"properties:{property.Name}:{Convert.ToString(property.GetValue(entity))}");
                default:
                    throw new NotSupportedException(string.Format("The type for '{0}' is not supported", typeCode));
            }
        }

        public double GetScore(T entity)
        {
            double score;

            PropertyInfo property = entity.GetType().GetProperty(_propertyName);
            TypeCode typeCode = Type.GetTypeCode(property.PropertyType);
            object propertyValue = property.GetValue(entity);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    if(property.PropertyType.IsEnum)
                    {
                        propertyValue = (int)propertyValue;
                    }

                    if (double.TryParse(propertyValue.ToString(), out score))
                    {
                        return score;
                    }
                    return 0;
                case TypeCode.Char:
                    return (char)propertyValue;
                case TypeCode.DateTime: 
                    return DateTime.SpecifyKind((DateTime)propertyValue, DateTimeKind.Utc).Ticks;
                case TypeCode.String:
                    return 0;
                default:
                    return 0;
            }
        }
    }
}

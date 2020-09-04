using StackExchange.Redis.Branch.Entity;
using System;
using System.Linq;
using System.Reflection;

namespace StackExchange.Redis.Branch.Repository
{
    /// <summary>
    /// Redis Group. It groups entities by a property. Redis Key is property name.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    internal class RedisGroupByProperty<T> : IGroup<T> where T : RedisEntity, new()
    {
        private string _propertyName { get; set; }

        /// <summary>
        /// GroupBy applied on property. Choosing string property ended with so  many keys on redis. When choose a string property use it with your own risk.
        /// </summary>
        /// <param name="propertyName">Property Name which applied GroupBy. Property must be a value type or string.</param>
        public RedisGroupByProperty(string propertyName)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperties().FirstOrDefault(x => x.Name == propertyName);
            if (propertyInfo == default)
            {
                throw new ArgumentException($"{propertyName} is not member of {typeof(T).Name}.");
            }

            if (!propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType != typeof(string))
            {
                throw new ArgumentException($"{propertyName} is {propertyInfo.PropertyType.Name}. GroupByProperty only applied on value types and string.");
            }

            _propertyName = propertyName;
        }

        public BranchRedisKey GetKey(T entity)
        {
            object propertyValue = entity.GetType().GetProperty(_propertyName).GetValue(entity);
            BranchRedisKey redisKey;
            if (_propertyName == "Id")
            {
                redisKey = new BranchRedisKey(BranchRedisKeyEnum.Data, propertyValue.ToString());
            }
            else
            {
                redisKey = new BranchRedisKey(BranchRedisKeyEnum.Group, _propertyName, propertyValue.ToString());
            }

            redisKey.SetValue(propertyValue.ToString());
            return redisKey;
        }

        public BranchRedisKey GetKey()
        {
            BranchRedisKey redisKey;
            if (_propertyName == "Id")
            {
                redisKey = new BranchRedisKey(BranchRedisKeyEnum.Data, "{propertyValue}");
            }
            else
            {
                redisKey = new BranchRedisKey(BranchRedisKeyEnum.Group, _propertyName, "{propertyValue}");
            }

            return redisKey;
        }

        public BranchRedisKey GetKey(string propertyValue)
        {
            BranchRedisKey redisKey;
            if (_propertyName == "Id")
            {
                redisKey = new BranchRedisKey(BranchRedisKeyEnum.Data, propertyValue);
            }
            else
            {
                redisKey = new BranchRedisKey(BranchRedisKeyEnum.Group, _propertyName, propertyValue);
            }

            return redisKey;
        }
    }
}

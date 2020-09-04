using StackExchange.Redis.Branch.Entity;
using System;
using System.Linq;

namespace StackExchange.Redis.Branch.Repository
{
    /// <summary>
    /// Redis Sort. It sorts entities by a property. Redis Key is property name. Sort is the last element of the branch and at most one sort can be in a branch.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    internal class RedisSortByProperty<T> : ISort<T> where T : RedisEntity, new()
    {
        private string _propertyName { get; set; }
        private BranchRedisKey _redisKey { get; set; }

        public RedisSortByProperty(string propertyName)
        {
            if (!typeof(T).GetProperties().Any(x => x.Name == propertyName))
            {
                throw new ArgumentException($"{propertyName} is not member of {typeof(T).Name}.");
            }

            _propertyName = propertyName;
        }

        public BranchRedisKey GetKey()
        {
            if (_redisKey == default)
            {
                _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Sort, _propertyName);
            }
            return _redisKey;
        }

        public double GetScore(T entity)
        {
            object propertyValue = entity.GetType().GetProperty(_propertyName).GetValue(entity);
            double score;
            if (double.TryParse(propertyValue.ToString(), out score))
            {
                return score;
            }
            throw new ArgumentException($"{_propertyName} with value of {propertyValue} can not be parsed to double.", _propertyName);
        }
    }
}

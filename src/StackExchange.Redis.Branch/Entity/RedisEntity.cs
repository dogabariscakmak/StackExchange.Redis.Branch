using System.Runtime.Serialization;

namespace StackExchange.Redis.Branch.Entity
{
    /// <summary>
    /// Base class for redis entity.
    /// </summary>
    public abstract class RedisEntity
    {
        public string Id { get; set; }

        [IgnoreDataMember]
        private IBranchRedisKey _redisKey { get; set; }


        public RedisEntity()
        {
        }

        public virtual string GetRedisKey()
        {
            if (_redisKey == default)
            {
                _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Data, Id);
            }

            return $"{this.GetType().Name}:{_redisKey}";
        }
    }
}

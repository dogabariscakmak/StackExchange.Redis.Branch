using StackExchange.Redis.Branch.Entity;

namespace StackExchange.Redis.Branch.Repository
{
    internal interface IGroup<T> where T : RedisEntity, new()
    {
        BranchRedisKey GetKey();
        BranchRedisKey GetKey(T entity);
        BranchRedisKey GetKey(string propertyValue);
    }
}

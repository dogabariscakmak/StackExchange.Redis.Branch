using StackExchange.Redis.Branch.Entity;

namespace StackExchange.Redis.Branch.Repository
{
    internal interface ISort<T> where T : RedisEntity, new()
    {
        double GetScore(T entity);
        BranchRedisKey GetKey();
    }
}

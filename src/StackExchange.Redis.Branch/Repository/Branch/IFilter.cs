using StackExchange.Redis.Branch.Entity;

namespace StackExchange.Redis.Branch.Repository
{
    internal interface IFilter<T> where T : RedisEntity, new()
    {
        bool Invoke(T entity);
    }
}

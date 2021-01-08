using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Repository
{
    internal interface IQuery<T> where T : RedisEntity, new()
    {
        double GetScore(T entity);
        BranchRedisKey GetKey(T entity);
    }
}

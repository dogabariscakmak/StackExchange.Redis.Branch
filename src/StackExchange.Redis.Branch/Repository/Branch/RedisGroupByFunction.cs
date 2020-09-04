using StackExchange.Redis.Branch.Entity;
using System;
using System.Linq.Expressions;

namespace StackExchange.Redis.Branch.Repository
{
    /// <summary>
    /// Redis Group. It groups entities by a function. Redis Key is function name.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    internal class RedisGroupByFunction<T> : IGroup<T> where T : RedisEntity, new()
    {
        private Expression<Func<T, string>> _groupFunction { get; set; }

        private string _functionName { get; set; }

        public RedisGroupByFunction(string functionName, Expression<Func<T, string>> groupFunction)
        {
            _groupFunction = groupFunction;
            _functionName = functionName;
        }

        public RedisGroupByFunction(Expression<Func<T, string>> groupFunction)
        {
            _groupFunction = groupFunction;
        }

        private string GetFunctionGroupKey(T entity)
        {
            var f = _groupFunction.Compile();
            return f.Invoke(entity);
        }

        public BranchRedisKey GetKey(T entity)
        {
            return new BranchRedisKey(BranchRedisKeyEnum.Group, $"{_functionName}:{GetFunctionGroupKey(entity)}");
        }

        public BranchRedisKey GetKey()
        {
            return new BranchRedisKey(BranchRedisKeyEnum.Group, $"{_functionName}:PropertyValue");
        }

        public BranchRedisKey GetKey(string functionReturnValue)
        {
            return new BranchRedisKey(BranchRedisKeyEnum.Group, $"{_functionName}:{functionReturnValue}");
        }
    }
}

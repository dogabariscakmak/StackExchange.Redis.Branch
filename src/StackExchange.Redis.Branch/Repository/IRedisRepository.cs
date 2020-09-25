using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackExchange.Redis.Branch.Repository
{
    public interface IRedisRepository<T> where T : RedisEntity, new()
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteAsync(string id);
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAsync(string branchId, params string[] groups);
        Task<IEnumerable<T>> GetAsync(string branchId, double from, params string[] groups);
        Task<IEnumerable<T>> GetAsync(string branchId, double from, double to, params string[] groups);
        Task<IEnumerable<T>> GetAsync(string branchId, double from, double to, long skip, long take, params string[] groups);
        Task<long> CountAsync(string branchId, params string[] groups);
        Task<long> CountAsync(string branchId, double from, params string[] groups);
        Task<long> CountAsync(string branchId, double from, double to, params string[] groups);
        void CreateBranches();
        void AddBranch(IBranch<T> branch);
        IEnumerable<string> GetBranches();
    }
}

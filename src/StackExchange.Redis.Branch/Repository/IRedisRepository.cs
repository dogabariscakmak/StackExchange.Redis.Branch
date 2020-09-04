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
        Task<IEnumerable<T>> GetByBranchAsync(string branchId, params string[] groups);
        Task<IEnumerable<T>> GetBySortedBranchAsync(string branchId, params string[] groups);
        Task<IEnumerable<T>> GetBySortedBranchAsync(string branchId, long from, params string[] groups);
        Task<IEnumerable<T>> GetBySortedBranchAsync(string branchId, long from, long to, params string[] groups);
        Task<IEnumerable<T>> GetBySortedBranchAsync(string branchId, long from, long to, long skip, long take, params string[] groups);
        Task<long> CountByBranchAsync(string branchId, params string[] groups);
        Task<long> CountBySortedBranchAsync(string branchId, params string[] groups);
        Task<long> CountBySortedBranchAsync(string branchId, long from, params string[] groups);
        Task<long> CountBySortedBranchAsync(string branchId, long from, long to, params string[] groups);
        void CreateBranches();
        void AddBranch(IBranch<T> branch);
        Task<bool> SetKeyExpireAsync(string id, TimeSpan timeSpan);
        Task<bool> SetKeyExpireAsync(T entity, TimeSpan timeSpan);
        IEnumerable<string> GetBranches();
    }
}

using StackExchange.Redis.Branch.Entity;
using System;
using System.Linq.Expressions;

namespace StackExchange.Redis.Branch.Repository
{
    public interface IBranch<T> where T : RedisEntity, new()
    {
        /// <summary>
        /// Gets BranchId.
        /// </summary>
        /// <returns>BranchId string</returns>
        string GetBranchId();
        
        /// <summary>
        /// Sets BranchId. BranchId should be unique within scope of repository.
        /// </summary>
        /// <param name="branchId">BranchId string</param>
        void SetBranchId(string branchId);
        
        /// <summary>
        /// Gets Entity Type of Branch.
        /// </summary>
        /// <returns>Type of entity for branch</returns>
        Type GetEntityType();
        
        /// <summary>
        /// Adds Filter Function to Branch pipeline. Function gets a RedisEntity as parameter and returns bool value. 
        /// Each function must return true to add an entity to branch. A Redis Branch can contain multiple filters.
        /// </summary>
        /// <param name="filterFunction">Function expression. RedisEntity as parameter and returns bool.</param>
        /// <returns>Branch to chain as fluent api</returns>
        IBranch<T> FilterBy(Expression<Func<T, bool>> filterFunction);

        /// <summary>
        /// Groups by Id property of RedisEntity. A Redis Branch can contain multiple group by.
        /// </summary>
        /// <returns>Branch to chain as fluent api</returns>
        IBranch<T> GroupBy();

        /// <summary>
        /// Groups by stated property by name. Groups are sets in Redis. Each unique value for property produces new set in Redis. 
        /// These sets store corresponding entity ids. A Redis Branch can contain multiple group by.
        /// </summary>
        /// <param name="propName">Property Name as string</param>
        /// <returns>Branch to chain as fluent api</returns>
        IBranch<T> GroupBy(string propName);

        /// <summary>
        /// Groups by function. Groups are sets in Redis. Combination of function name and group function return value produces redis key for set. 
        /// These sets store corresponding entity ids. A Redis Branch can contain multiple group by.
        /// </summary>
        /// <param name="functionName">Name of function as string</param>
        /// <param name="groupFunction">Function expression. RedisEntity as parameter and returns string as result.</param>
        /// <returns>Branch to chain as fluent api.</returns>
        IBranch<T> GroupBy(string functionName, Expression<Func<T, string>> groupFunction);

        /// <summary>
        /// Sorts by Id property of RedisEntity. A Redis Branch can contain at most one sort by.
        /// </summary>
        void SortBy();

        /// <summary>
        /// Sorts by stated property by name. Sorts are sorted sets in Redis. Stated property must be parsable to double.
        /// These sorted sets store corresponding entity ids with scores coming from property. A Redis Branch can contain at most one sort by.
        /// </summary>
        /// <param name="propName">Property Name as string</param>
        void SortBy(string propName);

        /// <summary>
        /// Sorts by function. Sorts are sorted sets in Redis. Function name is redis key and sort function return value is used as score in sorted set. 
        /// These sorted sets store corresponding entity ids with scores coming from function. A Redis Branch can contain at most one sort by.
        /// </summary>
        /// <param name="functionName">Name of function as string</param>
        /// <param name="sortFunction">Function expression. RedisEntity as parameter and returns double as score.</param>
        void SortBy(string functionName, Expression<Func<T, double>> sortFunction);

        /// <summary>
        /// Get branch key. In other words, redis key for branch.
        /// </summary>
        /// <returns>Branch Key with placeholders</returns>
        string GetBranchKey();

        /// <summary>
        /// Get branch key. In other words, redis key for branch.
        /// </summary>
        /// <returns>Branch Key with placeholders if values are needed</returns>
        string GetBranchKey(T entity);

        /// <summary>
        /// Get branch key. In other words, redis key for branch.
        /// </summary>
        /// <returns>Branch Key with actual values</returns>
        string GetBranchKey(params string[] values);
    }

    internal interface IBranchInternal<T> where T : RedisEntity, new()
    {
        bool IsSortable();
        bool IsQueryable();
        double GetScore(T entity);
        bool ApplyFilters(T entity);
        void QueryBy(string propName);
    }
}

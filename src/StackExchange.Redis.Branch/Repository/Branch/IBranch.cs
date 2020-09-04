using StackExchange.Redis.Branch.Entity;
using System;
using System.Linq.Expressions;

namespace StackExchange.Redis.Branch.Repository
{
    public interface IBranch<T> where T : RedisEntity, new()
    {
        string GetBranchId();
        void SetBranchId(string branchId);
        Type GetEntityType();
        bool IsSortable();
        double GetScore(T entity);
        IBranch<T> FilterBy(Expression<Func<T, bool>> filterFunction);
        IBranch<T> GroupBy();
        IBranch<T> GroupBy(string propName);
        IBranch<T> GroupBy(string functionName, Expression<Func<T, string>> groupFunction);
        void SortBy();
        void SortBy(string propName);
        void SortBy(string functionName, Expression<Func<T, double>> sortFunction);
        string GetBranchKey();
        string GetBranchKey(T entity);
        string GetBranchKey(params string[] values);
        bool ApplyFilters(T entity);
    }
}

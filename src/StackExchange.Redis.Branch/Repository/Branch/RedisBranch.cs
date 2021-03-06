﻿using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StackExchange.Redis.Branch.Repository
{
    /// <summary>
    /// Redis Branch. There are two kind of Redis Branch: One can contain filters, groups and a sort. Other can contain only query.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    public class RedisBranch<T> : IBranch<T>, IBranchInternal<T> where T : RedisEntity, new()
    {
        internal List<IFilter<T>> Filters { get; private set; }
        internal List<IGroup<T>> Groups { get; private set; }
        internal ISort<T> Sort { get; private set; }
        internal IQuery<T> Query { get; private set; }

        private string _branchId { get; set; }
        private Type _entityType { get; set; }

        public RedisBranch()
        {
            Filters = new List<IFilter<T>>();
            Groups = new List<IGroup<T>>();
            _entityType = typeof(T);
        }

        public string GetBranchId()
        {
            return _branchId;
        }

        public void SetBranchId(string branchId)
        {
            _branchId = branchId;
        }

        public IBranch<T> GroupBy()
        {
            Groups.Add(new RedisGroupByProperty<T>("Id"));
            return this;
        }

        public IBranch<T> GroupBy(string propName)
        {
            Groups.Add(new RedisGroupByProperty<T>(propName));
            return this;
        }

        public IBranch<T> GroupBy(string functionName, Expression<Func<T, string>> groupFunction)
        {
            Groups.Add(new RedisGroupByFunction<T>(functionName, groupFunction));
            return this;
        }

        public void SortBy()
        {
            Sort = new RedisSortByProperty<T>("Id");
        }

        public void SortBy(string propName)
        {
            Sort = new RedisSortByProperty<T>(propName);
        }

        public void SortBy(string functionName, Expression<Func<T, double>> sortFunction)
        {
            Sort = new RedisSortByFunction<T>(functionName, sortFunction);
        }

        public string GetBranchKey(T entity)
        {
            string branchKey = entity.GetType().Name;
            foreach (IGroup<T> group in Groups)
            {
                branchKey = $"{branchKey}:{group.GetKey(entity)}";
            }

            if (Sort != default)
            {
                branchKey = $"{branchKey}:{Sort.GetKey()}";
            }

            if (Query != default)
            {
                branchKey = $"{branchKey}:{Query.GetKey(entity)}";
            }

            return branchKey;
        }

        public string GetBranchKey(string[] values)
        {
            string branchKey = _entityType.Name;

            if (values.Length != Groups.Count())
            {
                throw new IndexOutOfRangeException($"Parameters are not equal to branch group parameters. Branch group parameters: {Groups.Count()}, Parameters: {values.Length}");
            }

            var valueIndex = 0;
            foreach (var group in Groups)
            {
                branchKey = $"{branchKey}:{group.GetKey(values[valueIndex])}";
                valueIndex++;
            }

            if (Sort != default)
            {
                branchKey += $":{Sort.GetKey()}";
            }

            return branchKey;
        }

        public string GetBranchKey()
        {
            string branchKey = _entityType.Name;
            foreach (IGroup<T> group in Groups)
            {
                branchKey = $"{branchKey}:{group.GetKey()}";
            }

            if (Sort != default)
            {
                branchKey = $"{branchKey}:{Sort.GetKey()}";
            }

            return branchKey;
        }

        public Type GetEntityType()
        {
            return _entityType;
        }

        public IBranch<T> FilterBy(Expression<Func<T, bool>> filterFunction)
        {
            Filters.Add(new RedisFilter<T>(filterFunction));
            return this;
        }

        bool IBranchInternal<T>.IsSortable()
        {
            return Sort != default;
        }

        bool IBranchInternal<T>.IsQueryable()
        {
            return Query != default;
        }

        double IBranchInternal<T>.GetScore(T entity)
        {
            if (((IBranchInternal<T>)this).IsSortable())
            {
                return Sort.GetScore(entity);
            }
            else if (((IBranchInternal<T>)this).IsQueryable())
            {
                return Query.GetScore(entity);
            }
            else
            {
                throw new InvalidOperationException($"{GetBranchId()} is not a sortable branch.");
            }
        }

        bool IBranchInternal<T>.ApplyFilters(T entity)
        {
            bool isFilterPass = true;
            foreach (IFilter<T> filter in Filters)
            {
                isFilterPass = filter.Invoke(entity);
                if (!isFilterPass)
                {
                    break;
                }
            }
            return isFilterPass;
        }

        void IBranchInternal<T>.QueryBy(string propName)
        {
            Query = new RedisQueryByProperty<T>(propName);
        }

    }
}

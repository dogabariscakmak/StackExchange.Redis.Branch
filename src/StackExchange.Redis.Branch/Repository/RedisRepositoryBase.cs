using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackExchange.Redis.Branch.Repository
{
    /// <summary>
    /// Base abstract class for redis repositories. Any overriden function must call base function. Otherwise unexpected behavior may be happened.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RedisRepositoryBase<T> : IRedisRepository<T> where T : RedisEntity, new()
    {
        public static string BRANCH_DATA = "BRANCH_DATA";

        protected readonly IConnectionMultiplexer _redisConnectionMultiplexer;
        protected readonly IDatabase _redisDatabase;
        private List<IBranch<T>> _branches;
        protected IReadOnlyCollection<IBranch<T>> Branches => _branches?.AsReadOnly();

        public RedisRepositoryBase(IConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisConnectionMultiplexer = redisConnectionMultiplexer;
            _redisDatabase = _redisConnectionMultiplexer.GetDatabase();

            _branches = new List<IBranch<T>>();
            IBranch<T> dataBranch = new RedisBranch<T>();
            dataBranch.SetBranchId(BRANCH_DATA);
            _branches.Add(dataBranch.GroupBy());

            CreateBranches();
        }

        /// <summary>
        /// Adds entity to redis and updates branches.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task AddAsync(T entity)
        {
            await _redisDatabase.HashSetAsync(entity).ConfigureAwait(false);
            await UpdateBranchesAsync(entity, RedisEntityStateEnum.Added).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates entity to redis and updates branches.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(T entity)
        {
            await _redisDatabase.HashSetAsync(entity).ConfigureAwait(false);
            await UpdateBranchesAsync(entity, RedisEntityStateEnum.Updated).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes entity and updates branches.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T entity)
        {
            bool result = await _redisDatabase.KeyDeleteAsync(entity.GetRedisKey()).ConfigureAwait(false);
            await UpdateBranchesAsync(entity, RedisEntityStateEnum.Deleted).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Deletes entity and updates branches.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(string id)
        {
            T entity = await GetByIdAsync(id).ConfigureAwait(false);
            bool result = false;
            if (entity != default)
            {
                result = await _redisDatabase.KeyDeleteAsync(entity.GetRedisKey()).ConfigureAwait(false);
                await UpdateBranchesAsync(entity, RedisEntityStateEnum.Deleted).ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Get all entity in branch by branchId.
        /// </summary>
        /// <param name="branchId">Id to identify branch.</param>
        /// <param name="groups">Groups parameters.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetByBranchAsync(string branchId, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            string branchKey = branch.GetBranchKey(groups);

            List<T> resultList = new List<T>();

            RedisValue[] ids = await _redisDatabase.SetMembersAsync(branchKey).ConfigureAwait(false);
            foreach (RedisValue id in ids)
            {
                resultList.Add((await _redisDatabase.HashGetAllAsync($"{typeof(T).Name}:data:{id}").ConfigureAwait(false)).ConvertFromHashEntryList<T>());
            }
            return resultList;
        }

        /// <summary>
        /// Get entity by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(string id)
        {
            IBranch<T> dataBranch = _branches.Find(i => i.GetBranchId() == BRANCH_DATA);
            if (dataBranch == default)
            {
                throw new KeyNotFoundException("Data Branch not found.");
            }
            string redisKey = dataBranch.GetBranchKey().Replace("{propertyValue}", id);
            HashEntry[] hashSet = (await _redisDatabase.HashGetAllAsync(redisKey).ConfigureAwait(false));
            return hashSet.Length == 0 ? null : hashSet.ConvertFromHashEntryList<T>();
        }

        /// <summary>
        /// Get all entity in sorted branch by branchId.
        /// </summary>
        /// <param name="branchId">Id to identify branch.</param>
        /// <param name="groups">Groups parameters.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetBySortedBranchAsync(string branchId, params string[] groups)
        {
            return await GetBySortedBranchAsync(branchId, long.MinValue, long.MaxValue, groups).ConfigureAwait(false);
        }

        /// <summary>
        /// Get all entity in sorted branch by branchId.
        /// </summary>
        /// <param name="branchId">Id to identify branch.</param>
        /// <param name="from">Score from.</param>
        /// <param name="groups">Groups parameters.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetBySortedBranchAsync(string branchId, long from, params string[] groups)
        {
            return await GetBySortedBranchAsync(branchId, from, long.MaxValue, groups);
        }

        /// <summary>
        /// Get all entity in sorted branch by branchId.
        /// </summary>
        /// <param name="branchId">Id to identify branch.</param>
        /// <param name="from">Score from.</param>
        /// <param name="to">Score to.</param>
        /// <param name="groups">Groups parameters.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetBySortedBranchAsync(string branchId, long from, long to, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            string branchKey = branch.GetBranchKey(groups);
            List<T> resultList = new List<T>();
            RedisValue[] ids = await _redisDatabase.SortedSetRangeByScoreAsync(branchKey, from, to).ConfigureAwait(false);
            foreach (RedisValue id in ids)
            {
                resultList.Add((await _redisDatabase.HashGetAllAsync($"{typeof(T).Name}:data:{id}").ConfigureAwait(false)).ConvertFromHashEntryList<T>());
            }
            return resultList;
        }

        /// <summary>
        /// Get all entity in sorted branch by branchId.
        /// </summary>
        /// <param name="branchId">Id to identify branch.</param>
        /// <param name="from">Score from.</param>
        /// <param name="to">Score to.</param>
        /// <param name="skip">skip parameters.</param>
        /// <param name="take">take parameters.</param>
        /// <param name="groups">Groups parameters.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetBySortedBranchAsync(string branchId, long from, long to, long skip, long take, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            string branchKey = branch.GetBranchKey(groups);
            List<T> resultList = new List<T>();
            RedisValue[] ids = await _redisDatabase.SortedSetRangeByScoreAsync(branchKey, from, to, Exclude.None, Order.Ascending, skip, take).ConfigureAwait(false);
            foreach (RedisValue id in ids)
            {
                resultList.Add((await _redisDatabase.HashGetAllAsync($"{typeof(T).Name}:data:{id}").ConfigureAwait(false)).ConvertFromHashEntryList<T>());
            }
            return resultList;
        }

        /// <summary>
        /// Get entity counts in the branch.
        /// </summary>
        /// <param name="branchId">Id to identify branch.</param>
        /// <param name="groups">Groups parameters.</param>
        /// <returns></returns>
        public async Task<long> CountByBranchAsync(string branchId, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            string branchKey = branch.GetBranchKey(groups);

            return await _redisDatabase.SetLengthAsync(branchKey).ConfigureAwait(false);
        }

        /// <summary>
        /// Get entity counts in the branch.
        /// </summary>
        /// <param name="branchId">Id to identify branch.</param>
        /// <param name="groups">Groups parameters.</param>
        /// <returns></returns>
        public async Task<long> CountBySortedBranchAsync(string branchId, params string[] groups)
        {
            return await CountBySortedBranchAsync(branchId, long.MinValue, long.MaxValue, groups).ConfigureAwait(false);
        }

        /// <summary>
        /// Get entity counts in the branch.
        /// </summary>
        /// <param name="branchId">Id to identify branch.</param>
        /// <param name="from">Score from.</param>
        /// <param name="groups">Groups parameters.</param>
        /// <returns></returns>
        public async Task<long> CountBySortedBranchAsync(string branchId, long from, params string[] groups)
        {
            return await CountBySortedBranchAsync(branchId, from, long.MaxValue, groups).ConfigureAwait(false);
        }

        /// <summary>
        /// Get entity counts in the branch.
        /// </summary>
        /// <param name="branchId">Id to identify branch.</param>
        /// <param name="from">Score from.</param>
        /// <param name="to">Score to.</param>
        /// <param name="groups">Groups parameters.</param>
        /// <returns></returns>
        public async Task<long> CountBySortedBranchAsync(string branchId, long from, long to, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            var branchKey = branch.GetBranchKey(groups);

            return await _redisDatabase.SortedSetLengthAsync(branchKey, from, to).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds branch.
        /// </summary>
        /// <param name="branch"></param>
        public void AddBranch(IBranch<T> branch)
        {
            if (string.IsNullOrEmpty(branch.GetBranchId()))
            {
                throw new ArgumentException($"BranchId must be set. branch.SetBranchId(string branchId)");
            }
            if (branch.GetEntityType() == default)
            {
                throw new ArgumentException($"EntityType must be set. branch.SetEntityType(RedisEntity entity);");
            }
            _branches.Add(branch);
        }

        /// <summary>
        /// Updates branches according to event type and entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        private async Task UpdateBranchesAsync(T entity, RedisEntityStateEnum entityState)
        {
            switch (entityState)
            {
                case RedisEntityStateEnum.Added:
                case RedisEntityStateEnum.Updated:
                    foreach (var branch in _branches.Where(b => b.GetBranchId() != BRANCH_DATA))
                    {
                        string key = branch.GetBranchKey(entity);
                        if (branch.ApplyFilters(entity))
                        {
                            if (branch.IsSortable())
                            {
                                await _redisDatabase.SortedSetAddAsync(key, entity.Id, branch.GetScore(entity)).ConfigureAwait(false);
                            }
                            else
                            {
                                await _redisDatabase.SetAddAsync(key, entity.Id).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            if (branch.IsSortable())
                            {
                                await _redisDatabase.SortedSetRemoveAsync(key, entity.Id).ConfigureAwait(false);
                            }
                            else
                            {
                                await _redisDatabase.SetRemoveAsync(key, entity.Id).ConfigureAwait(false);
                            }
                        }
                    }
                    break;
                case RedisEntityStateEnum.Deleted:
                    foreach (var branch in _branches.Where(b => b.GetBranchId() != BRANCH_DATA))
                    {
                        string key = branch.GetBranchKey(entity);
                        if (branch.IsSortable())
                        {
                            await _redisDatabase.SortedSetRemoveAsync(key, entity.Id).ConfigureAwait(false);
                        }
                        else
                        {
                            await _redisDatabase.SetRemoveAsync(key, entity.Id).ConfigureAwait(false);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Creates branches in this method.
        /// </summary>
        public abstract void CreateBranches();

        /// <summary>
        /// Set key expire for specified entity definde by id.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public virtual async Task<bool> SetKeyExpireAsync(string id, TimeSpan timeSpan)
        {
            T entity = await GetByIdAsync(id);
            bool result = false;
            if (entity != default)
            {
                result = await _redisDatabase.KeyExpireAsync(entity.GetRedisKey(), timeSpan).ConfigureAwait(false);
                foreach (var branch in _branches.Where(b => b.GetBranchId() != BRANCH_DATA))
                {
                    string key = branch.GetBranchKey(entity);
                    if (branch.IsSortable())
                    {
                        result = await _redisDatabase.SortedSetRemoveAsync(key, entity.Id).ConfigureAwait(false);
                    }
                    else
                    {
                        result = await _redisDatabase.SetRemoveAsync(key, entity.Id).ConfigureAwait(false);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Set key expire for specified entity.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public virtual async Task<bool> SetKeyExpireAsync(T entity, TimeSpan timeSpan)
        {
            bool result = await _redisDatabase.KeyExpireAsync(entity.GetRedisKey(), timeSpan).ConfigureAwait(false);
            foreach (var branch in _branches.Where(b => b.GetBranchId() != BRANCH_DATA))
            {
                string key = branch.GetBranchKey(entity);
                if (branch.IsSortable())
                {
                    result = await _redisDatabase.SortedSetRemoveAsync(key, entity.Id).ConfigureAwait(false);
                }
                else
                {
                    result = await _redisDatabase.SetRemoveAsync(key, entity.Id).ConfigureAwait(false);
                }
            }
            return result;
        }

        public IEnumerable<string> GetBranches()
        {
            foreach (IBranch<T> branch in Branches)
            {
                yield return branch.GetBranchKey();
            }
        }
    }
}

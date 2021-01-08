using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StackExchange.Redis.Branch.Repository
{
    /// <summary>
    /// Base abstract class for redis repositories. Any overriden function must call base function. Otherwise unexpected behavior may be happened.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    ///     If any derived class from RedisRepositoryBase added to DI as Singleton, 
    ///     making it thread-safe is the developer's responsibility.
    /// </remarks> 
    public abstract class RedisRepositoryBase<T> : IRedisRepository<T> where T : RedisEntity, new()
    {
        public static string BRANCH_DATA = "BRANCH_DATA";
        public static string BRANCH_QUERY_PREFIX = "BRANCH_QUERY";

        protected readonly IConnectionMultiplexer _redisConnectionMultiplexer;
        protected readonly IDatabase _redisDatabase;
        private List<IBranch<T>> _branches;
        protected IReadOnlyCollection<IBranch<T>> Branches => _branches?.AsReadOnly();

        /// <summary>
        /// Since RedisRepository is dependent on StackExchange.Redis ConnectionMultiplexer must be provided.
        /// </summary>
        /// <param name="redisConnectionMultiplexer">IConnectionMultiplexer from StackExchange.Redis</param>
        public RedisRepositoryBase(IConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisConnectionMultiplexer = redisConnectionMultiplexer;
            _redisDatabase = _redisConnectionMultiplexer.GetDatabase();

            _branches = new List<IBranch<T>>();
            IBranch<T> dataBranch = new RedisBranch<T>();
            dataBranch.SetBranchId(BRANCH_DATA);
            _branches.Add(dataBranch.GroupBy());

            CreateBranches();
            CreateQueryBranches();
        }

        /// <summary>
        /// Creates branches for each property. These branches are used by query providers. For now, supported types: string, double, bool, DateTime
        /// </summary>
        private void CreateQueryBranches()
        {
            Type[] validPropertyTypes = { typeof(bool), typeof(char), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
                                          typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(DateTime), typeof(string),
                                          typeof(Enum)};
            if (Attribute.IsDefined(typeof(T), typeof(RedisQueryableAttribute)))
            {
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    IBranch<T> queryBranch = new RedisBranch<T>();
                    queryBranch.SetBranchId($"{BRANCH_QUERY_PREFIX}_{property.Name}");
                    if ((validPropertyTypes.Contains(property.PropertyType) || property.PropertyType.IsEnum)
                        && !property.IsDefined(typeof(IgnoreDataMemberAttribute), true)
                        && property.Name != "Id")
                    {
                        ((IBranchInternal<T>)queryBranch).QueryBy(property.Name);
                        AddBranch(queryBranch);
                    }
                }
            }
        }

        /// <summary>
        /// Private helper method to get sorted branch items.
        /// </summary>
        /// <param name="branch">Sorted Branch to query</param>
        /// <param name="from">Elements have score from</param>
        /// <param name="to">Elements have score to</param>
        /// <param name="skip">Skip elements</param>
        /// <param name="take">Take elements</param>
        /// <param name="groups">Groups values to be filtered</param>
        /// <returns>Filtered items</returns>
        private async Task<IEnumerable<T>> GetSortedAsync(IBranch<T> branch, double from, double to, long skip, long take, params string[] groups)
        {
            List<T> resultList = new List<T>();
            RedisValue[] ids = await _redisDatabase.SortedSetRangeByScoreAsync(branch.GetBranchKey(groups), from, to, Exclude.None, Order.Ascending, skip, take).ConfigureAwait(false);
            foreach (RedisValue id in ids)
            {
                resultList.Add((await _redisDatabase.HashGetAllAsync($"{typeof(T).Name}:data:{id}").ConfigureAwait(false)).ConvertFromHashEntryList<T>());
            }
            return resultList;
        }

        /// <summary>
        /// Private helper method to get entity counts in the sorted branch.
        /// </summary>
        /// <param name="branch">Sorted Branch to query</param>
        /// <param name="from">Elements have score from</param>
        /// <param name="to">Elements have score to</param>
        /// <param name="groups">Groups values to be filtered</param>
        /// <returns>Counts of filtered items.</returns>
        private async Task<long> CountSortedAsync(IBranch<T> branch, double from, double to, params string[] groups)
        {
            return await _redisDatabase.SortedSetLengthAsync(branch.GetBranchKey(groups), from, to).ConfigureAwait(false);
        }

        /// <summary>
        /// Private helper method to update branches according to entity state and entity.
        /// </summary>
        /// <param name="entity">Entity of repository</param>
        /// <param name="entityState">RedisEntityStateEnum</param>
        /// <returns></returns>
        protected async Task UpdateBranchesAsync(T entity, RedisEntityStateEnum entityState)
        {
            switch (entityState)
            {
                case RedisEntityStateEnum.Added:
                case RedisEntityStateEnum.Updated:
                    foreach (var branch in _branches.Where(b => b.GetBranchId() != BRANCH_DATA))
                    {
                        string key = branch.GetBranchKey(entity);
                        if (((IBranchInternal<T>)branch).ApplyFilters(entity))
                        {
                            if (((IBranchInternal<T>)branch).IsSortable())
                            {
                                await _redisDatabase.SortedSetAddAsync(key, entity.Id, ((IBranchInternal<T>)branch).GetScore(entity)).ConfigureAwait(false);
                            }
                            else if (((IBranchInternal<T>)branch).IsQueryable())
                            {
                                await _redisDatabase.SortedSetAddAsync(key, entity.Id, ((IBranchInternal<T>)branch).GetScore(entity)).ConfigureAwait(false);
                            }
                            else
                            {
                                await _redisDatabase.SetAddAsync(key, entity.Id).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            if (((IBranchInternal<T>)branch).IsSortable() || ((IBranchInternal<T>)branch).IsQueryable())
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
                        if (((IBranchInternal<T>)branch).IsSortable() || ((IBranchInternal<T>)branch).IsQueryable())
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
        /// Adds entity to redis and updates branches. When you override this method do not forget to call base class method.
        /// </summary>
        /// <param name="entity">Entity of repository</param>
        public virtual async Task AddAsync(T entity)
        {
            //If entity already there, we should delete and add it again.
            //So, we can apply group by function for new values. 
            //If the entity should be deleted because updated one not fit for group by function branch, this way it is deleted
            T existedEntity = await GetByIdAsync(entity.Id);
            if (existedEntity != default)
            {
                await DeleteAsync(existedEntity);
            }

            await _redisDatabase.HashSetAsync(entity).ConfigureAwait(false);
            await UpdateBranchesAsync(entity, RedisEntityStateEnum.Added).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates entity to redis and updates branches. When you override this method do not forget to call base class method.
        /// </summary>
        /// <param name="entity">Entity of repository</param>
        public virtual async Task UpdateAsync(T entity)
        {
            //If entity already there, we should delete and add it again.
            //So, we can apply group by function for new values. 
            //If the entity should be deleted because updated one not fit for group by function branch, this way it is deleted
            T existedEntity = await GetByIdAsync(entity.Id);
            if (existedEntity != default)
            {
                await DeleteAsync(existedEntity);
            }

            await _redisDatabase.HashSetAsync(entity).ConfigureAwait(false);
            await UpdateBranchesAsync(entity, RedisEntityStateEnum.Updated).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes entity and updates branches.
        /// </summary>
        /// <param name="entity">Entity of repository</param>
        /// <returns>True if entity deleted.</returns>
        public virtual async Task<bool> DeleteAsync(T entity)
        {
            bool result = await _redisDatabase.KeyDeleteAsync(entity.GetRedisKey()).ConfigureAwait(false);
            await UpdateBranchesAsync(entity, RedisEntityStateEnum.Deleted).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Deletes entity and updates branches.
        /// </summary>
        /// <param name="id">Id of entity.</param>
        /// <returns>True if entity found and deleted.</returns>
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
        /// Get entity by id.
        /// </summary>
        /// <param name="id">Id of Entity</param>
        /// <returns>Entity of repository</returns>
        public async Task<T> GetByIdAsync(string id)
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
        /// Get all entities in branch by branchId. If the branch sorted, it returns from sorted branch.
        /// </summary>
        /// <param name="branchId">Id to identify branch</param>
        /// <param name="groups">Groups are values to create branch keys</param>
        /// <returns>Found entities</returns>
        public async Task<IEnumerable<T>> GetAsync(string branchId, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            if (((IBranchInternal<T>)branch).IsSortable())
            {
                return await GetSortedAsync(branch, double.MinValue, double.MaxValue, 0, long.MaxValue, groups);
            }
            else
            {
                RedisValue[] ids = await _redisDatabase.SetMembersAsync(branch.GetBranchKey(groups)).ConfigureAwait(false);
                List<T> resultList = new List<T>();
                foreach (RedisValue id in ids)
                {
                    resultList.Add((await _redisDatabase.HashGetAllAsync($"{typeof(T).Name}:data:{id}").ConfigureAwait(false)).ConvertFromHashEntryList<T>());
                }
                return resultList;
            }
        }

        /// <summary>
        /// Get all entities in sorted branch by branchId. If the branch is not sorted throws ArgumentException.
        /// </summary>
        /// <param name="branchId">Id to identify branch</param>
        /// <param name="from">Score from</param>
        /// <param name="groups">Groups are values to create branch keys.</param>
        /// <returns>Found entities</returns>
        public async Task<IEnumerable<T>> GetAsync(string branchId, double from, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            if (((IBranchInternal<T>)branch).IsSortable())
            {
                return await GetSortedAsync(branch, from, double.MaxValue, 0, long.MaxValue, groups);
            }
            else
            {
                throw new ArgumentException($"Branch({branchId}:{branch.GetBranchKey()} is not a sortable branch.)");
            }
        }

        /// <summary>
        /// Get all entities in sorted branch by branchId. If the branch is not sorted throws ArgumentException.
        /// </summary>
        /// <param name="branchId">Id to identify branch</param>
        /// <param name="from">Score from</param>
        /// <param name="to">Score to</param>
        /// <param name="groups">Groups are values to create branch keys.</param>
        /// <returns>Found entities</returns>
        public async Task<IEnumerable<T>> GetAsync(string branchId, double from, double to, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            if (((IBranchInternal<T>)branch).IsSortable())
            {
                return await GetSortedAsync(branch, from, to, 0, long.MaxValue, groups);
            }
            else
            {
                throw new ArgumentException($"Branch({branchId}:{branch.GetBranchKey()} is not a sortable branch.)");
            }
        }

        /// <summary>
        /// Get all entities in sorted branch by branchId. If the branch is not sorted throws ArgumentException.
        /// </summary>
        /// <param name="branchId">Id to identify branch</param>
        /// <param name="from">Score from</param>
        /// <param name="to">Score to</param>
        /// <param name="skip">Skip elements</param>
        /// <param name="take">Take elements</param>
        /// <param name="groups">Groups are values to create branch keys.</param>
        /// <returns>Found entities</returns>
        public async Task<IEnumerable<T>> GetAsync(string branchId, double from, double to, long skip, long take, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            if (((IBranchInternal<T>)branch).IsSortable())
            {
                return await GetSortedAsync(branch, from, to, skip, take, groups);
            }
            else
            {
                throw new ArgumentException($"Branch({branchId}:{branch.GetBranchKey()} is not a sortable branch.)");
            }
        }

        /// <summary>
        /// Get entity counts in the branch. If the branch sorted, it returns counts in the sorted branch.
        /// </summary>
        /// <param name="branchId">Id to identify branch</param>
        /// <param name="groups">Groups parameters</param>
        /// <returns>Count of entities meet with criterias</returns>
        public async Task<long> CountAsync(string branchId, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            if (((IBranchInternal<T>)branch).IsSortable())
            {
                return await CountSortedAsync(branch, double.MinValue, double.MaxValue, groups);
            }
            else
            {
                return await _redisDatabase.SetLengthAsync(branch.GetBranchKey(groups)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get entity counts in the branch. If the branch is not sorted throws ArgumentException.
        /// </summary>
        /// <param name="branchId">Id to identify branch</param>
        /// <param name="from">Score from</param>
        /// <param name="groups">Groups are values to create branch keys.</param>
        /// <returns>Count of entities meet with criterias</returns>
        public async Task<long> CountAsync(string branchId, double from, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            if (((IBranchInternal<T>)branch).IsSortable())
            {
                return await CountSortedAsync(branch, from, double.MaxValue, groups);
            }
            else
            {
                throw new ArgumentException($"Branch({branchId}:{branch.GetBranchKey()} is not a sortable branch.)");
            }
        }

        /// <summary>
        /// Get entity counts in the branch. If the branch is not sorted throws ArgumentException.
        /// </summary>
        /// <param name="branchId">Id to identify branch</param>
        /// <param name="from">Score from</param>
        /// <param name="to">Score to</param>
        /// <param name="groups">Groups are values to create branch keys.</param>
        /// <returns>Count of entities meet with criterias</returns>
        public async Task<long> CountAsync(string branchId, double from, double to, params string[] groups)
        {
            IBranch<T> branch = _branches.Find(b => b.GetBranchId() == branchId);
            if (branch == default)
            {
                throw new KeyNotFoundException($"branchId not found: {branchId}. Possible branches: {String.Join(", ", _branches.Select(i => i.GetBranchKey()))}");
            }

            if (((IBranchInternal<T>)branch).IsSortable())
            {
                return await CountSortedAsync(branch, from, to, groups);
            }
            else
            {
                throw new ArgumentException($"Branch({branchId}:{branch.GetBranchKey()} is not a sortable branch.)");
            }
        }

        /// <summary>
        /// Adds branch. If you add new branch outside of CreateBranches, newly added branch criterias are not applied on already saved entities.
        /// </summary>
        /// <param name="branch">IBranch</param>
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
        /// Creates branches in this method.
        /// </summary>
        public abstract void CreateBranches();

        /// <summary>
        /// Get branch keys.
        /// </summary>
        /// <returns>Branch keys</returns>
        public IEnumerable<string> GetBranches()
        {
            foreach (IBranch<T> branch in Branches)
            {
                yield return branch.GetBranchKey();
            }
        }
    }
}

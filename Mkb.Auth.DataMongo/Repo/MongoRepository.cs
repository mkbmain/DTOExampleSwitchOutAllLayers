using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pluralize.NET;

namespace Mkb.Auth.DataMongo.Repo
{
    /// <summary>
    ///     Defines the <see cref="MongoRepository" />.
    /// </summary>
    /// <typeparam name="TEntity">Type of a class.</typeparam>
    public class MongoRepository : IMongoRepository
    {
        private readonly MongoClient _mongoClient;
        private readonly IPluralize _pluralize;
        private readonly string DbName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MongoRepository" /> class.
        /// </summary>
        public MongoRepository( string dbName, IPluralize pluralize, MongoClient mongoClient)
        {
            _pluralize = pluralize;
            _mongoClient = mongoClient;
            DbName = dbName;
        }

        /// <summary>
        ///     The Add.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The <see cref="Task{TEntity}" />.</returns>
        public async Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : MongoEntity
        {
            try
            {
                await Collection<TEntity>().InsertOneAsync(entity);
                return entity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Task BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : MongoEntity
        {
            return Collection<TEntity>().InsertManyAsync(entities);
        }

        public Task<long> CountAsync<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : MongoEntity
        {
            return Collection<TEntity>().Find(condition.AndAlso(o => o.IsArchived == false)).CountDocumentsAsync();
        }

        public async Task<bool> DeleteAsync<TEntity>(string id) where TEntity : MongoEntity
        {
            var update = Builders<TEntity>.Update
                .Set(m => m.IsArchived, true);

            var options = new FindOneAndUpdateOptions<TEntity, TEntity>
            {
                IsUpsert = false,
                ReturnDocument = ReturnDocument.After
            };

            var updatedItem = await Collection<TEntity>()
                .FindOneAndUpdateAsync<TEntity>(options => options.Id == id, update, options);
            return updatedItem != null;
        }

        public async Task<bool> DeleteBulkAsync<TEntity>(IEnumerable<string> ids) where TEntity : MongoEntity
        {
            if (ids.Count() == 0) throw new Exception("No ID's were passed");

            Expression<Func<TEntity, bool>> field = o => o.IsArchived;

            var filter = Builders<TEntity>.Filter.In(x => x.Id, ids) &
                         Builders<TEntity>.Filter.Where(o => o.IsArchived == false);

            var updateDefinition = Builders<TEntity>.Update.Set(field, true);

            var result = await Collection<TEntity>().UpdateManyAsync(filter, updateDefinition);

            return result.ModifiedCount > 0;
        }

        public Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : MongoEntity
        {
            return Collection<TEntity>().Find(condition.AndAlso(o => o.IsArchived == false)).AnyAsync();
        }

        public Task<TEntity> FindOneAsync<TEntity>(Expression<Func<TEntity, bool>> condition)
            where TEntity : MongoEntity
        {
            return Collection<TEntity>().Find(condition.AndAlso(o => o.IsArchived == false)).FirstOrDefaultAsync();
        }

        public Task<List<TEntity>> GetAllAsync<TEntity>(SearchParameter<TEntity> searchParameter)
            where TEntity : MongoEntity
        {
            var collection = Collection<TEntity>().Find(searchParameter.Condition.AndAlso(o => o.IsArchived == false));
            if (searchParameter.Sort != null)
                collection = !searchParameter.SortByDescending
                    ? collection.SortBy(searchParameter.Sort)
                    : collection.SortByDescending(searchParameter.Sort);

            return collection
                .Skip(searchParameter.PageIndex * searchParameter.PageSize)
                .Limit(searchParameter.PageSize)
                .ToListAsync();
        }

        public Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : MongoEntity
        {
            return GetAllAsync<TEntity>(f => f.IsArchived == false);
        }

        /// <summary>
        ///     Get list of entity.
        /// </summary>
        /// <param name="searchParameter">The searchParameter<see cref="SearchParameter{TEntity}" />.</param>
        /// <returns>List of entity.</returns>
        public Task<List<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> func,
            Expression<Func<TEntity, object>> sort = null, bool sortByDescending = false)
            where TEntity : MongoEntity
        {
            return GetAllAsync(new SearchParameter<TEntity>
                {Condition = func, Sort = sort, SortByDescending = sortByDescending});
        }

        public Task<TEntity> GetByIdAsync<TEntity>(string id) where TEntity : MongoEntity
        {
            return Collection<TEntity>().Find(item => item.IsArchived == false && item.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<TEntity>> GetByIdsAsync<TEntity>(IEnumerable<string> ids,
            Expression<Func<TEntity, bool>> condition) where TEntity : MongoEntity
        {
            var filter = Builders<TEntity>.Filter.In(x => x.Id, ids) &
                         Builders<TEntity>.Filter.Where(o => o.IsArchived == false)
                         & Builders<TEntity>.Filter.Where(condition);
            return Collection<TEntity>().Find(filter).ToListAsync();
        }

        public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : MongoEntity
        {
            var result = await Collection<TEntity>().ReplaceOneAsync(item => item.Id == entity.Id, entity);
            if (result.IsAcknowledged) return entity;

            throw new Exception("Update Failed.");
        }


        public IMongoCollection<T> Collection<T>() where T : MongoEntity
        {
            var database = _mongoClient.GetDatabase(DbName);
            return database.GetCollection<T>(_pluralize.Pluralize(typeof(T).Name.ToLower()));
        }
    }
}
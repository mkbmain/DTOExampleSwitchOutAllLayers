using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mkb.Auth.DataMongo.Repo
{
    public interface IMongoRepository
    {
        Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : MongoEntity;

        Task BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : MongoEntity;

        Task<long> CountAsync<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : MongoEntity;


        Task<bool> DeleteAsync<TEntity>(string id) where TEntity : MongoEntity;


        Task<bool> DeleteBulkAsync<TEntity>(IEnumerable<string> ids) where TEntity : MongoEntity;


        Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : MongoEntity;


        Task<TEntity> FindOneAsync<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : MongoEntity;


        Task<List<TEntity>> GetAllAsync<TEntity>(SearchParameter<TEntity> searchParameter)
            where TEntity : MongoEntity;

        Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : MongoEntity;


        Task<List<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> func,
            Expression<Func<TEntity, object>> sort = null, bool sortByDescending = false)
            where TEntity : MongoEntity;


        Task<TEntity> GetByIdAsync<TEntity>(string id) where TEntity : MongoEntity;

        Task<List<TEntity>> GetByIdsAsync<TEntity>(IEnumerable<string> ids, Expression<Func<TEntity, bool>> condition)
            where TEntity : MongoEntity;

        Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : MongoEntity;
    }
}
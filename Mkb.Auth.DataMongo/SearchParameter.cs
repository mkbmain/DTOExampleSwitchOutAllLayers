using System;
using System.Linq.Expressions;
using Mkb.Auth.DataMongo.Repo;

namespace Mkb.Auth.DataMongo
{
    public class SearchParameter<TEntity> where TEntity : MongoEntity
    {
        public SearchParameter()
        {
            Condition = o => o.Id != null;
            PageSize = 10;
            PageIndex = 0;
            Sort = null;
            SortByDescending = false;
        }

        public Expression<Func<TEntity, object>> Sort { get; set; }
        public bool SortByDescending { get; set; }

        public Expression<Func<TEntity, bool>> Condition { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; } = 10;
    }
}
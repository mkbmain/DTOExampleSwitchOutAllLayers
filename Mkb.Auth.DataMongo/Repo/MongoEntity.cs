using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mkb.Auth.DataMongo.Repo
{
    public class MongoEntity : ISupportInitialize
    {
        public MongoEntity()
        {
            // A new entity is never archived.
            IsArchived = false;
        }

        [BsonExtraElements] [JsonIgnore] public IDictionary<string, object> ExtraElements { get; set; }


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        public bool IsArchived { get; set; }


        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }
    }
}
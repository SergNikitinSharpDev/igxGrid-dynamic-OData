using Dapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Edm;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace PostgreODataAPI.DynamicOData
{
    public class PostgreDataService: IDataService
    {
        private readonly IConfiguration _configuration;

        public PostgreDataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private EdmEntityObject CreateEdmEntity(IEdmEntityType entityType, dynamic row)
        {
            if (row == null)
                return null;

            var entity = new EdmEntityObject(entityType);
            IDictionary<string, object> propertyMap = row as IDictionary<string, object>;

            if (propertyMap != null)
            {
                foreach (var propertyPair in propertyMap)
                    entity.TrySetPropertyValue(propertyPair.Key, propertyPair.Value);
            }

            return entity;
        }

        public int Count(IEdmCollectionType collectionType, ODataQueryOptions queryOptions, string clientName)
        {
            var entityType = collectionType.ElementType.Definition as EdmEntityType;
            int count = 0;

            if (entityType != null)
            {
                using (var connection = new NpgsqlConnection(_configuration.GetConnectionString(clientName)))
                {
                    var sqlBuilder = new PostgreSqlQueryBuilder(queryOptions);
                    count = connection.Query<int>(sqlBuilder.ToCountSql()).Single();
                }
            }

            return count;
        }

        public EdmEntityObjectCollection Get(IEdmCollectionType collectionType, ODataQueryOptions queryOptions, string clientName)
        {
            var entityType = collectionType.ElementType.Definition as EdmEntityType;
            var collection = new EdmEntityObjectCollection(new EdmCollectionTypeReference(collectionType));

            if (entityType != null)
            {
                using (var connection = new NpgsqlConnection(_configuration.GetConnectionString(clientName)))
                {
                    var sqlBuilder = new PostgreSqlQueryBuilder(queryOptions);
                    IEnumerable<dynamic> rows = connection.Query<dynamic>(sqlBuilder.ToSql());

                    foreach (dynamic row in rows)
                    {
                        var entity = CreateEdmEntity(entityType, row);
                        collection.Add(entity);
                    }
                }
            }

            return collection;
        }

        public EdmEntityObject Get(string key, IEdmEntityType entityType, string clientName)
        {
            var keys = entityType.DeclaredKey.ToList();

            // make sure entity type has unique key, not composite key
            if (keys.Count != 1)
                return null;

            var sql = $@"SELECT * FROM [{entityType.Namespace}].[{entityType.Name}] WHERE [{keys.First().Name}] = @Key";

            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString(clientName)))
            {
                var row = connection.Query(sql, new
                {
                    Key = key
                }).SingleOrDefault();

                var entity = CreateEdmEntity(entityType, row);
                return entity;
            }
        }
    }
}

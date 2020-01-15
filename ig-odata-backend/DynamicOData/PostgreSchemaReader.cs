using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreODataAPI.DynamicOData
{
    public class PostgreSchemaReader : ISchemaReader
    {
        private readonly string _connectionString;

        public PostgreSchemaReader(IConfiguration configuration, string clientName)
        {
            _connectionString = configuration.GetConnectionString(clientName);
        }

        private string BuildSql(IEnumerable<TableInfo> tableInfos)
        {
            string sql = @"select * from 
            (   SELECT table_schema as pSchema,
	                table_name as pTable,
	                column_name as Name,
	                (case when is_identity = 'YES' then true else false end) AS IsPrimaryKey,
	                (case when is_nullable = 'YES' then true else false end) AS Nullable,
	                data_type as DataType
                FROM information_schema.columns
                WHERE data_type!='USER-DEFINED'
            ) as subq";

            if (tableInfos != null && tableInfos.Any())
            {
                var pairClauses = tableInfos.Select(
               info => $"(pSchema = '{info.Schema}' AND pTable = '{info.Name}')");

                string whereClause = string.Join(" OR ", pairClauses);

                if (!string.IsNullOrEmpty(whereClause))
                    sql += " WHERE " + whereClause;
            }

            return sql;
        }

        private IEnumerable<DatabaseColumn> GetColumns(IEnumerable<TableInfo> tableInfos)
        {
            string sql = BuildSql(tableInfos);

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var databaseColumns = connection.Query<DatabaseColumn>(sql);
                return databaseColumns;
            }
        }

        public IEnumerable<DatabaseTable> GetTables(IEnumerable<TableInfo> tableInfos)
        {
            var columns = GetColumns(tableInfos);
            List<DatabaseTable> tables = new List<DatabaseTable>();

            foreach (var schema in columns.GroupBy(c => c.pSchema))
            {
                var tableList = schema.GroupBy(c => c.pTable).Select(tableGroup => new DatabaseTable()
                {
                    Schema = schema.Key,
                    Name = tableGroup.Key,
                    Columns = tableGroup.AsEnumerable()
                });

                tables.AddRange(tableList);
            }

            return tables;
        }
    }
}

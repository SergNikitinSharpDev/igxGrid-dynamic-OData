using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreODataAPI.DynamicOData
{
    public class PostgreEdmModelBuilder : IEdmModelBuilder
    {
        private readonly ISchemaReader _schemaReader;

        public PostgreEdmModelBuilder(ISchemaReader schemaReader)
        {
            _schemaReader = schemaReader;
        }

        private static IDictionary<string, EdmPrimitiveTypeKind> BuildEdmTypeMap()
        {
            var map = new Dictionary<string, EdmPrimitiveTypeKind>
            {
                {"tinyint", EdmPrimitiveTypeKind.Byte},
                {"smallint", EdmPrimitiveTypeKind.Int16},
                {"integer", EdmPrimitiveTypeKind.Int32},
                {"bigint", EdmPrimitiveTypeKind.Int64},
                {"float", EdmPrimitiveTypeKind.Double},
                {"real", EdmPrimitiveTypeKind.Single},
                {"uuid", EdmPrimitiveTypeKind.Guid},
                {"geography", EdmPrimitiveTypeKind.Geography},
                {"boolean", EdmPrimitiveTypeKind.Boolean},
                {"binary", EdmPrimitiveTypeKind.Binary},
                {"USER-DEFINED", EdmPrimitiveTypeKind.None}
            };

            var stringTypes = new[] { "char", "nchar", "varchar", "nvarchar", "text", "ntext" }
                .ToDictionary(s => s, _ => EdmPrimitiveTypeKind.String);

            var decimalTypes = new[] { "decimal", "numeric", "money", "smallmoney" }
                .ToDictionary(s => s, _ => EdmPrimitiveTypeKind.Decimal);

            //TODO check for Datetime (was not available)
            var dateTimeTypes = new[] { "datetime", "smalldatetime", "date" }
                .ToDictionary(s => s, _ => EdmPrimitiveTypeKind.Date);

            var timeStampTypes = new[] { "time", "timestamp", "timestamp with time zone" }
                .ToDictionary(s => s, _ => EdmPrimitiveTypeKind.DateTimeOffset);

            map = map.Concat(stringTypes)
                .Concat(decimalTypes)
                .Concat(dateTimeTypes)
                .Concat(timeStampTypes)
                .ToDictionary(x => x.Key, x => x.Value);

            return map;
        }

        private static EdmStructuralProperty BuildProperty(EdmEntityType entity, DatabaseColumn column)
        {
            var typeKind = EdmPrimitiveTypeKind.String;
            var typeMap = BuildEdmTypeMap();

            if (typeMap.ContainsKey(column.DataType))
            {
                typeKind = typeMap[column.DataType];
            }

            return entity.AddStructuralProperty(column.Name, typeKind, column.Nullable);
        }


        private static EdmEntityType BuildEdmEntityType(DatabaseTable table)
        {
            EdmEntityType entity = new EdmEntityType(table.Schema, table.Name);

            foreach (var column in table.Columns)
            {
                var property = BuildProperty(entity, column);

                if (column.IsPrimaryKey)
                    entity.AddKeys(property);
            }

            return entity;
        }

        public EdmModel GetModel(string clientName)
        {
            EdmModel model = new EdmModel();
            EdmEntityContainer container = new EdmEntityContainer("ns", "container");
            model.AddElement(container);

            var tableInfos = new List<TableInfo>() { 
                new TableInfo() { Schema="main", Name= "project_construction" },
                new TableInfo() { Schema="main", Name= "system_event_cursor" }
            };
            var databaseTables = _schemaReader.GetTables(tableInfos, clientName);

            foreach (var table in databaseTables)
            {
                var entityType = BuildEdmEntityType(table);
                string setName = table.Name.Replace(" ", string.Empty);

                model.AddElement(entityType);
                container.AddEntitySet(setName, entityType);
            }

            return model;
        }
    }
}

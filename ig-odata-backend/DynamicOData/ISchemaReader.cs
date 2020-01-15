using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Collections.Generic;

namespace PostgreODataAPI.DynamicOData
{
    public interface ISchemaReader
    {
        IEnumerable<DatabaseTable> GetTables(IEnumerable<TableInfo> tableInfos, string clientName);
    }
}

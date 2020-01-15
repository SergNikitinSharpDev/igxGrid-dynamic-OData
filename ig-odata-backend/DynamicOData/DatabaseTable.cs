using System.Collections.Generic;

namespace PostgreODataAPI.DynamicOData
{
    public class DatabaseTable
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public IEnumerable<DatabaseColumn> Columns { get; set; }
    }
}

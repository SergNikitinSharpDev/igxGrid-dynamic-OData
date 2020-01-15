namespace PostgreODataAPI.DynamicOData
{
    public class DatabaseColumn
    {
        public string pSchema { get; set; }
        public string pTable { get; set; }
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool Nullable { get; set; }

        public string DataType { get; set; }
    }
}

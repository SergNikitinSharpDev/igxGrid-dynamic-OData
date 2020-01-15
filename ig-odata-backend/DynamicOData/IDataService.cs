using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;

namespace PostgreODataAPI.DynamicOData
{
    public interface IDataService
    {
        EdmEntityObjectCollection Get(IEdmCollectionType collectionType, ODataQueryOptions queryOptions, string clientName);

        EdmEntityObject Get(string key, IEdmEntityType entityType, string clientName);

        int Count(IEdmCollectionType collectionType, ODataQueryOptions queryOptions, string clientName);
    }
}

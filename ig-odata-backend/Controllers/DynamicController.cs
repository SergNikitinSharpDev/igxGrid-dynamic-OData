using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using PostgreODataAPI.DynamicOData;
using PostgreODataAPI.Routing;
using ODataPath = Microsoft.AspNet.OData.Routing.ODataPath;

namespace PostgreODataAPI.Controllers
{
    public class DynamicController : ODataController
    {
        private readonly IDataService _dataService;
        private readonly IEdmModelBuilder _edmModelBuilder;

        public DynamicController(IDataService dataService, IEdmModelBuilder edmModelBuilder)
        {
            _dataService = dataService;
            _edmModelBuilder = edmModelBuilder;
        }

        // Get entityset
        public EdmEntityObjectCollection Get()
        {
            // Get entity set's EDM type: A collection type.
            ODataPath path = Request.ODataFeature().Path;
            IEdmCollectionType collectionType = (IEdmCollectionType)path.EdmType;
            var entityType = collectionType?.ElementType.Definition as IEdmEntityType;

            string sourceString = Request.GetDataSource();
            var model = _edmModelBuilder.GetModel(sourceString);

            var queryContext = new ODataQueryContext(model, entityType, path);
            var queryOptions = new ODataQueryOptions(queryContext, Request);

            // make $count works
            var oDataProperties = Request.ODataFeature();
            if (queryOptions.Count != null)
            {
                oDataProperties.TotalCount = _dataService.Count(collectionType, queryOptions, sourceString);
            }

            //make $select works
            if (queryOptions.SelectExpand != null)
            {
                oDataProperties.SelectExpandClause = queryOptions.SelectExpand.SelectExpandClause;
            }

            var collection = _dataService.Get(collectionType, queryOptions, sourceString);

            return collection;
        }

        public IEdmEntityObject Get(string key)
        {
            ODataPath path = Request.ODataFeature().Path;
            IEdmEntityType entityType = (IEdmEntityType)path.EdmType;
            string sourceString = Request.GetDataSource();

            var entity = _dataService.Get(key, entityType, sourceString);

            // make sure return 404 if key does not exist in database
            if (entity == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return entity;
        }
    }
}

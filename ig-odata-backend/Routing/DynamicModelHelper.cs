using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Edm;
using PostgreODataAPI.DynamicOData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PostgreODataAPI.Routing
{
    [Obsolete]
    public class DynamicModelHelper
    {
        //public static ODataRoute CustomMapODataServiceRoute(IList<IRouter> routes, IConfiguration configuration, string routeName, string routePrefix, HttpMessageHandler handler = null)
        //{
        //    if (!string.IsNullOrEmpty(routePrefix))
        //    {
        //        int prefixLastIndex = routePrefix.Length - 1;
        //        if (routePrefix[prefixLastIndex] == '/')
        //        {
        //            routePrefix = routePrefix.Substring(0, routePrefix.Length - 1);
        //        }
        //    }

        //    var pathHandler = new DefaultODataPathHandler();

        //    var routingConventions = ODataRoutingConventions.CreateDefault();
        //    routingConventions.Insert(0, new DynamicRoutingConvention());

        //    var modelProvider = GetModelFuncFromRequest(configuration);

        //    var routeConstraint = new CustomODataPathRouteConstraint(pathHandler, modelProvider, routeName, routingConventions);

        //    var odataRoute = new CustomODataRoute(
        //        routeName: routeName,
        //        routePrefix: routePrefix,
        //        pathConstraint: routeConstraint,
        //        defaults: null,
        //        constraints: null,
        //        dataTokens: null,
        //        handler: handler);

        //    routes.Add(odataRoute);

        //    return odataRoute;
        //}

        //private static Func<HttpRequestMessage, IEdmModel> GetModelFuncFromRequest(IConfiguration configuration)
        //{
        //    return request =>
        //    {
        //        string odataPath = request.Properties[Constants.CustomODataPath] as string ?? string.Empty;
        //        string[] segments = odataPath.Split('/');
        //        string odataEndpoint = segments[0];

        //        request.Properties[Constants.ODataEndpoint] = odataEndpoint;
        //        request.Properties[Constants.CustomODataPath] = string.Join("/", segments, 1, segments.Length - 1);

        //        var modelBuilder = new EdmModelBuilder(new PostgreSchemaReader(configuration,odataEndpoint));
        //        IEdmModel model = modelBuilder.GetModel();

        //        return model;
        //    };
        //}
    }
}

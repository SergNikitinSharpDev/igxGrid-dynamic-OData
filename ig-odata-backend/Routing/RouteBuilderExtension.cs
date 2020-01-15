using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData;
using ServiceLifetime = Microsoft.OData.ServiceLifetime;
using Microsoft.OData.Edm;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing.Conventions;
using PostgreODataAPI.DynamicOData;
using Microsoft.Extensions.Configuration;

namespace PostgreODataAPI.Routing
{
    public static class RouteBuilderExtension
    {
        public static ODataRoute CustomMapODataServiceRoute(this IRouteBuilder routeBuilder, string routeName, string routePrefix, IConfiguration configuration)
        {
            ODataRoute route = routeBuilder.MapODataServiceRoute(routeName, routePrefix, builder =>
            {
                // Get the model from the datasource of the current request: model-per-pequest.
                builder.AddService(ServiceLifetime.Scoped, sp =>
                {
                    var serviceScope = sp.GetRequiredService<HttpRequestScope>();

                    // serviceScope.
                    string sourceString = serviceScope.HttpRequest.GetDataSource();
                    var modelBuilder = new PostgreEdmModelBuilder(new PostgreSchemaReader(configuration,sourceString));
                    IEdmModel model = modelBuilder.GetModel();

                    return model;
                });

                // The routing conventions are registered as singleton.
                builder.AddService(ServiceLifetime.Singleton, sp =>
                {
                    IList<IODataRoutingConvention> routingConventions = ODataRoutingConventions.CreateDefault();
                    routingConventions.Insert(0, new MatchAllRoutingConvention());
                    return routingConventions.ToList().AsEnumerable();
                });
            });

            // route.Constraints.
            IRouter customRouter = routeBuilder.ServiceProvider.GetService<IRouter>();

            // Get constraint resolver.
            IInlineConstraintResolver inlineConstraintResolver = routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();

            CustomODataRoute odataRoute = new CustomODataRoute(customRouter != null ? customRouter : routeBuilder.DefaultHandler,
                routeName,
                routePrefix,
                new CustomODataPathRouteConstraint(routeName),
                inlineConstraintResolver);

            routeBuilder.Routes.Remove(route);
            routeBuilder.Routes.Add(odataRoute);

            return odataRoute;
        }
    }
}

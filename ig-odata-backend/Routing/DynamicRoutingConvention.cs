using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Routing;

namespace PostgreODataAPI.Routing
{
    [Obsolete]
    //https://github.com/OData/WebApi/issues/1377
    public class DynamicRoutingConvention : IODataRoutingConvention
    {
        //public string SelectController(Microsoft.AspNet.OData.Routing.ODataPath odataPath, HttpRequestMessage request)
        //{
        //    if (odataPath.Segments.FirstOrDefault() is EntitySetPathSegment)
        //        return "Dynamic";

        //    return "DynamicOdataMetadata";
        //}

        public IEnumerable<ControllerActionDescriptor> SelectAction(RouteContext routeContext)
        {
            return null;
        }

    }
}

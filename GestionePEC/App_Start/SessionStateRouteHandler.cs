using CertiWebAppAnag.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace GestionePEC.App_Start
{
    public class SessionStateRouteHandler : IRouteHandler
    {
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
        {
            return new SessionableControllerHandler(requestContext.RouteData);
        }
    }
}  

    
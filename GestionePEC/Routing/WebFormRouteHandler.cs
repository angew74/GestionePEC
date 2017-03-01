using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Routing;
using System.Security;
using System.Web.Compilation;

namespace GestionePEC.Routing
{
    public interface IRoutablePage
    {
        RequestContext RequestContext { set; }
    }

    public class WebFormRouteHandler : IRouteHandler
    {
        public string VirtualPath { get; private set; }
        public bool CheckPhysicalUrlAccess { get; set; }

        public WebFormRouteHandler(string virtualPath)
            : this(virtualPath, true)
        {
        }

        public WebFormRouteHandler(string virtualPath, bool checkPhysicalUrlAccess)
        {
            this.VirtualPath = virtualPath;
            this.CheckPhysicalUrlAccess = checkPhysicalUrlAccess;
        }

        #region IRouteHandler Membri di

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (this.CheckPhysicalUrlAccess &&
                !UrlAuthorizationModule.CheckUrlAccessForPrincipal(
                this.VirtualPath,
                requestContext.HttpContext.User,
                requestContext.HttpContext.Request.HttpMethod))
                throw new SecurityException();

            var page = BuildManager.CreateInstanceFromVirtualPath(this.VirtualPath,
                typeof(Page)) as IHttpHandler;
            if (page != null)
            {
                var routablePage = page as IRoutablePage;
                if (routablePage != null) routablePage.RequestContext = requestContext;
            }
            return page;
        }

        #endregion
    }
}

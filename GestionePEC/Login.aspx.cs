using AspNet.Identity.SQLServerProvider;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Logging.Mail;
using Com.Delta.Security;
using GestionePEC.Extensions;
using GestionePEC.pages.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC
{
    public partial class Login : BasePage, IAccessPage
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Login));
        private const int CACHEEXPIRATION = 15;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            if (MySecurityProvider.CurrentPrincipal != null
                    && MySecurityProvider.CurrentPrincipal.Identity.IsAuthenticated)
                Response.Redirect("~/pages/Default.aspx");

                      
        }
                   
    }
}

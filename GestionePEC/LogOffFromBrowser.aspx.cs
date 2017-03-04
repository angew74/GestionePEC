using GestionePEC.Extensions;
using GestionePEC.pages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC
{
    public partial class LogOffFromBrowser : BasePage, IAccessPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string user = String.Empty;
            string dip = String.Empty;
            try
            {
                user = Request.QueryString["usr"];
                dip = Request.QueryString["dip"];
            }
            finally
            {
                if (!"".Equals(user) && !"".Equals(dip)) //se è stato effettuato il login, viene rimossa la sessione
                {
                    //remove della sessione
                    Cache.Remove(Context.User.Identity.Name);
                    System.Threading.Thread.CurrentPrincipal = null;
                    Context.User = null;
                    System.Web.Security.FormsAuthentication.SignOut();
                    Session.Abandon();
                }
                Response.Redirect("~/Login.aspx");
            }
        }
    }
}
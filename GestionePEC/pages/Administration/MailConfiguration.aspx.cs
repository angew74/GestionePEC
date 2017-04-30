using Com.Delta.Security;
using GestionePEC.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.pages.Administration
{
    public partial class MailConfiguration : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (MySecurityProvider.CurrentPrincipal == null)
            {
                Redirect("LogOffPage", null);
                //Response.Redirect("~/LogOff.aspx");
            }

        }
    }
}
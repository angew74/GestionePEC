using Com.Delta.Logging;
using Com.Delta.Logging.Mail;
using Com.Delta.Security;
using GestionePEC.Extensions;
using GestionePEC.pages.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC
{
    public partial class LogOff : BasePage, IAccessPage
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LogOff));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (MySecurityProvider.CurrentPrincipal != null)
                {
                    MailLogInfo loginfo = new MailLogInfo("MAIL", "LGO", MySecurityProvider.CurrentPrincipal.Identity.Name,"", "");
                    Cache.Remove(Context.User.Identity.Name);
                }
                System.Threading.Thread.CurrentPrincipal = null;
                Context.User = null;
                System.Web.Security.FormsAuthentication.SignOut();
                Session.Abandon();             

            }
            catch (ManagedException) { }
            catch (Exception ex)
            {
                _log.Error(new Com.Delta.Logging.Errors.ErrorLog("MAIL", ex, string.Empty, string.Empty, string.Empty));
                info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }
            Response.Redirect("~/Login.aspx");
        }
    }
}
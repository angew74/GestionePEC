using AspNet.Identity.SQLServerProvider;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Messaging.MapperMessages;
using Com.Delta.Security;
using GestionePEC.Extensions;
using log4net;
using SendMail.BusinessEF;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.pages.Users
{
    public partial class RegisterUser : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger("RegisterUser");
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }
     
    }
}
using AspNet.Identity.SQLServerProvider;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Messaging.MapperMessages;
using GestionePEC.Extensions;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.pages.Roles
{
    public partial class RegisterRole : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger("RegisterRole");
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void CreateRole_Click(object sender, EventArgs e)
        {
            var roleStore = new RoleStore();
            var role = new IdentityRole() { Name = RoleName.Text };           
            try
            {
                string result = roleStore.CreateAsync(role).Result;
                if (result == "OK")
                {
                    info.AddMessage(string.Format("Ruolo {0} è stato correttamente creato!", role.Name), LivelloMessaggio.INFO);
                }
                else
                {
                    info.AddMessage(string.Format("Ruolo non creato"), LivelloMessaggio.ERROR);
                }                
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Errore creazione Ruolo. Dettaglio: " + ex.Message +
                        "StackTrace: " + ((ex.StackTrace != null) ? ex.StackTrace.ToString() : " vuoto "),
                        "ERR317",
                        string.Empty,
                        string.Empty,
                        ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
                info.AddMessage("Errore nell'inserimento: " + ex.Message, LivelloMessaggio.ERROR);
            }
        }
    }
}
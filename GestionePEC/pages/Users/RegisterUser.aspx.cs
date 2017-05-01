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
            if(Page.IsPostBack == false)
            {
                var roleStore = new RoleStore();              
                Role.DataTextField = "Name";
                Role.DataValueField = "Id";
                List<IdentityRole> r = null;
                r = roleStore.GetAll().Result;
                Role.DataSource = r;
                Role.DataBind();
            }
        }

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            // Default UserStore constructor uses the default connection string named: DefaultConnection
            var userStore = new UserStore();
            var user = new IdentityUser() { UserName = UserName.Text };
            user.PasswordHash = MySecurityProvider.PlainToSHA256(Password.Text);
            user.SecurityStamp = System.DateTime.Now.Ticks.ToString();            
            try
            {
                string result = userStore.CreateAsync(user).Result;              
                if (result == "OK")
                {
                    BackendUserService bus = new BackendUserService();
                    BackendUser userBackend = new BackendUser();
                    userBackend.Cognome = Cognome.Text.Trim().ToUpper();
                    userBackend.Nome = Nome.Text.Trim().ToUpper();
                    userBackend.UserName = UserName.Text.Trim().ToUpper();
                    userBackend.CodiceFiscale = CodiceFiscale.Text.Trim().ToUpper();
                    bus.Save(userBackend);
                    info.AddMessage(string.Format("Utente {0} è stato correttamente creato!", user.UserName), LivelloMessaggio.INFO);
                }
                else
                {
                    info.AddMessage(string.Format("Utente non creato"), LivelloMessaggio.ERROR);
                }
                var resultRole = (userStore.AddToRoleAsync(user, int.Parse(Role.SelectedValue))).Result;
                if(resultRole != 1)
                {
                    info.AddMessage(string.Format("Utente {0} non aggiunto a ruolo {1} è stato correttamente creato!", user.UserName,Role.SelectedItem.Text), LivelloMessaggio.INFO);
                }
            }
            catch(Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Errore creazione utente. Dettaglio: " + ex.Message +
                        "StackTrace: " + ((ex.StackTrace != null) ? ex.StackTrace.ToString() : " vuoto "),
                        "ERR317",
                        string.Empty,
                        string.Empty,
                        ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
            }
        }
    }
}
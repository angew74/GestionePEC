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

namespace GestionePEC.pages.Users
{
    public partial class ManagerUserToRole : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger("ManagerUserToRole");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                var roleStore = new RoleStore();
                Role.DataTextField = "Name";
                Role.DataValueField = "Id";
                List<IdentityRole> r = null;
                r = roleStore.GetAll().Result;
                Role.DataSource = r;
                Role.DataBind();
                var userStore = new UserStore();
                Users.DataTextField = "UserName";
                Users.DataValueField = "Id";
                List<IdentityUser> L = userStore.GetAll().Result;
                Users.DataSource = L;
                Users.DataBind();

            }
        }
        protected void CreateUserRole_Click(object sender, EventArgs e)
        {
            var userStore = new UserStore();
            var user = new IdentityUser() { Id = Users.SelectedValue, UserName = Users.SelectedItem.Text };
            try
            {
                var result = userStore.AddToRoleAsync(user, int.Parse(Role.SelectedValue)).Result;
                if (result != 0)
                {
                    info.AddMessage(string.Format("Utente {0} aggiunto a ruolo {1} !", user.UserName, Role.SelectedItem.Text), LivelloMessaggio.INFO);
                }
            }
            catch (Exception ex)
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
                    info.AddMessage("Errore nell'associazione utente a ruolo: " + ex.Message,LivelloMessaggio.ERROR);
                }
            }
        }

        protected void btnFindUser_OnClick(object sender, EventArgs e)
        {

        }
        protected void btnFindRole_OnClick(object sender, EventArgs e)
        {

        }
    }
}
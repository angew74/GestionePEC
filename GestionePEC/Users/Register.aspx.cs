using AspNet.Identity.SQLServerProvider;
using Com.Delta.Security;
using GestionePEC.Extensions;
using GestionePEC.pages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Users
{
    public partial class Register : BasePage, IAccessPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            // Default UserStore constructor uses the default connection string named: DefaultConnection
            var userStore = new UserStore();          
            var user = new IdentityUser() { UserName = UserName.Text};
            user.PasswordHash = MySecurityProvider.PlainToSHA256(Password.Text);
            user.SecurityStamp = System.DateTime.Now.Ticks.ToString();
            string result = userStore.CreateAsync(user).Result;
            if (result== "OK")
            {
                StatusMessage.Text = string.Format("Utente {0} è stato correttamente creato!", user.UserName);
            }
            else
            {
                StatusMessage.Text = "Utente non creato";
            }
        }
    }
}
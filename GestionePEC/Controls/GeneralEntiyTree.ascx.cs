using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class GeneralEntiyTree : System.Web.UI.UserControl
    {
        public string TreeViewDataSource
        {
            get
            {
                return "~/api/TreeController/GetTree";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class MailTreeViewer : System.Web.UI.UserControl
    {
        public event EventHandler MailSelected;
        private void onMailSelected()
        {
            if (MailSelected != null)
                MailSelected(this, EventArgs.Empty);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            butMailViewer.Style.Add(HtmlTextWriterStyle.Display, "none");
        }

        protected void butMailViewer_Click(object sender, EventArgs e)
        {
            onMailSelected();
        }
    }
}
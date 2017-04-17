using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class Comunicazione : System.Web.UI.UserControl
    {
        private static readonly ILog log = LogManager.GetLogger("UCComunicazione");

        //public event EventHandler<UCControlsEventArgs> UCComunicazioneViewChanged;

        private string comunicazione = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string TestoComunicazione
        {
            get
            {
                return comunicazione;
            }
            set
            {
                comunicazione = value;
                ShowComunicazione();
            }
        }



        public void ShowComunicazione()
        {
            //  string tipoComunicazione = lbltipoComunicazione.Text;   
            string msg = string.Empty;
            if (comunicazione != string.Empty)
            {
                msg = comunicazione;
                // Session[tipoComunicazione] = msg;
                // TestoCom.Text = msg;
                HtmlControl HtmlEditor = (HtmlControl)Page.FindControl(this.ClientID + "_Html");

                // HtmlEditorExt.Content = msg;
            }
        }

        public string GetComunicazione()
        {
            HtmlControl HtmlEditor = (HtmlControl)Page.FindControl(this.ClientID + "_divTextArea");
            return divTextArea.InnerText;
        }
    }
}
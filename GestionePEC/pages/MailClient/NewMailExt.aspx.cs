using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Mail.MailMessage;
using Com.Delta.Web;
using Com.Delta.Web.Session;
using GestionePEC.Extensions;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.pages.MailClient
{
    public partial class NewMailExt : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                string test = HidHtml.Value;
                if (!(string.IsNullOrEmpty(test)))
                {
                    SessionManager<string>.set(SessionKeys.TESTO_MAIL, test);
                }
            }
            else
            {
                SessionManager<string>.del(SessionKeys.TESTO_MAIL);
            }
        }

        private static readonly ILog log = LogManager.GetLogger("NewMailExt");

        private string _comunicazione;

        public string Comunicazione
        {
            get
            {
                if (SessionManager<string>.exist(SessionKeys.TESTO_MAIL))
                {
                    string html = string.Empty;
                    if (HidHtml.Value != string.Empty)
                    {
                        html = HidHtml.Value;
                        SessionManager<string>.set(SessionKeys.TESTO_MAIL, html);
                    }
                    else
                    {
                        html = SessionManager<string>.get(SessionKeys.TESTO_MAIL);
                    }
                    try
                    {
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        string htmlView = html.Replace("'", "''");
                        HidHtml.Value = htmlView;
                        doc.LoadHtml(htmlView);
                        if (doc.ParseErrors.Count() == 0)
                        {
                            var n = (from node in doc.DocumentNode.Descendants()
                                     where node.Name.Equals("body", StringComparison.InvariantCultureIgnoreCase)
                                     select node.FirstChild).SingleOrDefault();
                            if (n != null)
                                html = n.InnerHtml;
                        }
                    }
                    catch
                    {
                    }
                    return html;

                }
                else
                {
                    return _comunicazione;
                }
            }
            set
            {
                string htmlView = value.Replace("'", "''");
                _comunicazione = htmlView;
            }
        }
      
    }
}
using SendMail.BusinessEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class HeaderNewMail : System.Web.UI.UserControl
    {
        public string TitoloSelected
        {
            get { return ddlTitolo.SelectedItem.Value; }
        }

        public string SottoTitoloSelected
        {
            get { return ddlSottotitolo.SelectedItem.Value; }
        }

        public string DestinatarioSelected
        {
            get { return TOMail.Text; }
        }

        public string ConoscenzaSelected
        {
            get { return CCMail.Text; }
        }

        public string BCCSelected
        {
            get { return BCCMail.Text; }
        }

        public string ObjectSelected
        {
            get { return ObjectMail.Text; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //revMail.ValidationExpression = RegexUtils.EMAIL_REGEX;
            //RegularExpressionValidator1.ValidationExpression = RegexUtils.EMAIL_REGEX;
            //RegularExpressionValidator2.ValidationExpression = RegexUtils.EMAIL_REGEX;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                TitolarioService<SendMail.Model.Titolo> ts = new TitolarioService<SendMail.Model.Titolo>();
                IList<SendMail.Model.Titolo> titoli = ts.GetAll(null);
                ddlTitolo.DataTextField = "Nome";
                ddlTitolo.DataValueField = "Id";
                titoli.RemoveAt(0);
                ddlTitolo.DataSource = titoli;
                ddlTitolo.DataBind();
                this.ddlTitolo.Items.Insert(0, "-- Selezionare un titolo --");

                //Controllo per disabilitare l'invio nascosto in caso di PEC
                if (HttpContext.Current.Session["MAIL_ACCOUNT"] != null)
                    if ((HttpContext.Current.Session["MAIL_ACCOUNT"] as ActiveUp.Net.Mail.DeltaExt.MailUser).Dominus.Equals("PEC"))
                    {
                        trBCC.Visible = false;
                        BCCMail.Text = string.Empty;
                        BCCMail.ReadOnly = true;
                    }

            }

        }

        protected void OnChangeIndex_ddlTitolo(object sender, EventArgs e)
        {
            TitolarioService<SendMail.Model.SottoTitolo> ts = new TitolarioService<SendMail.Model.SottoTitolo>();
            if (ddlTitolo.SelectedItem.Value != "-- Selezionare un titolo --")
            {
                IList<SendMail.Model.SottoTitolo> sottotitoli = ts.FindByTitolo(int.Parse(ddlTitolo.SelectedItem.Value));
                ddlSottotitolo.DataTextField = "Nome";
                ddlSottotitolo.DataValueField = "Id";
                ddlSottotitolo.DataSource = sottotitoli;
                ddlSottotitolo.DataBind();
            }


        }
    }
}
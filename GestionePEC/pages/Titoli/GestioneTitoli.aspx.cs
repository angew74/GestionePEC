using GestionePEC.Extensions;
using SendMail.BusinessEF;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Web.UI;


namespace GestionePEC.pages.Titoli
{
    public partial class GestioneTitoli : BasePage
    {
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
            }

        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            TitolarioService<SendMail.Model.Titolo> ts = new TitolarioService<SendMail.Model.Titolo>();
            Titolo titolo = new Titolo();
            if (CodiceApplicazione.Text.Trim() != string.Empty && TitoloNome.Text.Trim() != string.Empty)
            {
                titolo.AppCode = CodiceApplicazione.Text;
                titolo.CodiceProtocollo = CodiceProtocollo.Text;
                titolo.Nome = TitoloNome.Text;
                titolo.Deleted = !(TitoloIsActive.Checked);
                try
                {
                    ts.insertTitolo(titolo);
                }
                catch (Exception ex)
                {
                    info.AddError("Creazione titolo errata dettagli: " + ex.Message);
                    return;
                }
                info.AddInfo("Titolo creato");
                IList<SendMail.Model.Titolo> titoli = ts.GetAll(null);
                ddlTitolo.DataTextField = "Nome";
                ddlTitolo.DataValueField = "Id";
                titoli.RemoveAt(0);
                ddlTitolo.DataSource = titoli;
                ddlTitolo.DataBind();
                this.ddlTitolo.Items.Insert(0, "-- Selezionare un titolo --");
            }
            else
            {
                info.AddError("Inserire i campi obbligatori");
            }

        }

        protected void buttonSalvaSottoTitolo_Click(object sender, EventArgs e)
        {
            if (SottoTitoloCodiceComunicazione.Text.Trim() != string.Empty && NomeSottotitolo.Text.Trim() != string.Empty && ddlTitolo.SelectedValue != "0")
            {
                TitolarioService<SendMail.Model.SottoTitolo> st = new TitolarioService<SendMail.Model.SottoTitolo>();
                TitolarioService<SendMail.Model.Titolo> ts = new TitolarioService<SendMail.Model.Titolo>();
                Titolo titolo = ts.LoadTitoloById(int.Parse(ddlTitolo.SelectedValue));
                SottoTitolo sottotitolo = new SottoTitolo(titolo);
                SottoTitolo s = null;
                sottotitolo.ComCode = SottoTitoloCodiceComunicazione.Text;
                sottotitolo.ProtocolloCode = SottoTitoloCodiceProtocollo.Text;
                sottotitolo.Nome = NomeSottotitolo.Text;
                sottotitolo.RefIdTitolo = titolo.Id;
                sottotitolo.UsaProtocollo = CheckProtocolloAttivo.Checked;
                sottotitolo.Deleted = CheckProtocolloAttivo.Checked;
                try
                {
                    s = st.insertTitolo(sottotitolo);
                }
                catch (Exception ex)
                {
                    info.AddError("Creazione sottotitolo errata dettagli: " + ex.Message);
                }
                if (titolo.Id > 0)
                {
                    info.AddInfo("Titolo creato");
                }
            }
        }
    }
}
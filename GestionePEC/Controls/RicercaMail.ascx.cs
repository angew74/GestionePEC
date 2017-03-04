using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Extensions;
using SendMail.Business;
using SendMail.Locator;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.util;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class RicercaMail : System.Web.UI.UserControl
    {
        public class MailSelectedEventArgs : EventArgs
        {
            public string UId { get; set; }
            public string CurrentAccount { get; set; }
            public string CurrentFolder { get; set; }
            public string ParentFolder { get; set; }
            public string Dimension { get; set; }
            public string CurrRating { get; set; }


            public MailSelectedEventArgs(string uid, string currAccount, string currFolder, string parFolder, string dimension, string rating)
            {
                this.UId = uid;
                this.CurrentAccount = currAccount;
                this.CurrentFolder = currFolder;
                this.Dimension = dimension;
                this.ParentFolder = parFolder;
                this.CurrRating = rating;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event MailHandler SelectMail;

        public delegate void MailHandler(MailSelectedEventArgs e);

        private const string STR_FIRST_PAGE = "0";

        public event EventHandler HideMail;

        #region Metodi Utili

        protected virtual void Redirect(string path, RouteValueDictionary routeDictionary)
        {
            var virtualPath = (this.Page as BasePage).VirtualPath(path, routeDictionary);
            Response.Redirect(virtualPath, true);
        }

        private string getCellValue(int riga, int colonna)
        {
            string s = this.gridBox.Rows[riga].Cells[colonna].Text;
            if (s != "&nbsp;") return s;
            else return "";
        }

        private enum col
        {
            IdMail,
            Utente,
            From,
            To,
            Subject,
            DataInvio,
            Dimension,
            Folder,
            Tipo,
            FolderId,
            Status,
            Select

        }

        private string getCellControlValue(int ARowIndex, int AColumnIndex)
        {
            string sResult = string.Empty;

            if ((ARowIndex > -1) && (ARowIndex < gridBox.Rows.Count) && (AColumnIndex > -1) && (AColumnIndex < gridBox.Columns.Count))
            {
                TableCell tc = gridBox.Rows[ARowIndex].Cells[AColumnIndex];

                Label lbl = null;
                if (tc.Controls.Count > 1)
                    lbl = tc.Controls[1] as Label;

                if (lbl != null)
                    sResult = lbl.Text;
                else
                    sResult = tc.Text;

                if (sResult == "&nbsp;")
                    sResult = string.Empty;
            }

            return sResult;
        }

        #endregion


        #region Caricamento Interfaccia



        protected void ddlManagedAccounts_DataBinding(object sender, EventArgs e)
        {
            string username = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;           
            List<MailUser> l = SessionManager<List<MailUser>>.get(SessionKeys.ACCOUNTS_LIST);
            if (!(l != null && l.Count != 0))
            {
                l = ServiceLocator.GetServiceFactory().getMailServerConfigFacade().GetManagedAccountByUser(username).ToList();
                if (l == null) l = new List<MailUser>();
                if (l.Where(x => x.UserId.Equals(-1)).Count() == 0)
                    l.Insert(0, new MailUser() { UserId = -1, EmailAddress = "" });
                SessionManager<List<MailUser>>.set(SessionKeys.ACCOUNTS_LIST, l);
            }            
            DropDownList ddl = sender as DropDownList;
            ddl.DataSource = l;
            ddl.DataTextField = "EmailAddress";
            ddl.DataValueField = "UserId";
            UpdAccounts.Update();
        }

        protected void ddlManagedAccounts_DataBound(object sender, EventArgs e)
        {
            //login automatico per unico account mail

            DropDownList ddl = sender as DropDownList;
            if (ddl.Items.Cast<ListItem>().Where(x => !x.Value.Equals("-1")).Count() == 1)
            {
                ddl.Items[1].Selected = true;
                ddlManagedAccounts_SelectedIndexChanged(ddl, EventArgs.Empty);
            }

        }

        protected void ddlManagedAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = sender as DropDownList;
            decimal userId = decimal.Parse(ddl.SelectedValue);
            if (userId != -1)
            {
                try
                {
                    CartellaAccess();
                    UtentiAccess();
                }
                catch
                {
                    ((BasePage)this.Page).info.AddError("Connessione al mail server impossibile, controllare le credenziali");
                }
            }

        }


        private void LoadTitoli()
        {
            ITitolarioService<SendMail.Model.Titolo> ts = ServiceLocator.GetServiceFactory().TitolarioService<SendMail.Model.Titolo>();
            IList<SendMail.Model.Titolo> titoli = ts.GetAll(null);
            ddlTitolo.DataTextField = "Nome";
            ddlTitolo.DataValueField = "Id";
            titoli.RemoveAt(0);
            ddlTitolo.DataSource = titoli;
            ddlTitolo.DataBind();
            ListItem item = new ListItem();
            item.Value = "";
            item.Text = "-- Selezionare un titolo --";
            item.Selected = true;
            this.ddlTitolo.Items.Insert(0, item);
            UpdTitolo.Update();
        }

        protected void rblTipoFolder_Changed(object sender, EventArgs e)
        {
            CartellaAccess();
        }

        protected void rblIOBox_Changed(object sender, EventArgs e)
        {

            switch (rblIOBox.SelectedValue)
            {
                case "I":
                    tbMail.Text = "Mittente";
                    UpdMail.Update();
                    ddlSottotitolo.Enabled = false;
                    ddlTitolo.Enabled = false;
                    ddlStatus.Enabled = false;
                    ddlMarcatori.Enabled = true;
                    ddlInboxStato.Enabled = true;
                    LoadTitoli();
                    break;
                case "O":
                    tbMail.Text = "Destinatario";
                    UpdMail.Update();
                    ddlSottotitolo.Enabled = true;
                    ddlTitolo.Enabled = true;
                    ddlMarcatori.Enabled = false;
                    ddlStatus.Enabled = true;
                    ddlInboxStato.Enabled = false;
                    break;
            }
            UpdStatus.Update();
            UpdMarcatori.Update();
            UpdTitolo.Update();
            UpdSottotitolo.Update();
            UpdInbox.Update();
            UpdMail.Update();
            CartellaAccess();
        }


        protected void OnChangeIndex_ddlTitolo(object sender, EventArgs e)
        {
            ITitolarioService<SendMail.Model.SottoTitolo> ts = ServiceLocator.GetServiceFactory().TitolarioService<SendMail.Model.SottoTitolo>();
            if (ddlTitolo.SelectedItem.Value != "-- Selezionare un titolo --" && ddlTitolo.SelectedItem.Value != string.Empty)
            {
                IList<SendMail.Model.SottoTitolo> sottotitoli = ts.FindByTitolo(int.Parse(ddlTitolo.SelectedItem.Value));
                ddlSottotitolo.DataTextField = "Nome";
                ddlSottotitolo.DataValueField = "Id";
                ddlSottotitolo.DataSource = sottotitoli;
                ddlSottotitolo.DataBind();
                ListItem item = new ListItem();
                item.Value = "";
                item.Text = "-- Selezionare un sottotitolo --";
                item.Selected = true;
                ddlSottotitolo.Items.Insert(0, item);
                UpdSottotitolo.Update();
            }


        }

        private void UtentiAccess()
        {
            if (ddlManagedAccounts.SelectedValue != null && ddlManagedAccounts.SelectedValue != string.Empty && ddlManagedAccounts.SelectedValue != "-1")
            {

                List<BackendUser> listaDipendentiAbilitati = ServiceLocator.GetServiceFactory().BackendUserService.GetDipendentiDipartimentoAbilitati(decimal.Parse(ddlManagedAccounts.SelectedValue));
                if (listaDipendentiAbilitati != null)
                {
                    ddlUtente.DataValueField = "UserId";
                    ddlUtente.DataTextField = "UserName";
                    ddlUtente.DataSource = listaDipendentiAbilitati.OrderBy(x => x.UserName);
                    ddlUtente.DataBind();
                    ListItem item = new ListItem();
                    item.Value = "";
                    item.Text = "-- Selezionare un Utente --";
                    item.Selected = true;
                    this.ddlUtente.Items.Insert(0, item);
                    UpdUtente.Update();
                }
            }
        }

        private void CartellaAccess()
        {
            if (ddlManagedAccounts.SelectedValue != null && ddlManagedAccounts.SelectedValue != string.Empty && ddlManagedAccounts.SelectedValue != "-1")
            {

                List<Folder> list = new List<Folder>();
                string io = rblIOBox.SelectedValue;
                string m = "E";
                string f = "I";
                if (io == "I")
                {
                    switch (rblTipoFolder.SelectedValue)
                    {
                        case "O":
                            f = "I";
                            m = "I";
                            break;
                        case "A":
                            f = "A";
                            m = "E";
                            break;
                        case "C":
                            f = "C";
                            m = "I";
                            break;
                    }
                }
                else
                {
                    switch (rblTipoFolder.SelectedValue)
                    {
                        case "O":
                            f = "O";
                            m = "O";
                            break;
                        case "A":
                            f = "A";
                            m = "D";
                            break;
                        case "C":
                            f = "C";
                            m = "O";
                            break;
                    }
                }
                list = ServiceLocator.GetServiceFactory().MailLocalService.getFoldersByAccount(decimal.Parse(ddlManagedAccounts.SelectedValue)).Where(x => x.TipoFolder == f || x.TipoFolder == m).ToList();
                if (f == "C" && m == "I")
                { list = list.Where(x => x.TipoFolder == f && (x.IdNome == "1" || x.IdNome == "3")).ToList(); }
                if (f == "C" && m == "O")
                { list = list.Where(x => x.TipoFolder == f && x.IdNome == "2").ToList(); }
                ddlCartellaSposta.DataTextField = ddlCartella.DataTextField = "Nome";
                ddlCartellaSposta.DataValueField = ddlCartella.DataValueField = "Id";
                ddlCartellaSposta.DataSource = ddlCartella.DataSource = list;
                ddlCartella.DataBind();
                ddlCartellaSposta.DataBind();
                ListItem item = new ListItem();
                ListItem itema = new ListItem();
                item.Value = "0";
                item.Text = "-- Tutte --";
                item.Selected = true;
                itema.Value = "";
                itema.Text = "Effettuare una scelta";
                itema.Selected = true;
                ddlCartella.Items.Insert(0, item);
                ddlCartellaSposta.Items.Insert(0, itema);
                UpdCartella.Update();
                UpdCartellaSposta.Update();
            }

        }


        #endregion

        #region Eventi Pagina

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                hdTabIndex.Value = "0";
                ddlManagedAccounts.DataBind();
                LoadTitoli();
                ddlTitolo.Enabled = false;
                ddlSottotitolo.Enabled = false;
                ddlStatus.Enabled = false;
                ddlMarcatori.Enabled = false;
                dtInizio.Giorno = dtFine.Giorno = System.DateTime.Now.Day.ToString().PadLeft(2, '0');
                dtInizio.Mese = dtFine.Mese = System.DateTime.Now.Month.ToString().PadLeft(2, '0');
                dtInizio.Anno = dtFine.Anno = System.DateTime.Now.Year.ToString();

            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
           // ScriptManager.RegisterStartupScript(this, this.GetType(), "UCRicercaMailJS", BuildScript(), true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnClickRicerca(object sender, EventArgs e)
        {
            Search(STR_FIRST_PAGE);
            // NotifyDataReady(null);
        }

        protected void OnPagerIndexChanged(string sPaginaRichiesta, int pag)
        {
            int da = (pag * int.Parse(Properties.Settings.Default.ListaRisultatiPerPagina)) + 1;
            int per = int.Parse(Properties.Settings.Default.ListaRisultatiPerPagina);
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx = getDictionaryChoice();
            ResultList<MailHeaderExtended> result = ServiceLocator.GetServiceFactory().MailLocalService.GetMailsGridByParams(ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, rblIOBox.SelectedValue, rblTipoFolder.SelectedValue, idx, da, per);
            gridBox.DataSource = result.List;
            gridBox.DataBind();
            gridBox.BottomPagerRow.Visible = true;
            ((Paging)gridBox.BottomPagerRow.Controls[0].Controls[1]).configureControl((pag + 1).ToString(), Properties.Settings.Default.ListaRisultatiPerPagina.ToString(), result.Totale.ToString());
        }

        protected void btnStampa_Click(object sender, EventArgs e)
        {
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx = getDictionaryChoice();
            ResultList<MailHeaderExtended> result = ServiceLocator.GetServiceFactory().MailLocalService.GetMailsGridByParams(ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, rblIOBox.SelectedValue, rblTipoFolder.SelectedValue, idx, 1, 1000);
            byte[] b = Helpers.StampaEmailAttoITEXT(result.List.ToList(), ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedItem.Text, dtInizio.DateString(), dtFine.DateString(), rblIOBox.SelectedValue, ddlManagedAccounts.SelectedValue);
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=DistintaMail_" + ddlManagedAccounts.SelectedItem.Text + ".pdf");
            Response.OutputStream.Write(b, 0, b.Length);
            Response.End();

        }

        private Dictionary<MailTypeSearch, string> getDictionaryChoice()
        {
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx.Add(MailTypeSearch.Utente, ddlUtente.SelectedItem.Text);
            idx.Add(MailTypeSearch.TipoBox, rblIOBox.SelectedValue);
            if (rblIOBox.SelectedValue == "O")
            {
                idx.Add(MailTypeSearch.SottoTitolo, ddlSottotitolo.SelectedValue);
                idx.Add(MailTypeSearch.Titolo, ddlTitolo.SelectedValue);
                idx.Add(MailTypeSearch.Status, ddlStatus.SelectedValue);
            }
            else
            {
                idx.Add(MailTypeSearch.Marcatori, ddlMarcatori.SelectedValue);
                idx.Add(MailTypeSearch.StatusInbox, ddlInboxStato.SelectedValue);
            }
            idx.Add(MailTypeSearch.Mail, tbMittente.Text);
            idx.Add(MailTypeSearch.Oggetto, tbOggetto.Text);
            idx.Add(MailTypeSearch.DataInzio, dtInizio.DateString());
            idx.Add(MailTypeSearch.DataFine, dtFine.DateString());
            return idx;
        }

        protected void btnLetta_Click(object sender, EventArgs e)
        {
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx = getDictionaryChoice();
            try
            {
                bool ok = ServiceLocator.GetServiceFactory().MailLocalService.UpdateAllMails(MailStatus.LETTA, ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, MySecurityProvider.CurrentPrincipal.Identity.Name, idx);
                if (ok == true)
                {
                    (this.Page as BasePage).info.AddMessage("Aggiornamento effettuato", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                }
            }
            catch (Exception ex)
            {
                (this.Page as BasePage).info.AddMessage("Attenzione mails non aggiornate", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }
        }

        protected void btnUnLetta_Click(object sender, EventArgs e)
        {
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx = getDictionaryChoice();
            try
            {
                bool ok = ServiceLocator.GetServiceFactory().MailLocalService.UpdateAllMails(MailStatus.SCARICATA, ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, MySecurityProvider.CurrentPrincipal.Identity.Name, idx);
                if (ok == true)
                {
                    (this.Page as BasePage).info.AddMessage("Aggiornamento effettuato", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                }
            }
            catch (Exception ex)
            {
                (this.Page as BasePage).info.AddMessage("Attenzione mails non aggiornate", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }
        }

        protected void btnSposta_Click(object sender, EventArgs e)
        {
            pnlSposta.Visible = true;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx = getDictionaryChoice();
            try
            {
                decimal idaccount = decimal.Parse(ddlManagedAccounts.SelectedValue);
                bool ok = ServiceLocator.GetServiceFactory().MailLocalService.MoveAllMails(ddlManagedAccounts.SelectedItem.Text, idaccount, ddlCartellaSposta.SelectedValue, ddlCartella.SelectedValue, MySecurityProvider.CurrentPrincipal.Identity.Name, rblIOBox.SelectedValue, idx);
                if (ok == true)
                {
                    (this.Page as BasePage).info.AddMessage("Spostamento effettuato", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                }
            }
            catch (Exception ex)
            {
                (this.Page as BasePage).info.AddMessage("Attenzione mails non spostate", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }

        }

        #endregion

        #region Private

        private string BuildScript()
        {
            StringBuilder sb = new StringBuilder();
           // sb.Append(GetTabIndexIDScript());
           // sb.Append(GetActivateTabScript());
           // sb.Append(GetActivateTabChangedScript());
           // sb.Append(GetCreateTabsScript());
           // sb.Append(GetValidatePageScript());
            return sb.ToString();
        }


        private void Search(string APageIndex)
        {
            int index = int.Parse(hdTabIndex.Value.ToString());
            switch (index)
            {
                case 0:
                    SearchByParams(APageIndex);
                    break;
            }
        }

        private void SearchByParams(string APageIndex)
        {
            btnStampaDistinta.Visible = false;
            ddlCartellaSposta.Visible = false;
            pnlSposta.Visible = btnSposta.Visible = false;
            btnUnLetta.Visible = false;
            btnLetta.Visible = false;
            HideMail(null, null);
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx = getDictionaryChoice();
            if (ddlManagedAccounts.SelectedItem.Text != string.Empty && ddlCartella.SelectedValue != string.Empty &&
                rblIOBox.SelectedValue != string.Empty && rblTipoFolder.SelectedValue != string.Empty)
            {

                ResultList<MailHeaderExtended> result = ServiceLocator.GetServiceFactory().MailLocalService.GetMailsGridByParams(ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, rblIOBox.SelectedValue, rblTipoFolder.SelectedValue, idx, 1, 5);
                gridBox.DataSource = result.List;
                gridBox.DataBind();
                if (result.List.Count > 0)
                {
                    btnStampaDistinta.Visible = true;
                    ddlCartellaSposta.Visible = true;
                    btnSposta.Visible = true;
                    if (rblIOBox.SelectedValue == "I")
                    {
                        if (ddlInboxStato.SelectedValue == "1")
                        {
                            btnLetta.Visible = false;
                            btnUnLetta.Visible = true;
                        }
                        if (ddlInboxStato.SelectedValue == "0")
                        {
                            btnLetta.Visible = true;
                            btnUnLetta.Visible = false;
                        }
                        else if (ddlInboxStato.SelectedValue != "1" && ddlInboxStato.SelectedValue != "0")
                        {
                            btnLetta.Visible = false;
                            btnUnLetta.Visible = false;
                        }
                    }
                    else
                    {
                        btnLetta.Visible = false;
                        btnUnLetta.Visible = false;
                    }
                    gridBox.BottomPagerRow.Controls[0].Controls[1].Visible = true;
                    gridBox.BottomPagerRow.Visible = true;
                    int resPag = int.Parse(Properties.Settings.Default.ListaRisultatiPerPagina);
                    ((Paging)gridBox.BottomPagerRow.Controls[0].Controls[1]).configureControl("1", resPag.ToString(), result.Totale.ToString());
                }
            }
            else { (this.Page as BasePage).info.AddMessage("Selezionare tutti i campi obbligatori", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO); }
        }


        private string GetCurrentValidationGroup()
        {
            string sResult = "vgTabCasella";
            int i = 0;
            int.TryParse(hdTabIndex.Value.ToString(), out i);
            switch (i)
            {
                case 0:
                    sResult = "vgTabCasella";
                    break;
            }

            return sResult;
        }

        #endregion

        #region GridView

        protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int riga = Convert.ToInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                default:
                    string idmail = getCellControlValue(riga, (int)col.IdMail);
                    string dim = getCellControlValue(riga, (int)col.Dimension);
                    string folderid = getCellControlValue(riga, (int)col.FolderId);
                    int rating = (int)Enum.Parse(typeof(MailStatus), getCellControlValue(riga, (int)col.Status));
                    MailSelectedEventArgs ex = new MailSelectedEventArgs(idmail, ddlManagedAccounts.SelectedValue, folderid, rblIOBox.SelectedValue, dim, rating.ToString());
                    SelectMail(ex);
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Grid_DataBound(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Grid_RowEditing(object sender, GridViewEditEventArgs e)
        {
        }

        protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblFrom = (Label)e.Row.FindControl("lblFrom");
                string tooltipFrom = lblFrom.Text;
                Label lblTo = (Label)e.Row.FindControl("lblTo");
                string tooltipTo = lblTo.Text;
                Label lblSubject = (Label)e.Row.FindControl("lblSubject");
                string tooltipSubject = lblSubject.Text;
                e.Row.Cells[2].Attributes.Add("title", tooltipFrom);
                e.Row.Cells[3].Attributes.Add("title", tooltipTo);
                e.Row.Cells[4].Attributes.Add("title", tooltipSubject);
                Label lblFolder = (Label)e.Row.FindControl("lblFolder");
                Label lblTipo = (Label)e.Row.FindControl("lblTipo");
                if (lblTipo.Text == "I")
                {
                    gridBox.Columns[3].Visible = false;
                }
                else
                {
                    gridBox.Columns[2].Visible = false;
                }

            }
        }


        #endregion

        #region Script

        //private string GetCreateTabsScript()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("Ext.onReady(function(){");
        //    sb.Append("var ResearchTabs = new Ext.TabPanel({");
        //    sb.Append(string.Format("renderTo: '{0}',", pnlTabContainer.ClientID));
        //    sb.Append(string.Format("activeTab: {0},", hdTabIndex.Value.ToString()));
        //    sb.Append("plain:true,");
        //    sb.Append("defaults:{autoHeight: true, autoWidth: true, autoScroll: true},");
        //    sb.Append("items:[");
        //    sb.Append("{contentEl:'" + pnlCasella.ClientID + "', title: 'Casella Mail', listeners: {activate: RIAhandleActivate}}");
        //    sb.Append("],");
        //    sb.Append("listeners: {'tabchange': RIAactiveTabChanged}");
        //    sb.Append("});");
        //    sb.Append("ResearchTabs.render();");
        //    sb.Append("});");

        //    return sb.ToString();
        //}


        //private string GetActivateTabScript()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("function RIAhandleActivate(tab)");
        //    sb.Append("{");
        //    sb.Append(" var pnl = document.getElementById(tab.contentEl);");
        //    sb.Append(" if (pnl != null)");
        //    sb.Append(" {");
        //    sb.Append("     pnl.style.display = 'block';");
        //    sb.Append(" }");
        //    sb.Append("}");

        //    return sb.ToString();
        //}


        //private string GetActivateTabChangedScript()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("function RIAactiveTabChanged(tab, tabPanel)");
        //    sb.Append("{");
        //    /*[LBONI]: 
        //     * azzera il riferimento al controllo non valido sul quale settare il focus
        //     * in quanto l'utente si è spostato di tab.
        //     * evita la comparsa di un errore js per control.focus() 
        //     * su di un controllo non accessibile.
        //    */
        //    sb.Append(" Page_InvalidControlToBeFocused = null;");
        //    sb.Append(" var index = 0;");
        //    sb.Append(" switch (tabPanel.title)");
        //    sb.Append(" {");
        //    sb.Append("     case 'Casella Mail':");
        //    sb.Append("         RIAValidationGroup = 'vgTabCodiceIndiv';");
        //    sb.Append("         index = 0;");
        //    sb.Append("         break;");
        //    sb.Append(" }");
        //    sb.Append(" var tabIndex = document.getElementById(hdTabIndexID);");
        //    sb.Append(" if (tabIndex != null)");
        //    sb.Append("     tabIndex.value = index;");
        //    sb.Append("}");

        //    return sb.ToString();
        //}


        //private string GetValidatePageScript()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    //sb.Append("var RIAValidationGroup = 'vgTabCodiceIndiv';");
        //    sb.Append(string.Format("var RIAValidationGroup = '{0}';", GetCurrentValidationGroup()));

        //    sb.Append("function RIAValidatePage(sender, event)");
        //    sb.Append("{");
        //    sb.Append(" var fResult = false;");
        //    sb.Append(" if (RIAValidationGroup != '')");
        //    sb.Append(" {");
        //    sb.Append("     Page_ClientValidate(RIAValidationGroup);");
        //    sb.Append("     fResult = Page_IsValid;");
        //    sb.Append("     if (!fResult)");
        //    sb.Append("         Page_BlockSubmit = false;");
        //    sb.Append(" }");
        //    sb.Append(" return fResult;");
        //    sb.Append("}");

        //    return sb.ToString();
        //}


        //private string GetTabIndexIDScript()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append(string.Format("var hdTabIndexID ='{0}';", hdTabIndex.ClientID));

        //    return sb.ToString();
        //}


        #endregion
    }
}
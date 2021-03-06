﻿using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Extensions;
using SendMail.BusinessEF;
using SendMail.BusinessEF.MailFacedes;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.UI.WebControls;
using static GestionePEC.Controls.RicercaMail;

namespace GestionePEC.Controls
{
    public partial class MailMoveResearch : System.Web.UI.UserControl
    {

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
            MailServerConfigFacade mscf = MailServerConfigFacade.GetInstance();
            if (!(l != null && l.Count != 0))
            {

                l = mscf.GetManagedAccountByUser(username).ToList();
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

        protected void rblTipoFolder_Changed(object sender, EventArgs e)
        {
            CartellaAccess();
        }

        protected void rblIOBox_Changed(object sender, EventArgs e)
        {
            switch (rblIOBox.SelectedValue)
            {
                case "I":
                    break;
                case "O":
                    break;
            }
            CartellaAccess();
        }

        private void UtentiAccess()
        {
            BackendUserService buservice = new BackendUserService();
            if (ddlManagedAccounts.SelectedValue != null && ddlManagedAccounts.SelectedValue != string.Empty && ddlManagedAccounts.SelectedValue != "-1")
            {
                List<BackendUser> listaDipendentiAbilitati = null;
                if (SessionManager<List<BackendUser>>.exist(SessionKeys.UTENTIBACKEND))
                {
                    listaDipendentiAbilitati = SessionManager<List<BackendUser>>.get(SessionKeys.UTENTIBACKEND);
                }
                else
                {
                    listaDipendentiAbilitati = buservice.GetDipendentiDipartimentoAbilitati(decimal.Parse(ddlManagedAccounts.SelectedValue));
                }
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
            MailLocalService mailLocalService = new MailLocalService();
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
                if (SessionManager<List<Folder>>.exist(SessionKeys.FOLDERS))
                {
                    list = SessionManager<List<Folder>>.get(SessionKeys.FOLDERS);
                }
                else
                {
                    list = mailLocalService.getFoldersByAccount(decimal.Parse(ddlManagedAccounts.SelectedValue)).Where(x => x.TipoFolder == f || x.TipoFolder == m).ToList();
                }
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
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "UCRicercaMailMoveJS", BuildScript(), true);
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
            MailLocalService mailLocalService = new MailLocalService();
            int da = (pag * int.Parse(Properties.Settings.Default.ListaRisultatiPerPagina)) + 1;
            int per = int.Parse(Properties.Settings.Default.ListaRisultatiPerPagina);
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx = getDictionaryChoice();
            ResultList<MailHeaderExtended> result = mailLocalService.GetMailsMoveGridByParams(ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, rblIOBox.SelectedValue, rblTipoFolder.SelectedValue, idx, chkUfficio.Checked, da, per);
            gridBox.DataSource = result.List;
            gridBox.DataBind();
            gridBox.BottomPagerRow.Visible = true;
            ((Paging)gridBox.BottomPagerRow.Controls[0].Controls[1]).configureControl((pag + 1).ToString(), Properties.Settings.Default.ListaRisultatiPerPagina.ToString(), result.Totale.ToString());
        }

        protected void btnStampa_Click(object sender, EventArgs e)
        {
            MailLocalService mailLocalService = new MailLocalService();
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx = getDictionaryChoice();
            ResultList<MailHeaderExtended> result = mailLocalService.GetMailsMoveGridByParams(ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, rblIOBox.SelectedValue, rblTipoFolder.SelectedValue, idx, chkUfficio.Checked, 1, 1000);
            byte[] b = Helpers.StampaEmailAttoITEXT(result.List.ToList(), ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedItem.Text, dtInizio.DateString(), dtFine.DateString(), rblIOBox.SelectedValue, ddlManagedAccounts.SelectedValue);
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=DistintaMail_" + ddlManagedAccounts.SelectedItem.Text + ".pdf");
            Response.OutputStream.Write(b, 0, b.Length);
            Response.End();

        }

        protected void btnLetta_Click(object sender, EventArgs e)
        {
            MailLocalService mailLocalService = new MailLocalService();
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx = getDictionaryChoice();
            try
            {
                bool ok = mailLocalService.UpdateAllMails(MailStatus.LETTA, ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, MySecurityProvider.CurrentPrincipal.MyIdentity.UserName, idx);
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
            MailLocalService mailLocalService = new MailLocalService();
            idx = getDictionaryChoice();
            try
            {
                bool ok = mailLocalService.UpdateAllMails(MailStatus.SCARICATA, ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, MySecurityProvider.CurrentPrincipal.MyIdentity.UserName, idx);
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

        private Dictionary<MailTypeSearch, string> getDictionaryChoice()
        {
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            idx.Add(MailTypeSearch.Utente, ddlUtente.SelectedItem.Text);
            idx.Add(MailTypeSearch.TipoBox, rblIOBox.SelectedValue);
            idx.Add(MailTypeSearch.DataInzio, dtInizio.ToIsoFormat());
            System.DateTime df = (System.DateTime)dtFine.Date();
            idx.Add(MailTypeSearch.DataFine, df.AddDays(1).ToString("yyyyMMdd"));
            return idx;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            Dictionary<MailTypeSearch, string> idx = new Dictionary<MailTypeSearch, string>();
            MailLocalService mailLocalService = new MailLocalService();
            idx = getDictionaryChoice();
            try
            {
                decimal idaccount = decimal.Parse(ddlManagedAccounts.SelectedValue);
                bool ok = mailLocalService.MoveAllMails(ddlManagedAccounts.SelectedItem.Text, idaccount, ddlCartellaSposta.SelectedValue, ddlCartella.SelectedValue, MySecurityProvider.CurrentPrincipal.MyIdentity.UserName, rblIOBox.SelectedValue, idx);
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
            MailLocalService mailLocalService = new MailLocalService();
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
                int da = (0 * int.Parse(Properties.Settings.Default.ListaRisultatiPerPagina)) + 1;
                int per = int.Parse(Properties.Settings.Default.ListaRisultatiPerPagina);
                ResultList<MailHeaderExtended> result = mailLocalService.GetMailsMoveGridByParams(ddlManagedAccounts.SelectedItem.Text, ddlCartella.SelectedValue, rblIOBox.SelectedValue, rblTipoFolder.SelectedValue, idx, chkUfficio.Checked, da, 6);
                gridBox.DataSource = result.List;
                gridBox.DataBind();
                if (result.List.Count > 0)
                {
                    btnStampaDistinta.Visible = true;
                    ddlCartellaSposta.Visible = true;
                    btnSposta.Visible = true;
                    if (rblIOBox.SelectedValue == "I")
                    {
                        btnLetta.Visible = true;
                        btnUnLetta.Visible = true;
                    }
                    else
                    {
                        btnLetta.Visible = false;
                        btnUnLetta.Visible = false;
                    }
                    gridBox.BottomPagerRow.Controls[0].Controls[1].Visible = true;
                    int resPag = int.Parse(Properties.Settings.Default.ListaRisultatiPerPagina);
                    ((Paging)gridBox.BottomPagerRow.Controls[0].Controls[1]).configureControl("1", resPag.ToString(), result.Totale.ToString());
                }
            }
            else { (this.Page as BasePage).info.AddMessage("Selezionare tutti i campi obbligatori", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO); }
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

     
    }
}
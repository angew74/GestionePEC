using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Extensions;
using SendMail.BusinessEF;
using SendMail.BusinessEF.MailFacedes;
using SendMail.Model;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class RicercaStat : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                hdTabIndex.Value = "0";
                dtInizio.Giorno = dtFine.Giorno = System.DateTime.Now.Day.ToString().PadLeft(2, '0');
                dtInizio.Mese = dtFine.Mese = System.DateTime.Now.Month.ToString().PadLeft(2, '0');
                dtInizio.Anno = dtFine.Anno = System.DateTime.Now.Year.ToString();
                ddlManagedAccounts.DataBind();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

           // ScriptManager.RegisterStartupScript(this, this.GetType(), "UCStatisticaJS", BuildScript(), true);
        }

        //private string BuildScript()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(GetTabIndexIDScript());
        //    sb.Append(GetActivateTabScript());
        //    sb.Append(GetActivateTabChangedScript());
        //    sb.Append(GetCreateTabsScript());
        //    sb.Append(GetValidatePageScript());
        //    return sb.ToString();
        //}

        protected void btnStampa_Statistica(object sender, EventArgs e)
        {
            string account = ddlManagedAccounts.SelectedItem.Text;
            string utente = ddlUtente.SelectedItem.Text;
            BackendUserService bus = new BackendUserService();
            List<UserResultItem> list = bus.GetStatsInBox(account, utente, dtInizio.ToIsoFormat(), dtFine.ToIsoFormat());
            DataTable xlWorkSheet = Helpers.StampaStatisticaExcel(list, account, dtInizio.DateString(), dtFine.DateString());
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Statistica" + account + ".xls"));
            Response.ContentType = "application/ms-excel";
            string str = string.Empty;
            foreach (DataColumn dtcol in xlWorkSheet.Columns)
            {
                Response.Write(str + dtcol.ColumnName);
                str = "\t";
            }
            Response.Write("\n");
            foreach (DataRow dr in xlWorkSheet.Rows)
            {
                str = "";
                for (int j = 0; j < xlWorkSheet.Columns.Count; j++)
                {
                    Response.Write(str + Convert.ToString(dr[j]));
                    str = "\t";
                }
                Response.Write("\n");
            }
            Response.End();

        }

        protected void OnPagerIndexChanged(string sPaginaRichiesta, int pag)
        {
        }

        protected void OnClickRicerca(object sender, EventArgs e)
        {
            try
            {
                string account = ddlManagedAccounts.SelectedItem.Text;
                BackendUserService bus = new BackendUserService();
                string utente = ddlUtente.SelectedItem.Text;
                List<UserResultItem> list = bus.GetStatsInBox(account, utente, dtInizio.ToIsoFormat(), dtFine.ToIsoFormat());
                gridStat.DataSource = list;
                gridStat.DataBind();
                btnStampaStatistica.Visible = true;
                PanelTotale.Visible = true;
                string tot = bus.GetTotalePeriodoAccount(account, dtInizio.DateString(), dtFine.DateString());
                totPeriodo.Text = "Totale email ricevute nel periodo per casella : " + tot;

            }
            catch (Exception ex)
            {

                (this.Page as BasePage).info.AddMessage("Attenzione errore nella generazione della statistica", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }

        }


        #region Caricamento interfaccia

        protected void ddlManagedAccounts_DataBinding(object sender, EventArgs e)
        {
            string username = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;
            List<MailUser> l = SessionManager<List<MailUser>>.get(SessionKeys.ACCOUNTS_LIST);
            if (!(l != null && l.Count != 0))
            {
                MailServerConfigFacade serverFacade = MailServerConfigFacade.GetInstance();
                l = serverFacade.GetManagedAccountByUser(username).ToList();
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
                    UtentiAccess();
                }
                catch
                {
                    ((BasePage)this.Page).info.AddError("Connessione al mail server impossibile, controllare le credenziali");
                }
            }

        }

        private void UtentiAccess()
        {
            if (ddlManagedAccounts.SelectedValue != null && ddlManagedAccounts.SelectedValue != string.Empty && ddlManagedAccounts.SelectedValue != "-1")
            {
                BackendUserService bus = new BackendUserService();
                List<BackendUser> listaDipendentiAbilitati = bus.GetDipendentiDipartimentoAbilitati(decimal.Parse(ddlManagedAccounts.SelectedValue));
                if (listaDipendentiAbilitati != null)
                {
                    ddlUtente.DataValueField = "UserId";
                    ddlUtente.DataTextField = "UserName";
                    ddlUtente.DataSource = listaDipendentiAbilitati.OrderBy(x => x.UserName);
                    ddlUtente.DataBind();
                    ListItem item = new ListItem();
                    item.Value = "";
                    item.Text = "TUTTI";
                    item.Selected = true;
                    this.ddlUtente.Items.Insert(0, item);
                    UpdUtente.Update();
                }
            }
        }

        #endregion
            

    }
}
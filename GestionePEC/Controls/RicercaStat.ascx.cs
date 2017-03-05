using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Extensions;
using SendMail.Locator;
using SendMail.Model;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
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
            List<UserResultItem> list = ServiceLocator.GetServiceFactory().BackendUserService.GetStatsInBox(account, utente, dtInizio.DateString(), dtFine.DateString());
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
                string utente = ddlUtente.SelectedItem.Text;
                List<UserResultItem> list = ServiceLocator.GetServiceFactory().BackendUserService.GetStatsInBox(account, utente, dtInizio.DateString(), dtFine.DateString());
                gridStat.DataSource = list;
                gridStat.DataBind();
                btnStampaStatistica.Visible = true;
                PanelTotale.Visible = true;
                string tot = ServiceLocator.GetServiceFactory().BackendUserService.GetTotalePeriodoAccount(account, dtInizio.DateString(), dtFine.DateString());
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

                List<BackendUser> listaDipendentiAbilitati = ServiceLocator.GetServiceFactory().BackendUserService.GetDipendentiDipartimentoAbilitati(decimal.Parse(ddlManagedAccounts.SelectedValue));
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
        //    sb.Append("{contentEl:'" + pnlStatistica.ClientID + "', title: 'Statistica Lavorazioni', listeners: {activate: RIAhandleActivate}}");
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
        //    sb.Append("     case 'Statistica Lavorazioni':");
        //    sb.Append("         RIAValidationGroup = 'vgTabStatistica';");
        //    sb.Append("         index = 0;");
        //    sb.Append("         break;");
        //    sb.Append(" }");
        //    sb.Append(" var tabIndex = document.getElementById(hdTabIndexID);");
        //    sb.Append(" if (tabIndex != null)");
        //    sb.Append("     tabIndex.value = index;");
        //    sb.Append("}");

        //    return sb.ToString();
        //}

        //private string GetCurrentValidationGroup()
        //{
        //    string sResult = "vgTabStatistica";
        //    int i = 0;
        //    int.TryParse(hdTabIndex.Value.ToString(), out i);
        //    switch (i)
        //    {
        //        case 0:
        //            sResult = "vgTabStatistica";
        //            break;
        //    }

        //    return sResult;
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
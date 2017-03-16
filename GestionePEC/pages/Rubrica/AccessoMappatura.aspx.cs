using GestionePEC.Controls;
using GestionePEC.Extensions;
using SendMail.Locator;
using SendMail.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.pages.Rubrica
{
    public partial class AccessoMappatura : BasePage
    {
        public List<BackEndRefCode> listadescr = new List<BackEndRefCode>();
        public List<BackEndRefCode> listadescrupd = new List<BackEndRefCode>();
        public BackEndRefCode appoggio;
        public BackEndRefCode codInser;
        public ICollection<BackEndRefCode> codeBk = null;
        public ICollection<BackEndRefCode> codeent = null;
        public ICollection<BackEndRefCode> codeId = null;
        public ICollection<BackEndRefCode> idA = null;
        public BackEndRefCode entity = null;
        public SendMail.Model.BackEndRefCode t = null;
        public SendMail.Business.Contracts.IBackEndDictionaryService s = null;
        public BackEndRefCode sF = null;
        public List<BackEndRefCode> bk = new List<BackEndRefCode>();
        public string descBeck;
        public string codback;
        public BackEndRefCode id;
        private Paging ucpag = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlGrid.Visible = false;
                AccessoView.Visible = false;
                // PanelRicerca.Visible = false;
                PanelInsDettRic.Visible = false;
                //  AccessoFormView.ChangeMode(FormViewMode.Insert);
                //  AccessoFormView.Visible = true;


            }
            else
            {
                pnlGrid.Visible = true;
                griBackend.Visible = true;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            btnSearch.Enabled = true;
            base.OnPreRender(e);
        }

        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            griBackend.EditIndex = e.NewEditIndex;
            // this.BindGrid();
            griBackend.Columns[2].Visible = true;
        }

        //funzione per visualizzare i dati dell'ente ricercato
        private void GetGridResult()
        {

            if ((codiceback.Text.Length == 0) && (descrizioneback.Text.Length == 0))
            {
                this.info.AddMessage("Attenzione: Immettere Codice Backend o la Descrizione", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                griBackend.Visible = false;
                AccessoView.Visible = false;
                pnlGrid.Visible = false;
                PanelInsDettRic.Visible = false;
                //PanelRicerca.Visible = false;
                return;
            }

            if ((codiceback.Text.Length > 0) && (descrizioneback.Text.Length > 0))
            {
                griBackend.Visible = false;
                AccessoView.Visible = false;
                pnlGrid.Visible = false;
                PanelInsDettRic.Visible = false;
                // PanelRicerca.Visible = false;
                this.info.AddMessage("Attenzione: Immettere Codice Backend o la Descrizione", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                return;
            }

            if (codiceback.Text.Length > 0)
            {

                AccessoView.Visible = false;
                //  Session["Accessopag"] = null; 

                s = ServiceLocator.GetServiceFactory().BackEndDictionaryService;

                codeBk = new List<BackEndRefCode>();

                appoggio = s.GetByCode(codiceback.Text);
                if (appoggio == null)
                {
                    this.info.AddMessage("Attenzione: Codice non presente", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    griBackend.Visible = false;
                    pnlGrid.Visible = false;
                    AccessoView.Visible = false;
                    return;
                }

                codeBk.Add(appoggio);
                griBackend.Visible = true;
                griBackend.DataSource = codeBk;

                Session["Accesso"] = codeBk;
                griBackend.DataBind();
                griBackend.Visible = true;
            }
            else
            {
                codeBk = null;
            }

            if (descrizioneback.Text.Length > 0)
            {
                AccessoView.Visible = false;

                s = ServiceLocator.GetServiceFactory().BackEndDictionaryService;
                listadescr = s.GetByDescr(descrizioneback.Text);

                if (listadescr == null)
                {
                    this.info.AddMessage("Attenzione: Descrizione non presente", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    griBackend.Visible = false;
                    pnlGrid.Visible = false;
                    AccessoView.Visible = false;
                    return;
                }

                griBackend.DataSource = listadescr;

                Session["Accesso"] = listadescr;

                griBackend.DataBind();
                griBackend.Visible = true;

                OnAccessoPagerIndexChanged("", 0);

                if (listadescr.Count <= PagerSize)
                    griBackend.BottomPagerRow.Visible = false;

                //  Session["Accessopag"] = null;

            }
            else if (codiceback.Text.Length == 0)
            {
                listadescr = null;
            }
        }

        #region Proprietà

        private const int GRID_PAGER = 5;

        public int PagerSize
        {
            get { return GRID_PAGER; }
        }

        private long? hfCurrentID
        { get; set; }
       

        public string PnlGridClientID
        {
            get
            {
                return pnlGrid.ClientID;
            }
        }       

        private FormViewMode CurrentFvState
        { get; set; }

        #endregion

        #region Evento

        public void DataAccessoView(decimal Id)
        {
            if (Id > 0)
            {
                codeId = (List<BackEndRefCode>)Session["Accesso"];
                var ci = codeId.Where(q => q.Id == Id);
                if (ci.Count() != 1) return;
                else
                {
                    AccessoView.DataSource = ci;
                    Session["AccessoId"] = ci.First();
                    AccessoView.DataBind();
                    AccessoView.Visible = true;
                }
            }
            else AccessoView.Visible = false;
        }

        //click sul record dei backend
        protected void BkAccesso_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            PanelInsDettRic.Visible = true;

            AccessoView.ChangeMode(FormViewMode.Edit);
            int nrow = Convert.ToInt32(e.CommandArgument);
            var hfId = griBackend.Rows[nrow].Cells[0].Controls.OfType<HiddenField>().SingleOrDefault();

            if (hfId != null)
            {
                long Id1 = Convert.ToInt64((hfId as HiddenField).Value);
                DataAccessoView(Id1);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //  PanelRicerca.Visible = true;
            pnlGrid.Visible = true;
            PanelInsDettRic.Visible = false;
            GetGridResult();
        }

        protected void btnIns_Click(object sender, EventArgs e)
        {
            PanelInsDettRic.Visible = true;
            AccessoView.ChangeMode(FormViewMode.Insert);
            AccessoView.Visible = true;
            if (descrizioneback.Text.Length == 0)
            {
                griBackend.Visible = false;
                pnlGrid.Visible = false;
            }
            else
            {

                s = ServiceLocator.GetServiceFactory().BackEndDictionaryService;
                listadescr = s.GetByDescr(descrizioneback.Text);

                if (listadescr == null)
                {
                    pnlGrid.Visible = false;
                }
            }

        }

        protected void odsBackend_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void odsBackend_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void AccessoView_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Insert"))
            {
                BackEndRefCode entity = new BackEndRefCode();
                try
                {                   

                    if ((((TextBox)AccessoView.FindControl("TextCode")).Text != null) && (((TextBox)AccessoView.FindControl("TextCode")).Text != ""))
                    {

                        s = ServiceLocator.GetServiceFactory().BackEndDictionaryService;
                        codInser = s.GetByCode(((TextBox)AccessoView.FindControl("TextCode")).Text);
                        if (codInser != null)
                        {
                            this.info.AddMessage("Attenzione: codice backend e' legato ad un altro accesso", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                            return;
                        }

                        entity.Codice = ((TextBox)AccessoView.FindControl("TextCode")).Text;

                    }
                    else
                    {
                        this.info.AddMessage("Attenzione: codice backend e' obbligatorio", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                        return;
                    }

                    entity.Descrizione = ((TextBox)AccessoView.FindControl("TextDescrizione")).Text;
                    entity.Categoria = ((TextBox)AccessoView.FindControl("TextCategoria")).Text;
                    entity.DescrizionePlus = ((TextBox)AccessoView.FindControl("TextDescrizionePlus")).Text;


                }

                catch
                {
                    this.info.AddMessage("Attenzione: si è verificato un errore", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return;
                }

                if (entity.Descrizione == "")
                    entity.Descrizione = null;
                if (entity.Categoria == "")
                    entity.Categoria = null;
                if (entity.DescrizionePlus == "")
                    entity.DescrizionePlus = null;

                try
                {

                    ServiceLocator.GetServiceFactory().BackEndDictionaryService.Insert(entity);

                    if (descrizioneback.Text.Length > 0)
                    {
                        s = ServiceLocator.GetServiceFactory().BackEndDictionaryService;
                        listadescr = s.GetByDescr(descrizioneback.Text);

                        if (listadescr != null)
                        {

                            //griBackend.DataSource = listadescr;

                            Session["Accesso"] = listadescr;
                            if (listadescr.Count > 0)
                            {
                                int index = listadescr.IndexOf(listadescr.First(x => x.Codice == entity.Codice));
                                OnAccessoPagerIndexChanged("", index / PagerSize);

                                // OnAccessoPagerIndexChanged("", 0);
                                if (listadescr.Count <= PagerSize)
                                    griBackend.BottomPagerRow.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        griBackend.Visible = false;
                        pnlGrid.Visible = false;
                    }
                }
                catch (Exception)
                {
                    this.info.AddMessage("Inserimento non riuscito: Errore in banca dati", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return;
                }
                this.info.AddMessage("Accesso inserito", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                //  griBackend.Visible = false;
                //  AccessoView.Visible = false;
                this.AccessoView.DataSource = null;
                this.AccessoView.DataBind();

            }
            else
            {

                // BackEndRefCode entity = (BackEndRefCode)Session["AccessoId"];

                entity = (BackEndRefCode)Session["AccessoId"];

                int idCnt = Int32.Parse(((HiddenField)AccessoView.FindControl("hfIdAccesso")).Value);

                if (entity == null) return;

                if (entity.Id == idCnt)
                {

                    if (e.CommandName.Equals("Update"))
                    {
                        try
                        {

                            AccessoView.ChangeMode(FormViewMode.Edit);

                            //((TextBox)AccessoView.FindControl("TextCode")); 
                            if ((((TextBox)AccessoView.FindControl("TextCode")).Text != null) && (((TextBox)AccessoView.FindControl("TextCode")).Text != ""))
                            {
                                s = ServiceLocator.GetServiceFactory().BackEndDictionaryService;
                                codInser = s.GetByCode(((TextBox)AccessoView.FindControl("TextCode")).Text);
                                if ((codInser != null) && (entity.Id != codInser.Id))
                                {
                                    this.info.AddMessage("Attenzione: codice backend e' legato ad un altro accesso", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                                    return;
                                }

                                entity.Codice = ((TextBox)AccessoView.FindControl("TextCode")).Text;

                            }
                            else
                            {
                                this.info.AddMessage("Attenzione: codice backend e' obbligatorio", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                                return;
                            }

                            entity.Descrizione = ((TextBox)AccessoView.FindControl("TextDescrizione")).Text;
                            entity.Categoria = ((TextBox)AccessoView.FindControl("TextCategoria")).Text;
                            entity.DescrizionePlus = ((TextBox)AccessoView.FindControl("TextDescrizionePlus")).Text;

                            List<BackEndRefCode> bkEnt = (List<BackEndRefCode>)Session["Accesso"];
                            int index = bkEnt.IndexOf(bkEnt.First(x => x.Id == idCnt));
                            OnAccessoPagerIndexChanged("", index / PagerSize);

                            if (bkEnt.Count <= PagerSize)
                                griBackend.BottomPagerRow.Visible = false;

                        }
                        catch
                        {
                            this.info.AddMessage("Attenzione: si è verificato un errore", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                            return;
                        }
                    }

                    if (e.CommandName.Equals("Delete"))
                    {
                        try
                        {
                            //metodo per l'update
                            ServiceLocator.GetServiceFactory().BackEndDictionaryService.Delete(idCnt);
                            // List<BackEndRefCode> bkEnt = (List<BackEndRefCode>)Session["Accesso"];
                            // int index = bkEnt.IndexOf(bkEnt.First(x => x.Id != idCnt));
                            // OnAccessoPagerIndexChanged("", index / PagerSize);
                            if (descrizioneback.Text.Length > 0)
                            {
                                s = ServiceLocator.GetServiceFactory().BackEndDictionaryService;
                                listadescr = s.GetByDescr(descrizioneback.Text);

                                if (listadescr != null)
                                {

                                    //griBackend.DataSource = listadescr;

                                    Session["Accesso"] = listadescr;
                                    OnAccessoPagerIndexChanged("", 0);
                                    if (listadescr.Count <= PagerSize)
                                        griBackend.BottomPagerRow.Visible = false;
                                }
                            }
                            else
                            {
                                griBackend.Visible = false;
                                pnlGrid.Visible = false;
                            }

                            // if (bkEnt.Count <= PagerSize)
                            //     griBackend.BottomPagerRow.Visible = false;
                        }
                        catch (Exception)
                        {
                            this.info.AddMessage("Cancellazione non riuscita: Errore in banca dati", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                            return;
                        }
                        this.info.AddMessage("Accesso cancellato", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                        PanelInsDettRic.Visible = false;
                        // griBackend.Visible = false;
                        AccessoView.Visible = false;

                    }
                }

                if (e.CommandName.Equals("Update"))
                {

                    if (entity.Descrizione == "")
                        entity.Descrizione = null;
                    if (entity.Categoria == "")
                        entity.Categoria = null;
                    if (entity.DescrizionePlus == "")
                        entity.DescrizionePlus = null;

                    try
                    {
                        //metodo per l'update
                        ServiceLocator.GetServiceFactory().BackEndDictionaryService.Update(entity);
                    }
                    catch (Exception)
                    {
                        this.info.AddMessage("Aggiornamento non riuscito: Errore in banca dati", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                        return;
                    }
                    this.info.AddMessage("Accesso modificato", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);

                    //   griBackend.Visible = true;
                    //   griBackend.DataSource = listadescrupd;
                    //   griBackend.DataBind();
                    PanelInsDettRic.Visible = false;
                    AccessoView.Visible = false;
                }
            }
        }
      

        protected void AccessoView_ItemInserting(object sender, FormViewInsertEventArgs e)
        {

        }

        protected void AccessoFormView_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {

        }

        protected void AccessoFormView_ModeChanging(object sender, FormViewModeEventArgs e)
        {

        }

        protected void ContactsFormView_DataBound(object sender, EventArgs e)
        {

        }

        public string CheckString(object AValue)
        {
            return (AValue != null) ? AValue.ToString().Trim() : "";
        }

        protected void odsBackend_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = t;
        }

        protected void odsBackend_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void AccessoView_ItemDeleting(object sender, FormViewDeleteEventArgs e)
        {
            //  throw new NotImplementedException();
        }

        protected void AccessoView_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            //evento obbligatorio, senza va in errore
            foreach (DictionaryEntry entry in e.NewValues)
            {
                if (entry.Value.ToString().Equals(""))
                {
                    e.NewValues[entry.Key] = null;
                }
            }
        }

        protected void ucAccessoPaging_Init(object sender, EventArgs e)
        {
            ucpag = (Paging)sender;
        }

        //click sulla ddl per la paginazione dei contatti (fa il databind della gridview)
        protected void OnAccessoPagerIndexChanged(string s, int pag)
        {
            int da = pag + 1;

            List<BackEndRefCode> bkEnt = (List<BackEndRefCode>)Session["Accesso"];
            griBackend.DataSource = bkEnt.Skip(pag * PagerSize).Take(PagerSize).ToList();
            griBackend.DataBind();
            griBackend.BottomPagerRow.Visible = true;
            ucpag.Visible = true;
            ucpag.configureControl(da.ToString(), PagerSize.ToString(), bkEnt.Count.ToString());
            // Session["Accessopag"] = pag;
        }

        #endregion
    }
}

using Com.Delta.Web;
using GestionePEC.Extensions;
using SendMail.Business.Contracts;
using SendMail.Locator;
using SendMail.Model;
using SendMail.Model.RubricaMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class EntiViewer : System.Web.UI.UserControl
    {

        #region "Properties"
        private const int GRID_PAGER = 5;

        public int PagerSize
        {
            get { return GRID_PAGER; }
        }

        private FormViewMode CurrentFvState
        { get; set; }

        private long? hfCurrentID
        { get; set; }

        public string OrgDen
        {
            get { return (string)this.ViewState["OrgDen"]; }
            set { this.ViewState["OrgDen"] = value; }
        }

        public RubricaEntita EntFormViewDataSource
        {
            get
            {
                return Session["EntSession"] as RubricaEntita;
            }
            set
            {
                Session["EntSession"] = value;
                EntFormView.ChangeMode(FormViewMode.ReadOnly);
                GetGridResult();
            }
        }

        public string TxtCodFisClientID
        {
            get
            {
                Control c = EntFormView.FindControl("TxtCodFis");
                if (c != null) return c.ClientID;
                else return "";
            }
        }
        #endregion

        #region "Page Events"

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FormView1.Visible = false;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            headerBut.DataBind();
            ContactsFormView.ChangeMode(CurrentFvState);
            switch (CurrentFvState)
            {
                case FormViewMode.Edit:
                    //DataContactView(hfCurrentID);
                    break;
                case FormViewMode.Insert:
                    break;
            }

        }


        #endregion


        //funzione per visualizzare i dati dell'ente ricercato
        private void GetGridResult()
        {
            CurrentFvState = FormViewMode.Edit;

            //metto l'oggetto in sessione
            if (EntFormViewDataSource == null) return;

            if (EntFormViewDataSource.RefOrg.HasValue)
            {
                IRubricaEntitaService S = ServiceLocator.GetServiceFactory().RubricaEntitaService;
                RubricaEntita DataOrg = S.GetRubricaEntitaCompleteById(EntFormViewDataSource.RefOrg.Value);
                OrgDen = DataOrg.RagioneSociale;
            }
            else OrgDen = null;

            EntFormView.DataBind();
            FormView1.Visible = true;

            if (EntFormViewDataSource.Contatti == null)
            {
                List<RubricaContatti> lrc = new List<RubricaContatti>();
                EntFormViewDataSource.Contatti = lrc;
            }

            switch (EntFormViewDataSource.Contatti.Count)
            {
                case 0:
                    gvContacts.Visible = true;
                    ContactsFormView.Visible = true;

                    gvContacts.DataBind();

                    ContactsFormView.DataBind();
                    break;
                //case 1:
                //hfCurrentID = EntFormViewDataSource.Contatti[0].IdContact;
                //DataContactView(EntFormViewDataSource.Contatti[0].IdContact);

                //    gvContacts.Visible = true;
                //    ContactsFormView.Visible = true;
                //    break;
                default:
                    gvContacts.Visible = true;
                    ContactsFormView.Visible = false;

                    OnContactsPagerIndexChanged("", 0);

                    if (EntFormViewDataSource.Contatti.Count <= PagerSize)
                        gvContacts.BottomPagerRow.Visible = false;
                    break;
            }
        }

        //metodo per il binding dei contatti
        public void DataContactView(long? idContact)
        {
            if (idContact.HasValue && idContact > 0)
            {
                //rubEnt = (RubricaEntita)Session["EntSession"];
                RubricaEntita rubEnt = EntFormViewDataSource;

                List<RubricaContatti> l1 = new List<RubricaContatti>();
                l1.Add(rubEnt.Contatti.SingleOrDefault(x => x.IdContact == Convert.ToInt32(idContact)));

                ContactsFormView.DataSource = l1;
                ContactsFormView.DataBind();

                ContactsFormView.Visible = true;
            }
            else ContactsFormView.Visible = false;
        }

        //metodo per il controllo dei contatti già presenti
        public bool ContactControls(long? TidContact)
        {
            RubricaEntita TrubEnt = Session["EntSession"] as RubricaEntita;

            if (!string.IsNullOrEmpty(Convert.ToString(TidContact))) //nel caso di update
            {
                var lCont = TrubEnt.Contatti.Where(x => x.IdContact != TidContact);

                if (lCont.Any(x => x.Mail == (ContactsFormView.FindControl("TextMail") as TextBox).Text) && (ContactsFormView.FindControl("TextMail") as TextBox).Text != string.Empty)
                {
                    ((BasePage)this.Page).info.AddMessage("Operazione impossibile: Email già presente nei contatti", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return false;
                }

                if (lCont.Any(x => x.Telefono == (ContactsFormView.FindControl("TextTelefono") as TextBox).Text) && (ContactsFormView.FindControl("TextTelefono") as TextBox).Text != string.Empty)
                {
                    ((BasePage)this.Page).info.AddMessage("Operazione impossibile: Telefono già presente nei contatti", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return false;
                }

                if (lCont.Any(x => x.Fax == (ContactsFormView.FindControl("TextFax") as TextBox).Text) && (ContactsFormView.FindControl("TextFax") as TextBox).Text != string.Empty)
                {
                    ((BasePage)this.Page).info.AddMessage("Operazione impossibile: Fax già presente nei contatti", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return false;
                }
            }

            else  //nel caso di insert
            {

                if (TrubEnt.Contatti.Exists(x => x.Mail == (ContactsFormView.FindControl("TextMail") as TextBox).Text) && (ContactsFormView.FindControl("TextMail") as TextBox).Text != string.Empty)
                {
                    ((BasePage)this.Page).info.AddMessage("Operazione impossibile: Email già presente nei contatti", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return false;
                }

                if (TrubEnt.Contatti.Exists(x => x.Telefono == (ContactsFormView.FindControl("TextTelefono") as TextBox).Text) && (ContactsFormView.FindControl("TextTelefono") as TextBox).Text != string.Empty)
                {
                    ((BasePage)this.Page).info.AddMessage("Operazione impossibile: Telefono già presente nei contatti", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return false;
                }

                if (TrubEnt.Contatti.Exists(x => x.Fax == (ContactsFormView.FindControl("TextFax") as TextBox).Text) && (ContactsFormView.FindControl("TextFax") as TextBox).Text != string.Empty)
                {
                    ((BasePage)this.Page).info.AddMessage("Operazione impossibile: Fax già presente nei contatti", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return false;
                }
            }
            return true;
        }


        #region Eventi

        protected void revMail_Init(object sender, EventArgs e)
        {
            (sender as RegularExpressionValidator).ValidationExpression = RegexUtils.EMAIL_REGEX;
        }

        //click sul record dei contatti
        protected void gvContacts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            CurrentFvState = FormViewMode.Edit;
            int nrow = Convert.ToInt32(e.CommandArgument);
            var hfId = gvContacts.Rows[nrow].Cells[0].Controls.OfType<HiddenField>().SingleOrDefault();
            if (hfId != null)
            {
                long idContact = Convert.ToInt64((hfId as HiddenField).Value);
                DataContactView(idContact);
                hfCurrentID = idContact;
            }
        }

        private Paging ucpag = null;

        protected void ucContactsPaging_Init(object sender, EventArgs e)
        {
            ucpag = (Paging)sender;
        }

        //click sulla ddl per la paginazione dei contatti (fa il databind della gridview)
        protected void OnContactsPagerIndexChanged(string s, int pag)
        {
            int da = pag + 1;
            RubricaEntita rubEnt = (RubricaEntita)Session["EntSession"];
            gvContacts.Visible = true;
            gvContacts.DataSource = rubEnt.Contatti.Skip(pag * PagerSize).Take(PagerSize).ToList();

            gvContacts.DataBind();
            gvContacts.BottomPagerRow.Visible = true;
            ucpag.configureControl(da.ToString(), PagerSize.ToString(), rubEnt.Contatti.Count.ToString());
        }

        /// <summary>
        /// Switch to edit mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ibEntitaEdit_Click(object sender, ImageClickEventArgs e)
        {
            EntFormView.ChangeMode(FormViewMode.Edit);
        }

        //click sul bottone di aggiornamento dell'ente
        protected void EntFormView_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            if (Page.IsValid == false) return;
        }

        protected void EntFormView_ItemUpdating(object sender, FormViewUpdateEventArgs e)
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

        protected void odsEntita_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = EntFormViewDataSource;
        }

        protected void odsEntita_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            RubricaEntita rubEnt = e.InputParameters[0] as RubricaEntita;
            //metodo per l'update
            ServiceLocator.GetServiceFactory().RubricaEntitaService.Update(rubEnt);
            EntFormViewDataSource = ServiceLocator.GetServiceFactory().RubricaEntitaService.GetRubricaEntitaCompleteById(rubEnt.IdReferral.Value);
            ((BasePage)this.Page).info.AddMessage("Ente modificato", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void custCodFisValidate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string cFis = args.Value;
            switch (cFis.Length)
            {
                case 11:      //codice provvisorio
                    CodFisProvvCheck CodFisProvvCheck1 = new CodFisProvvCheck();
                    args.IsValid = CodFisProvvCheck1.CFprovvCheck(cFis);
                    break;
                case 16:        //codice standard
                    CodFisCheck CodFisCheck1 = new CodFisCheck();
                    args.IsValid = CodFisCheck1.controllaCorrettezza(cFis);
                    break;
                default:
                    args.IsValid = false;
                    break;
            }
        }

        //click sul bottone della formview dei contatti: gestisce update ed inserimento
        protected void ContactsFormView_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            RubricaEntita rubEnt = Session["EntSession"] as RubricaEntita;
            RubricaContatti contact = new RubricaContatti();


            #region "Gestione Inseret/update contatto"
            if (e.CommandName.Equals("Update"))
            {
                int idCnt = Int32.Parse(((HiddenField)ContactsFormView.FindControl("hfIdContact")).Value);
                contact = rubEnt.Contatti.SingleOrDefault(x => x.IdContact == idCnt);
            }


            if ((e.CommandName.Equals("Insert") && hidInsertType.Value.Equals("Insert_c")) || e.CommandName.Equals("Update"))
            {

                if ((e.CommandName.Equals("Insert"))) contact.RefIdReferral = rubEnt.IdReferral;

                //funzione per il controllo di inserimento di contatti già inseriti
                if (contact != null && ContactControls(contact.IdContact) == false)//caso in cui non supera i controlli
                {
                    if (e.CommandName == "Update")//ricarico i dati precedenti in caso di update
                    {
                        DataContactView(Int32.Parse(((HiddenField)ContactsFormView.FindControl("hfIdContact")).Value));
                    }
                    else //svuoto i campi in caso di insert
                    {
                        (ContactsFormView.FindControl("TextMail") as TextBox).Text = "";
                        (ContactsFormView.FindControl("TextTelefono") as TextBox).Text = "";
                        (ContactsFormView.FindControl("TextFax") as TextBox).Text = "";
                    }
                    return;
                }

                try
                {
                    contact.Telefono = ((TextBox)ContactsFormView.FindControl("TextTelefono")).Text;
                    contact.Mail = ((TextBox)ContactsFormView.FindControl("TextMail")).Text;
                    contact.Fax = ((TextBox)ContactsFormView.FindControl("TextFax")).Text;
                    contact.ContactRef = ((TextBox)ContactsFormView.FindControl("TextRef")).Text;
                    contact.Note = ((TextBox)ContactsFormView.FindControl("TextNote")).Text;
                    contact.IsPec = ((CheckBox)ContactsFormView.FindControl("chkPec")).Checked;

                    if (contact.IsPec == false)
                    {
                        foreach (MailPecForCheck val in Enum.GetValues(typeof(MailPecForCheck)))
                        {
                            if (contact.Mail.Contains("@" + val.ToString() + "."))
                                contact.IsPec = true;
                        }
                    }
                }
                catch
                {
                    ((BasePage)this.Page).info.AddMessage("Attenzione: si è verificato un errore", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return;
                }

                if (contact.Telefono == "" && contact.Mail == "" && contact.Fax == "")
                {
                    ((BasePage)this.Page).info.AddMessage("Inserire almeno un parametro", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return;
                }

                if (contact.Mail == "" && contact.IsPec == true)
                {
                    ((BasePage)this.Page).info.AddMessage("Inserire indirizzo email valido", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    contact.IsPec = false;
                    return;
                }

                if (contact.Telefono == "")
                    contact.Telefono = null;
                if (contact.Mail == "")
                    contact.Mail = null;
                if (contact.Fax == "")
                    contact.Fax = null;
                if (contact.ContactRef == "")
                    contact.ContactRef = null;
                if (contact.Note == "")
                    contact.Note = null;

                if (e.CommandName == "Update")
                {
                    try
                    {
                        //metodo per l'update
                        ServiceLocator.GetServiceFactory().ContattoService.UpdateRubrContatti(contact, false);
                    }
                    catch (Exception)
                    {
                        ((BasePage)this.Page).info.AddMessage("Aggiornamento non riuscito: Errore in banca dati", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                        return;
                    }
                    ((BasePage)this.Page).info.AddMessage("Contatto modificato", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                }
                else if (e.CommandName == "Insert")
                {
                    try
                    {
                        //metodo per l'insert
                        ServiceLocator.GetServiceFactory().ContattoService.InsertRubrContatti(contact, false);
                    }
                    catch (Exception)
                    {
                        ((BasePage)this.Page).info.AddMessage("Inserimento non riuscito: Errore in banca dati", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                        return;
                    }
                    ((BasePage)this.Page).info.AddMessage("Contatto inserito", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                }



            }
            #endregion


            #region "Insert di un nuovo gruppo o di un nuovo ufficio"

            if ((e.CommandName.Equals("Insert") && hidInsertType.Value.Equals("Insert_u")))
            {
                RubricaEntita newEnt = new RubricaEntita();

                newEnt.Ufficio = ((TextBox)ContactsFormView.FindControl("TextUfficio")).Text;
                newEnt.IdPadre = rubEnt.IdReferral;
                newEnt.RefOrg = (rubEnt.RefOrg == null ? rubEnt.IdReferral : rubEnt.RefOrg);
                if (rubEnt.ReferralType.ToString().StartsWith("PA"))
                    newEnt.ReferralType = EntitaType.PA_UFF;
                else if (rubEnt.ReferralType.ToString().StartsWith("AZ"))
                    newEnt.ReferralType = EntitaType.AZ_UFF;
                try
                {
                    //metodo per l'insert
                    ServiceLocator.GetServiceFactory().RubricaEntitaService.Insert(newEnt);
                }
                catch (Exception e0)
                {
                    ((BasePage)this.Page).info.AddMessage("Inserimento non riuscito: " + e0.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return;
                }
                ((BasePage)this.Page).info.AddMessage("Gruppo inserito", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "reload_local_tree", "ShowTree(config.tree);", true);
            }
            else if ((e.CommandName.Equals("Insert") && hidInsertType.Value.Equals("Insert_g")))
            {
                RubricaEntita newEnt = new RubricaEntita();

                newEnt.Ufficio = ((TextBox)ContactsFormView.FindControl("TextGruppo")).Text;
                newEnt.IdPadre = rubEnt.IdReferral;
                newEnt.RefOrg = (rubEnt.RefOrg == null ? rubEnt.IdReferral : rubEnt.RefOrg);
                if (rubEnt.ReferralType.ToString().StartsWith("PA"))
                    newEnt.ReferralType = EntitaType.PA_GRP;
                else if (rubEnt.ReferralType.ToString().StartsWith("AZ"))
                    newEnt.ReferralType = EntitaType.AZ_GRP;
                try
                {
                    //metodo per l'insert
                    ServiceLocator.GetServiceFactory().RubricaEntitaService.Insert(newEnt);
                }
                catch (Exception e1)
                {
                    ((BasePage)this.Page).info.AddMessage("Inserimento non riuscito: " + e1.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return;
                }
                ((BasePage)this.Page).info.AddMessage("Ufficio inserito", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "reload_local_tree", "ShowTree();", true);
            }


            #endregion
            hfCurrentID = contact.IdContact ?? -1;
            long idEntita = long.Parse((EntFormView.FindControl("hfIdEntita") as HiddenField).Value);
            EntFormViewDataSource = ServiceLocator.GetServiceFactory().RubricaEntitaService.GetRubricaEntitaCompleteById(idEntita);
            rubEnt = EntFormViewDataSource;
            switch (rubEnt.Contatti.Count)
            {
                case 0:
                    gvContacts.Visible = true;
                    gvContacts.DataBind();
                    ContactsFormView.DataBind();
                    break;
                default:
                    if (hfCurrentID == -1) OnContactsPagerIndexChanged("", 0);
                    else
                    {
                        int index = rubEnt.Contatti.IndexOf(rubEnt.Contatti.First(x => x.IdContact == hfCurrentID));
                        OnContactsPagerIndexChanged("", index / PagerSize);
                    }
                    if (rubEnt.Contatti.Count <= PagerSize)
                        gvContacts.BottomPagerRow.Visible = false;
                    break;
            }
            ContactsFormView.Visible = false;
        }

        protected void ContactsFormView_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {

        }

        protected void ContactsFormView_ItemInserting(object sender, FormViewInsertEventArgs e)
        {

        }

        protected void ContactsFormView_ModeChanging(object sender, FormViewModeEventArgs e)
        {

        }

        protected void ContactsFormView_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {

        }

        protected void ContactsFormView_DataBound(object sender, EventArgs e)
        {
            if (((FormView)sender).CurrentMode == FormViewMode.Insert)
            {

                if (hidInsertType.Value.Equals("Insert_u"))
                {
                    ((Panel)ContactsFormView.FindControl("panelGrp")).Visible = false;
                    ((Panel)ContactsFormView.FindControl("panelUff")).Visible = true;
                    ((Panel)ContactsFormView.FindControl("panelCon")).Visible = false;

                }
                else if (hidInsertType.Value.Equals("Insert_g"))
                {
                    ((Panel)ContactsFormView.FindControl("panelGrp")).Visible = true;
                    ((Panel)ContactsFormView.FindControl("panelUff")).Visible = false;
                    ((Panel)ContactsFormView.FindControl("panelCon")).Visible = false;

                }
                else
                {
                    ((Panel)ContactsFormView.FindControl("panelGrp")).Visible = false;
                    ((Panel)ContactsFormView.FindControl("panelUff")).Visible = false;
                    ((Panel)ContactsFormView.FindControl("panelCon")).Visible = true;

                }
            }
        }


        //click sul bottone per creare un contatto nuovo
        protected void ContactsFormView_InsertCommand(object sender, EventArgs e)
        {
            hidInsertType.Value = ((ImageButton)sender).CommandName;
            if (gvContacts.Rows.Count == 0)      //serve per nascondere la scritta
                gvContacts.Visible = false;      //dell'empty data template della gv

            RubricaEntita rubEnt = EntFormViewDataSource;
            if (rubEnt.Contatti.Count == 1)
            {
                OnContactsPagerIndexChanged("", 0);
                gvContacts.BottomPagerRow.Visible = false;
            }
            ContactsFormView.Visible = true;
            CurrentFvState = FormViewMode.Insert;
            ContactsFormView.DataBind();

        }

        #endregion
    }
}
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Security;
using GestionePEC.Extensions;
using log4net;
using SendMail.BusinessEF;
using SendMail.Model;
using SendMail.Model.RubricaMapping;
using SendMail.Model.WebserviceMappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace GestionePEC.pages.Rubrica
{
    public partial class EntiResearch : BasePage
    {

        private static readonly ILog log = LogManager.GetLogger("EntiResearch");
        [Newtonsoft.Json.JsonObject]
        internal class UserRoles
        {
            [Newtonsoft.Json.JsonProperty]
            internal List<string> roles { get; set; }
            [Newtonsoft.Json.JsonProperty]
            internal int dip { get; set; }
        }

        private string _roles;
        public string Roles
        {
            get
            {
                if (string.IsNullOrEmpty(_roles))
                {
                    UserRoles userRoles = new UserRoles();
                    userRoles.roles = (List<string>)MySecurityProvider.CurrentPrincipal.getRoles(MySecurityProvider.CurrentPrincipal.MyIdentity).Result;
                    if (!(string.IsNullOrEmpty(MySecurityProvider.CurrentPrincipal.MyIdentity.dipartimento)))
                    { userRoles.dip = int.Parse((MySecurityProvider.CurrentPrincipal.MyIdentity as MyIdentity).dipartimento); }
                    else { userRoles.dip = 152; }
                    _roles = Newtonsoft.Json.JsonConvert.SerializeObject(userRoles);
                }
                return _roles;
            }
        }

        private string _canali;
        public string Canali
        {
            get
            {
                if (string.IsNullOrEmpty(_canali))
                {
                    string[] canali = Enum.GetNames(typeof(TipoCanale));
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        System.Runtime.Serialization.Json.DataContractJsonSerializer ser =
                            new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(string[]));
                        ser.WriteObject(ms, canali);
                        ms.Position = 0;
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(ms))
                        {
                            _canali = sr.ReadToEnd();
                        }
                    }
                }
                return _canali;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlDettagli.Visible = false;
            }
            btnShowEntita.Style.Add(HtmlTextWriterStyle.Display, "none");
            btnTreeNode.Style.Add(HtmlTextWriterStyle.Display, "none");
        }

        protected void btnTreeNode_Click(object sender, EventArgs e)
        {
            long id = -1;
            Item item;
            try
            {
                item = Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(hfSelectedTreeNode.Value);
            }
            catch
            {
                return;
            }
            string nodeId = item.Id;
            if (nodeId.Contains('@'))
            {
                nodeId = nodeId.Substring(nodeId.IndexOf('@') + 1);
            }

            if (long.TryParse(nodeId, out id))
            {
                pnlDettagli.Visible = true;
                RubricaEntitaService rus = new RubricaEntitaService();
                RubricaEntita r = rus.GetRubricaEntitaCompleteById(id);               
                UCEntiViewer.EntFormViewDataSource = r;
                pnlDownPage.Update();
            }
        }

        protected void btnShowEntita_OnClick(object sender, EventArgs e)
        {
            pnlDettagli.Visible = true;      
            if (ucEntiSearcher.EnteSelected == null) return;
            Item item = ucEntiSearcher.EnteSelected;
            long id = default(long);
            string[] itemIds = item.Id.Split(';');            
            string itemId = itemIds[0].Split('#')[0];
            if (long.TryParse(itemId, out id))
            {
                try
                {
                    RubricaEntitaService rus = new RubricaEntitaService();
                    RubricaEntita r = rus.GetRubricaEntitaCompleteById(id);                  
                    UCEntiViewer.EntFormViewDataSource = r;
                }
                catch(Exception ex)
                {

                    if (ex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(ex.Message,
                            "ERR_125",
                            string.Empty,
                            string.Empty,
                            ex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                    }
                    pnlDettagli.Visible = false;//* ATT.947493
                    (this.Page as BasePage).info.AddMessage("Errore nel caricamento dell'entità. Riprovare", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }           

            pnlDownPage.Update();
        }
    }
}
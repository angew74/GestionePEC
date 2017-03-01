
using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.MailMessage;
using Com.Delta.Security;
using Delta.Utilities.LinqExtensions;
using GestionePEC.Extensions;
using log4net;
using SendMail.Locator;
using SendMail.Model;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Http;


namespace GestionePEC.Api
{
    public class PECController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PECController));
       
        [Serializable]
        internal class JSONTreeNode
        {
          
            internal string id { get; set; }
           
            internal string text { get; set; }
           
            internal bool leaf { get; set; }
          
            internal bool expanded { get; set; }
            
            internal JSONTreeNode[] data { get; set; }
           
            internal string icon { get; set; }
            
            internal string source { get; set; }
        }

       
        [Serializable]
        internal class JSONTreeNodeModel
        {
            
            internal List<JSONTreeNode> data;
            
            internal string Message;
           
            internal string TotalCount;
           
            internal string success;
        }

      
        [Serializable]
        internal class SubFilter<T>
        {
            
            private string tipo { get; set; }
           
            private string value { get; set; }

            internal MailIndexedSearch tipoSearch
            {
                get
                {
                    MailIndexedSearch mis = MailIndexedSearch.UNKNOWN;
                    int idx = -1;
                    if (!String.IsNullOrEmpty(tipo) && int.TryParse(tipo, out idx))
                    {
                        mis = (MailIndexedSearch)idx;
                    }
                    return mis;
                }
            }
            internal T[] values
            {
                get
                {
                    if (value != null)
                    {
                        string[] vals = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        T[] resp = new T[vals.Length];
                        Type ty = typeof(T);
                        TypeCode tc = Type.GetTypeCode(ty);
                        if (ty.IsPrimitive || tc == TypeCode.String)
                        {
                            resp = vals.Select(x => (T)Convert.ChangeType(x, typeof(T))).ToArray();
                        }
                        else if (ty.IsEnum)
                        {
                            resp = vals.Select(x => (T)Convert.ChangeType(x, Enum.GetUnderlyingType(ty))).ToArray();
                        }
                        return resp;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        
        [Serializable]
        internal class Filter
        {
           
            internal SubFilter<string> text { get; set; }
           
            internal SubFilter<MailStatus> status { get; set; }
        }

        
        [Serializable]
        internal class CarrierModel
        {
         
            internal List<Carrier> data;
          
            internal string Message;
           
            internal string TotalCount;
          
            internal string success;
        }


       
        [Serializable]
        internal class Carrier
        {
          
            internal string id;
            
            internal string from;
           
            internal string date;
           
            internal string subject;
          
            internal string sStatus;
          
            internal string mStatus;
           
            internal string attach;
            
            internal string utente;
            
            internal int dimen;           
        }

   


        [Authorize]
        [Route("api/PECController/GetTreeStructure")]
        public HttpResponseMessage GetTreeStructure(string idM)
        {
            JSONTreeNodeModel model = new JSONTreeNodeModel();
            if (string.IsNullOrEmpty(idM))
            {
                //Allineamento log - Ciro

                ManagedException mEx = new ManagedException("Il parametro idNode è nullo", "ERR_G005", string.Empty, string.Empty, null);
                ErrorLogInfo er = new ErrorLogInfo(mEx);
                er.objectID = idM;
                log.Error(er);
                model.success = "false";
                model.Message = mEx.Message;
                return this.Request.CreateResponse<JSONTreeNodeModel>(HttpStatusCode.OK, model);

            }
            long idMail = -1;
            if (!long.TryParse(idM, out idMail))
            {
                //Allineamento log - Ciro

                ManagedException mEx = new ManagedException("Il parametro idNode non è numerico", "ERR_G006", string.Empty, string.Empty, null);
                ErrorLogInfo er = new ErrorLogInfo(mEx);
                er.objectID = idM;
                log.Error(er);
                model.success = "false";
                model.Message = mEx.Message;
                return this.Request.CreateResponse<JSONTreeNodeModel>(HttpStatusCode.OK, model);
                //throw new InvalidOperationException("Il parametro idNode non è numerico");
            }

            MailUser us = WebMailClientManager.getAccount();
            if (us == null)
            {
                //Allineamento log - Ciro

                ManagedException mEx = new ManagedException("Non esiste un account loggato", "ERR_G007", string.Empty, string.Empty, null);
                ErrorLogInfo er = new ErrorLogInfo(mEx);
                er.objectID = idM;
                log.Error(er);
                model.success = "false";
                model.Message = mEx.Message;
                return this.Request.CreateResponse<JSONTreeNodeModel>(HttpStatusCode.OK, model);
            }
            //throw new InvalidOperationException("Non esiste un account loggato");

            List<SimpleTreeItem> result = ServiceLocator.GetServiceFactory().MailLocalService.LoadMailTree(us.EmailAddress,
                idMail)
                as List<SimpleTreeItem>;

            Func<List<SimpleTreeItem>, SimpleTreeItem> getRoot = l => l.FirstOrDefault(x => !l.Select(y => y.Value).Contains(x.Padre));
            IEnumerable<HierarchyNode<SimpleTreeItem>> hl = result.AsHierarchy(
                e => e.Value,
                e => e.Padre,
                e => e.Value,
                getRoot(result).Value);

            JSONTreeNode[] nodes = ConvertToJSON(hl);
            model.success = "true";
            model.data = nodes.ToList();
            model.TotalCount = model.data.Count.ToString();
            return this.Request.CreateResponse<JSONTreeNodeModel>(HttpStatusCode.OK, model);
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JSONTreeNode[]));
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //ser.WriteObject(ms, nodes);
            //string json = Encoding.UTF8.GetString(ms.ToArray());
            //ms.Close();

            //context.Response.ContentType = "application/json";
            //context.Response.AddHeader("Content-Type", "text/json");
            //context.Response.Write(json);
        }


        [Authorize]
        [Route("api/PECController/GetMails")]
        public HttpResponseMessage GetMails(int start, int limit, string folder, string parFolder, string mailAction, string mailIds, string filter, int page)
        {
            IEnumerable<Carrier> listCarrier = new List<Carrier>();
            CarrierModel model = new CarrierModel();
            int mailAct = 0;          
            start++;   
            if (mailAction.ToString() != "NaN")
            { mailAct = int.Parse(mailAction); }           
            string message = null;
            MailUser acc = WebMailClientManager.getAccount();
            if(string.IsNullOrEmpty(mailIds))
            { mailIds = string.Empty; }
            message = DoAction(folder, parFolder, mailAct, mailIds, message, acc);
            ResultList<MailHeader> m = new ResultList<MailHeader>(start, limit, new List<MailHeader>(), 0);
           // IEnumerable<string> items = null;
            if (acc != null && acc.IsManaged && !String.IsNullOrEmpty(filter))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(Filter));
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                byte[] res = new byte[filter.Length];
                Encoding.UTF8.GetBytes(filter, 0, filter.Length, res, 0);
                ms.Write(res, 0, res.Length);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                Filter filtro = (Filter)s.ReadObject(ms);

                Dictionary<MailIndexedSearch, List<string>> sValues = new Dictionary<MailIndexedSearch, List<string>>();
                if (filtro.text.tipoSearch != MailIndexedSearch.UNKNOWN &&
                    (filtro.text.values != null && filtro.text.values.Length > 0))
                {
                    sValues.Add(filtro.text.tipoSearch, filtro.text.values.ToList());
                }
                if (filtro.status.tipoSearch != MailIndexedSearch.UNKNOWN &&
                    (filtro.status.values != null && filtro.status.values.Length > 0))
                {
                    sValues.Add(filtro.status.tipoSearch, filtro.status.values.Select(e => ((int)e).ToString()).ToList());
                }

                ResultList<MailHeaderExtended> rl = ServiceLocator.GetServiceFactory().MailLocalService.GetMailsByParams(
                    acc.EmailAddress, folder, parFolder, sValues, start, limit);

                m.Per = rl.Per;
                m.List = (rl.List == null) ? null : rl.List.Cast<MailHeader>().ToList() as ICollection<MailHeader>;
                m.Totale = rl.Totale;
            }
            else
            {
                m = ServiceLocator.GetServiceFactory().getMailServerFacade(acc)
                                                      .MailHeader_ResultList_Fetch(folder, parFolder, start, limit);
            }

            if (m != null && m.List != null)
            {
                CultureInfo ci = CultureInfo.InvariantCulture;
                System.Runtime.Serialization.Json.DataContractJsonSerializer ser =
                       new DataContractJsonSerializer(typeof(Carrier));
                if (acc.IsManaged)
                {
                    listCarrier = m.List.Cast<MailHeaderExtended>().Select(h =>
                    {
                        string mStatus = "";
                        string destinatario = "";
                        switch (parFolder)
                        {
                            case "I":
                                mStatus = h.MailRating.ToString();
                                destinatario = h.From;
                                break;
                            case "O":
                            case "AO":
                                mStatus = ((int)h.MailStatus).ToString();
                                //per gestire il semaforo per gli invii da server non PEC
                                if (!acc.IsPec && h.MailStatus == MailStatus.SENT)
                                    mStatus = ((int)MailStatus.AVVENUTA_CONSEGNA).ToString();
                                destinatario = h.To;
                                break;
                            default:
                                MailUser muser = WebMailClientManager.getAccount();
                                string idnome = (from f in muser.Folders
                                                 where f.Id.ToString() == folder
                                                 select f.IdNome).First();
                                string tipo = (from f in muser.Folders
                                               where f.Id.ToString() == folder
                                               select f.TipoFolder).First();
                                switch (idnome)
                                {

                                    case "1":
                                    case "3":
                                        mStatus = h.MailRating.ToString();
                                        destinatario = h.From;
                                        break;
                                    case "2":
                                        mStatus = ((int)h.MailStatus).ToString();
                                        //per gestire il semaforo per gli invii da server non PEC
                                        if (!acc.IsPec && h.MailStatus == MailStatus.SENT)
                                            mStatus = ((int)MailStatus.AVVENUTA_CONSEGNA).ToString();
                                        destinatario = h.To;
                                        break;
                                    default:
                                        switch (tipo)
                                        {
                                            case "E":
                                                mStatus = h.MailRating.ToString();
                                                destinatario = h.From;
                                                break;
                                            case "D":
                                                mStatus = ((int)h.MailStatus).ToString();
                                                //per gestire il semaforo per gli invii da server non PEC
                                                if (!acc.IsPec && h.MailStatus == MailStatus.SENT)
                                                    mStatus = ((int)MailStatus.AVVENUTA_CONSEGNA).ToString();
                                                destinatario = h.To;
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }

                        ActiveUp.Net.Mail.Address add = ActiveUp.Net.Mail.Parser.ParseAddress(destinatario);
                        if (String.IsNullOrEmpty(add.Name))
                            destinatario = add.Email;
                        else
                        {
                            destinatario = System.Text.RegularExpressions.Regex.Replace(add.Name,
                                "Per conto di:?", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                            if (destinatario.EndsWith("\""))
                                destinatario.Replace("\"", "");
                        }

                        Carrier c = new Carrier
                        {
                            id = h.UniqueId,
                            from = destinatario.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\"", "\\\""),
                            date = h.Date.ToString("dd/MM/yyyy HH:mm:ss", ci),
                            subject = h.Subject.Replace("\"", "\\\""),
                            sStatus = mStatus,
                            mStatus = ((int)h.MailStatus).ToString(),
                            attach = Convert.ToInt16(h.HasAttachments).ToString(),
                            utente = ((h.MailStatus == MailStatus.SCARICATA) ? "" : h.Utente),
                            dimen = h.Dimensione
                        };
                        return c;
                        //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        //ser.WriteObject(ms, c);
                        //string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                        //ms.Close();
                        //return jsonString;
                    });
                }
                else
                {
                    listCarrier = m.List.Select(h =>
                    {
                        Carrier c = new Carrier()
                        {
                            id = h.UniqueId,
                            from = h.From.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\"", "\\\""),
                            date = h.Date.ToString("dd/MM/yyyy HH:mm:ss", ci),
                            subject = h.Subject.Replace("\"", "\\\""),
                            mStatus = ((int)MailStatus.UNKNOWN).ToString(),
                            sStatus = "",
                            attach = ""
                        };
                        return c;
                        //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        //ser.WriteObject(ms, c);
                        //string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                        //ms.Close();
                        //return jsonString;
                    });
                }
            }
            else
            {        
                model.Message = "Nessun ritrovamento";
                  return this.Request.CreateResponse<CarrierModel>(HttpStatusCode.OK,model);
                //message = "Nessun ritrovamento";
            }

            //StringBuilder sb = new StringBuilder();
            //sb.Append("{\"TotalCount\":\"" + m.Totale.ToString() + "\",");
            //sb.Append("\"Message\":\"" + message + "\"");
            //if (items != null)
            //{
            //    sb.Append(",\"Data\":[" + String.Join(",", items.ToArray()) + "]");
            //}
            //else
            //{
            //    sb.Append(",\"Data\": []");
            //}
            //sb.Append("}");
           model.TotalCount = m.Totale.ToString();
           model.data = listCarrier.ToList();
           model.success = "true";
           return this.Request.CreateResponse<CarrierModel>(HttpStatusCode.OK,model);
            //context.Response.ContentType = "application/json";
            //context.Response.AppendHeader("Content-type", "text/json");
            //context.Response.Write(sb.ToString());
        }
        /// <summary>
        /// modificato per gestione folders todo modificare metodi move
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="parentFolder"></param>
        /// <param name="mailAction"></param>
        /// <param name="mailIds"></param>
        /// <param name="message"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        private string DoAction(string folder, string parentFolder, int mailAction, string AllIds, string message, MailUser acc)
        {
            string[] mailIds = AllIds.Split(',');
            List<string> idMail = mailIds.Where(x => !String.IsNullOrEmpty(x)).ToList();
            string utente = MySecurityProvider.CurrentPrincipalName;
            int folderMezzo = 0;
            int.TryParse(folder, out folderMezzo);
            if (folderMezzo == 0)
            { folderMezzo = 1; }
            if (Helpers.CanDo(parentFolder, mailAction.ToString(), idMail))
            {
                Com.Delta.Mail.Facades.IMailServerFacade fac =
                    ServiceLocator.GetServiceFactory()
                    .getMailServerFacade(acc);
                int resp = 0;
                string azione = Helpers.GetAzione(mailAction.ToString());
                try
                {
                    switch (mailAction)
                    {
                        case 1:
                            resp = fac.MailMove(idMail, MailStatus.LETTA, mailAction.ToString(), utente, parentFolder);
                            break;
                        case 2:
                            resp = fac.MailMove(idMail, MailStatus.SCARICATA, mailAction.ToString(), utente, parentFolder);
                            break;

                        default:
                            // azioni generiche
                            resp = fac.MailMove(idMail, MailStatus.LETTA, mailAction.ToString(), utente, parentFolder);
                            break;

                    }
                    if (resp != idMail.Count)
                        message = "Errore nell'azione specificata";
                }
                catch (Exception e)
                {
                    //Allineamento log - Ciro
                    if (e.GetType() == typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException("Errore nell'azione specificata " + e.Message, "ERR_G004", string.Empty, string.Empty, e);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        er.objectID = Convert.ToString(idMail);
                        log.Error(er);
                    }
                    message = "Errore nell'azione specificata " + e.Message;
                }
            }
            else if (((parentFolder == "O") || (parentFolder == "D") || (parentFolder == "AO")) && (mailIds.Length > 0) && (mailIds[0] != string.Empty))
            {
                Com.Delta.Mail.Facades.IMailServerFacade fac =
                    ServiceLocator.GetServiceFactory()
                    .getMailServerFacade(acc);
                int resp = 0;
                try
                {

                    resp = fac.MailArchivia(idMail, utente, mailAction.ToString(), parentFolder);
                    if (resp != idMail.Count)
                        message = "Errore nell'azione specificata";
                }
                catch (Exception e)
                {
                    //Allineamento log - Ciro
                    if (e.GetType() == typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException("Errore nell'azione specificata " + e.Message, "ERR_G005", string.Empty, string.Empty, e);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        er.objectID = Convert.ToString(idMail);
                        log.Error(er);
                    }
                    message = "Errore nell'azione specificata " + e.Message;
                }
            }
            return message;
        }


       

        #region "Private methods"

        private JSONTreeNode[] ConvertToJSON(IEnumerable<HierarchyNode<SimpleTreeItem>> tree)
        {
            List<JSONTreeNode> list = new List<JSONTreeNode>();
            foreach (HierarchyNode<SimpleTreeItem> item in tree)
            {
                JSONTreeNode node = new JSONTreeNode()
                {
                    id = item.Entity.Value,
                    text = item.Entity.Text,
                    source = item.Entity.Source
                };
                if (item.ChildNodes == null || item.ChildNodes.Count() == 0)
                { node.leaf = true; }
                else { 
                    node.leaf = false;
                    node.expanded = true;
                }
                if (item.Entity.Source == "I")
                    node.icon = "../../App_Themes/Delta/images/tree/left-blue-arrow.png";
                else if (item.Entity.Source == "O")
                    node.icon = "../../App_Themes/Delta/images/tree/right-red-arrow.png";
                if (item.ChildNodes != null && item.ChildNodes.Count() > 0)
                {
                    node.data = ConvertToJSON(item.ChildNodes);
                }
                list.Add(node);
            }
            return list.ToArray();
        }

        #endregion
    }
}


using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.Facades;
using Com.Delta.Mail.MailMessage;
using Com.Delta.Security;
using Com.Delta.Web;
using Com.Delta.Web.Cache;
using Com.Delta.Web.Session;
using Delta.Utilities.LinqExtensions;
using GestionePEC.Extensions;
using GestionePEC.Models;
using log4net;
using Newtonsoft.Json;
using SendMail.BusinessEF;
using SendMail.BusinessEF.MailFacedes;
using SendMail.Model;
using SendMail.Model.ComunicazioniMapping;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Xml;

namespace GestionePEC.Api
{
    public class PECController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PECController));

        //     [Serializable]
        [DataContract]
        public class JSONTreeNode
        {

            [DataMember]
            public string id { get; set; }

            [DataMember]
            public string text { get; set; }

            [DataMember]
            public bool leaf { get; set; }

            [DataMember]
            public bool expanded { get; set; }

            [DataMember]
            public JSONTreeNode[] data { get; set; }

            [DataMember]
            public string icon { get; set; }

            [DataMember]
            public string source { get; set; }
        }


        //[Serializable]
        //[JsonObject]
        [DataContract]
        public class JSONTreeNodeModel
        {

            [DataMember]
            public List<JSONTreeNode> data;

            [DataMember]
            public string Message;

            [DataMember]
            public string TotalCount;

            [DataMember]
            public string success;
        }



        [DataContract]
        [Serializable]
        internal class SubFilter<T>
        {
            [DataMember(Name = "tipo")]
            private string _tipo { get; set; }
            [DataMember(Name = "value")]
            private string _value { get; set; }

            internal MailIndexedSearch tipo
            {
                get
                {
                    MailIndexedSearch mis = MailIndexedSearch.UNKNOWN;
                    int idx = -1;
                    if (!String.IsNullOrEmpty(_tipo) && int.TryParse(_tipo, out idx))
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
                    if (_value != null)
                    {
                        string[] vals = _value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
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

        [DataContract]
        [Serializable]
        internal class Filter
        {
            [DataMember]
            internal SubFilter<string> text { get; set; }
            [DataMember]
            internal SubFilter<MailStatus> status { get; set; }
        }

        [DataContract]
        [Serializable]
        internal class CarrierModel
        {
            [DataMember]
            internal List<Carrier> data;
            [DataMember]
            internal string Message;
            [DataMember]
            internal string TotalCount;
            [DataMember]
            internal string success;
        }


        [DataContract]
        [Serializable]
        internal class Carrier
        {
            [DataMember]
            internal string id;
            [DataMember]
            internal string from;
            [DataMember]
            internal string date;
            [DataMember]
            internal string subject;
            [DataMember]
            internal string sStatus;
            [DataMember]
            internal string mStatus;
            [DataMember]
            internal string attach;
            [DataMember]
            internal string utente;
            [DataMember]
            internal int dimen;
        }



        [Authorize]
        [Route("api/PECController/GetTreeStructure")]
        public HttpResponseMessage GetTreeStructure(string idM)
        {
            JSONTreeNodeModel model = new JSONTreeNodeModel();
            if (string.IsNullOrEmpty(idM))
            {

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

                ManagedException mEx = new ManagedException("Il parametro idNode non è numerico", "ERR_G006", string.Empty, string.Empty, null);
                ErrorLogInfo er = new ErrorLogInfo(mEx);
                er.objectID = idM;
                log.Error(er);
                model.success = "false";
                model.Message = mEx.Message;
                return this.Request.CreateResponse<JSONTreeNodeModel>(HttpStatusCode.OK, model);

            }

            MailUser us = WebMailClientManager.getAccount();
            if (us == null)
            {
                ManagedException mEx = new ManagedException("Non esiste un account loggato", "ERR_G007", string.Empty, string.Empty, null);
                ErrorLogInfo er = new ErrorLogInfo(mEx);
                er.objectID = idM;
                log.Error(er);
                model.success = "false";
                model.Message = mEx.Message;
                return this.Request.CreateResponse<JSONTreeNodeModel>(HttpStatusCode.OK, model);
            }

            MailLocalService mailService = new MailLocalService();
            IEnumerable<HierarchyNode<SimpleTreeItem>> hl = null;
            List<SimpleTreeItem> result = mailService.LoadMailTree(us.EmailAddress, idMail) as List<SimpleTreeItem>;
            if (result.Count > 0)
            {
                Func<List<SimpleTreeItem>, SimpleTreeItem> getRoot = l => l.FirstOrDefault(x => !l.Select(y => y.Value).Contains(x.Padre));
                hl = result.AsHierarchy(
                     e => e.Value,
                     e => e.Padre,
                     e => e.Value,
                     getRoot(result).Value);
            }
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
            if (string.IsNullOrEmpty(mailIds))
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
                if (filtro.text.tipo != MailIndexedSearch.UNKNOWN &&
                    (filtro.text.values != null && filtro.text.values.Length > 0))
                {
                    sValues.Add(filtro.text.tipo, filtro.text.values.ToList());
                }
                if (filtro.status.tipo != MailIndexedSearch.UNKNOWN &&
                    (filtro.status.values != null && filtro.status.values.Length > 0))
                {
                    sValues.Add(filtro.status.tipo, filtro.status.values.Select(e => ((int)e).ToString()).ToList());
                }
                MailLocalService mailService = new MailLocalService();
                ResultList<MailHeaderExtended> rl = mailService.GetMailsByParams(acc.EmailAddress, folder, parFolder, sValues, start, limit);

                m.Per = rl.Per;
                m.List = (rl.List == null) ? null : rl.List.Cast<MailHeader>().ToList() as ICollection<MailHeader>;
                m.Totale = rl.Totale;
            }
            else
            {
                MailServerFacade mailServerFacade = MailServerFacade.GetInstance(acc);
                m = mailServerFacade.MailHeader_ResultList_Fetch(folder, parFolder, start, limit);
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
                return this.Request.CreateResponse<CarrierModel>(HttpStatusCode.OK, model);
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
            return this.Request.CreateResponse<CarrierModel>(HttpStatusCode.OK, model);
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
            string utente = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;
            int folderMezzo = 0;
            int.TryParse(folder, out folderMezzo);
            if (folderMezzo == 0)
            { folderMezzo = 1; }
            if (Helpers.CanDo(parentFolder, mailAction.ToString(), idMail))
            {
                MailServerFacade fac = MailServerFacade.GetInstance(acc);
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
                MailServerFacade fac = MailServerFacade.GetInstance(acc);
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


        [HttpPost]
        [Authorize]
        [Route("api/PECController/Comunicazione")]
        public HttpResponseMessage Comunicazione(FormDataCollection formsValues)
        {
            string regexMail = RegexUtils.EMAIL_REGEX.ToString();
            TitoliModel model = new TitoliModel();
            try
            {
                List<ComAllegato> allegati = new List<ComAllegato>();
                SendMail.Model.ComunicazioniMapping.Comunicazioni comun = new SendMail.Model.ComunicazioniMapping.Comunicazioni();
                string destinatario = formsValues["Destinatario"];
                string conoscenza = formsValues["Conoscenza"];
                string bcc = formsValues["BCC"];
                string oggetto = formsValues["Oggetto"];
                string testoMail = formsValues["TestoMail"];
                string titolo = formsValues["Titolo"];
                string sottotitolo = formsValues["SottoTitolo"];
                string userid = formsValues["SenderMail"];
                if (SessionManager<Dictionary<string, DTOFileUploadResult>>.exist(SessionKeys.DTO_FILE))
                {
                    Dictionary<string, DTOFileUploadResult> dictDto = SessionManager<Dictionary<string, DTOFileUploadResult>>.get(SessionKeys.DTO_FILE);
                    List<DTOFileUploadResult> dto = (List<DTOFileUploadResult>)dictDto.Values.ToList();
                    foreach (DTOFileUploadResult d in dto)
                    {
                        SendMail.Model.ComunicazioniMapping.ComAllegato allegato = new SendMail.Model.ComunicazioniMapping.ComAllegato();
                        allegato.AllegatoExt = d.Extension;
                        allegato.AllegatoFile = d.CustomData;
                        allegato.AllegatoName = d.FileName;
                        allegato.AllegatoTpu = "";
                        allegato.FlgInsProt = AllegatoProtocolloStatus.FALSE;
                        allegato.FlgProtToUpl = AllegatoProtocolloStatus.FALSE;
                        allegati.Add(allegato);
                    }

                }
                comun.ComAllegati = allegati;
                string[] destinatari = destinatario.Split(';');
                for (int i = 0; i < destinatari.Length; i++)
                {
                    var match = Regex.Match(destinatari[i], regexMail, RegexOptions.IgnoreCase);
                    if (!match.Success)
                    {
                        model.success = "false";
                        model.message = "Attenzione mail destinatario non valida";
                        return this.Request.CreateResponse<TitoliModel>(HttpStatusCode.OK, model);
                    }
                }
                // gestione destinatari
                ContactsApplicationMapping c = new ContactsApplicationMapping
                {
                    Mail = destinatario,
                    IdSottotitolo = long.Parse(sottotitolo),
                    IdTitolo = long.Parse(titolo)
                };
                Collection<ContactsApplicationMapping> en = new Collection<ContactsApplicationMapping>();
                en.Add(c);
                if (conoscenza != string.Empty)
                {
                    string[] conoscenze = conoscenza.Split(';');
                    for (int i = 0; i < conoscenze.Length; i++)
                    {
                        var match = Regex.Match(conoscenze[i], regexMail, RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {

                            model.success = "false";
                            model.message = "Attenzione mail conoscenza non valida";
                            return this.Request.CreateResponse<TitoliModel>(HttpStatusCode.OK, model);
                        }
                    }
                    ContactsApplicationMapping c1 = new ContactsApplicationMapping
                    {
                        Mail = conoscenza,
                        IdSottotitolo = long.Parse(sottotitolo),
                        IdTitolo = long.Parse(titolo)
                    };
                    Collection<ContactsApplicationMapping> en1 = new Collection<ContactsApplicationMapping>();
                    en1.Add(c1);
                    comun.SetMailDestinatari(en1, AddresseeType.CC);
                }
                if (bcc != string.Empty)
                {
                    string[] bccs = bcc.Split(';');
                    for (int i = 0; i < bcc.Length; i++)
                    {
                        var match = Regex.Match(bccs[i], regexMail, RegexOptions.IgnoreCase);
                        if (!match.Success)
                        {
                            model.success = "false";
                            model.message = "Attenzione mail destinatario nascosto non valido";
                            return this.Request.CreateResponse<TitoliModel>(HttpStatusCode.OK, model);
                        }
                    }

                    ContactsApplicationMapping c2 = new ContactsApplicationMapping
                    {
                        Mail = bcc,
                        IdSottotitolo = long.Parse(sottotitolo),
                        IdTitolo = long.Parse(titolo)
                    };
                    Collection<ContactsApplicationMapping> en2 = new Collection<ContactsApplicationMapping>();
                    en2.Add(c2);
                    comun.SetMailDestinatari(en2, AddresseeType.CCN);
                }
                comun.SetMailDestinatari(en, AddresseeType.TO);
                // gestione body email
                MailContent content = new MailContent();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                string html = testoMail;
                HtmlAgilityPack.HtmlNode body = null;
                if (string.IsNullOrEmpty(html) == false)
                {
                    doc.LoadHtml(html);
                    doc.OptionAutoCloseOnEnd = true;
                    doc.OptionFixNestedTags = true;
                    body = doc.DocumentNode.Descendants()
                                               .Where(n => n.Name.Equals("body", StringComparison.InvariantCultureIgnoreCase))
                                               .FirstOrDefault();
                }
                var ele = new HtmlAgilityPack.HtmlNode(HtmlAgilityPack.HtmlNodeType.Element, doc, 0);
                ele.Name = "div";
                ele.InnerHtml = testoMail;
                content.MailSubject = oggetto;
                content.MailText = ele.OuterHtml;
                // gestione sender 
                MailServerConfigFacade mailSCF = MailServerConfigFacade.GetInstance();
                MailUser mailuser = mailSCF.GetUserByUserId(decimal.Parse(userid));
                content.MailSender = mailuser.EmailAddress;
                c.IdTitolo = long.Parse(titolo);
                c.IdSottotitolo = long.Parse(sottotitolo);
                comun.UtenteInserimento = MySecurityProvider.CurrentPrincipal.MyIdentity.Name;
                comun.MailComunicazione = content;
                // da scommentare
                comun.FolderTipo = "O";
                comun.FolderId = 2;
                ComunicazioniService com = new ComunicazioniService();
                com.InsertComunicazione(comun);
                model.success = "true";
                SessionManager<Dictionary<string, DTOFileUploadResult>>.del(SessionKeys.DTO_FILE);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Errore invio nuova mail. Dettaglio: " + ex.Message +
                        "StackTrace: " + ((ex.StackTrace != null) ? ex.StackTrace.ToString() : " vuoto "),
                        "ERR317",
                        string.Empty,
                        string.Empty,
                        ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                    model.success = "false";
                    model.message = ex.Message;
                    return this.Request.CreateResponse<TitoliModel>(HttpStatusCode.OK, model);
                }
               
            }
            return this.Request.CreateResponse<TitoliModel>(HttpStatusCode.OK, model);
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
                else
                {
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

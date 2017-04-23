using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Model;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SendMail.Model.RubricaMapping;
using System.Data.Common;
using SendMail.Business.Data.QueryModel;

namespace SendMail.Data.SQLServerDB.Mapping
{
    public class DaoSQLServerDBHelper
    {
        internal static string TranslateOperator(CriteriaOperator co)
        {
            string op = "";
            switch (co)
            {
                case CriteriaOperator.Equal:
                    op = "=";
                    break;
                case CriteriaOperator.NotEqual:
                    op = "!=";
                    break;
                case CriteriaOperator.GreaterThan:
                    op = ">";
                    break;
                case CriteriaOperator.LesserThan:
                    op = "<";
                    break;
                case CriteriaOperator.GreaterThanOrEqual:
                    op = ">=";
                    break;
                case CriteriaOperator.LesserThanOrEqual:
                    op = "<=";
                    break;
                case CriteriaOperator.Like:
                    op = "LIKE";
                    break;
                case CriteriaOperator.NotLike:
                    op = "NOT LIKE";
                    break;
                case CriteriaOperator.StartsWith:
                    op = "LIKE";
                    break;
                case CriteriaOperator.EndsWith:
                    op = "LIKE";
                    break;
                case CriteriaOperator.IsNull:
                    op = "IS NULL";
                    break;
                case CriteriaOperator.IsNotNull:
                    op = "IS NOT NULL";
                    break;
                default:
                    break;
            }
            return op;
        }

        internal static ACTIONS_FOLDERS MapToActionFolder(ActionFolder a, bool v)
        {
            ACTIONS_FOLDERS f = new ACTIONS_FOLDERS()
            {
                IDACTION =(double) a.idAction,
                IDFOLDER = a.idFolder
            };
            if(!v)
            { f.ID = a.iD; }
            return f;
        }

        internal static ACTIONS MapToActionDto(ActiveUp.Net.Common.DeltaExt.Action entity,bool isInsert)
        {
            ACTIONS a = new ACTIONS()
            {
                ID_FOLDER_DESTINAZIONE = entity.IdFolderDestinazione,
                NOME_AZIONE = entity.NomeAzione,
                NUOVO_STATUS = entity.NuovoStatus,
                TIPO_AZIONE = entity.TipoAzione,
                ID_NOME_DESTINAZIONE =(double) entity.IdDestinazione,
                TIPO_DESTINAZIONE = entity.TipoDestinazione
            };
            if(!isInsert)
            {
                a.ID =(double) entity.Id;
            }
            return a;
        }

        internal static FOLDERS MapToFolderDto(Folder entity, bool v)
        {
            FOLDERS f = new FOLDERS()
            {
                ID = entity.Id,
                IDNOME =double.Parse(entity.IdNome),
                NOME = entity.Nome,
                SYSTEM = "0",
                TIPO = entity.TipoFolder                
            };
            if (v)
            {
                f.ID++;
            }
            return f;
        }

        internal static SottoTitolo MapToSottotitolo(COMUNICAZIONI_SOTTOTITOLI s)
        {
            SottoTitolo st = new SottoTitolo();
            st.Id = (Int32)s.ID_SOTTOTITOLO;
            st.Titolo.Id = (Int32)s.REF_ID_TITOLO;
                st.Nome = s.SOTTOTITOLO;
            st.ComCode =s.COM_CODE;
            st.Note = s.NOTE;
            if (s.ACTIVE != 1)
                st.Deleted = false;
            else
                st.Deleted = !Convert.ToBoolean(s.ACTIVE);
            st.ProtocolloCode = s.PROT_CODE;
            if (s.PROT_LOAD_ALLEGATI == 0)
                st.ProtocolloLoadAllegati = false;
            else
                st.ProtocolloLoadAllegati = Convert.ToBoolean(s.PROT_LOAD_ALLEGATI);
            st.ProtocolloPassword = s.PROT_PWD;
            st.ProtocolloSubCode =s.PROT_SUBCODE;
            st.TipiProcollo = null;
            if (!String.IsNullOrEmpty(s.PROT_TIPI_AMMESSI))
            {
                st.TipiProcollo = (from t in  s.PROT_TIPI_AMMESSI.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                   let exist = Enum.GetNames(typeof(ProtocolloTypes)).Contains(t)
                                   select ((exist) ? (ProtocolloTypes)Enum.Parse(typeof(ProtocolloTypes), t) : ProtocolloTypes.UNKNOWN)).ToList();

            }
            if (s.PROT_MANAGED == 0)
                st.UsaProtocollo = false;
            else
                st.UsaProtocollo =Convert.ToBoolean(s.PROT_MANAGED);
             
            return st;

        }

        internal static Model.ContactsApplicationMapping MapToContactsApplication(System.Data.Common.DbDataReader dr)
        {
            ContactsApplicationMapping cam = new ContactsApplicationMapping();
            cam.IdMap = (long)dr.GetDecimal("ID_MAP");
            cam.IdTitolo = (long)dr.GetDecimal("ID_TITOLO");
            cam.AppCode = dr.GetString("APP_CODE");
            cam.Titolo = dr.GetString("TITOLO");
            cam.TitoloCode = dr.GetString("TITOLO_PROT_CODE");
            cam.IsTitoloActive = Convert.ToBoolean(dr.GetDecimal("TITOLO_ACTIVE"));
            cam.IdSottotitolo = (long)dr.GetDecimal("ID_SOTTOTITOLO");
            cam.Sottotitolo = dr.GetString("SOTTOTITOLO");
            cam.SottotitoloCode = dr.GetString("SOTTOTITOLO_PROT_CODE");
            cam.IsSottotitoloActive = Convert.ToBoolean(dr.GetDecimal("SOTTOTITOLO_ACTIVE"));
            cam.ComCode = dr.GetString("COM_CODE");
            cam.IdContact = (long)dr.GetDecimal("ID_CONTACT");
            cam.Mail = dr.GetString("MAIL");
            cam.Fax = dr.GetString("FAX");
            cam.Telefono = dr.GetString("TELEFONO");
            cam.RefIdReferral = (long)dr.GetDecimal("REF_ID_REFERRAL");
            cam.IsPec = Convert.ToBoolean(dr.GetDecimal("FLG_PEC"));
            cam.RefOrg = (long)dr.GetDecimal("REF_ORG");
            cam.IdCanale = (long)dr.GetDecimal("ID_CANALE");
            cam.Codice = dr.GetString("CODICE");
            cam.IdBackend = (long)dr.GetDecimal("ID_BACKEND");
            cam.BackendCode = dr.GetString("BACKEND_CODE");
            cam.BackendDescr = dr.GetString("BACKEND_DESCR");
            cam.Category = dr.GetString("CATEGORY");
            cam.DescrPlus = dr.GetString("DESCR_PLUS");
            return cam;
        }

        internal static MAIL_INBOX MapToMailInBox(MailUser u, ActiveUp.Net.Mail.Message m)
        {

            MAIL_INBOX inbox = new MAIL_INBOX();
            inbox.MAIL_FROM = m.From.ToString();
            inbox.MAIL_TO = String.Join(";", m.To.Select(to => to.Email).ToArray());
            inbox.MAIL_CC = (!string.IsNullOrEmpty(m.Cc.ToString())) ? String.Join(";", m.Cc.Select(cc => cc.Email).ToArray()) : string.Empty;
            inbox.MAIL_CCN = (!string.IsNullOrEmpty(m.Bcc.ToString())) ? String.Join(";", m.Bcc.Select(Bcc => Bcc.Email).ToArray()) : string.Empty;
            inbox.MAIL_SUBJECT = m.Subject;
            inbox.MAIL_TEXT = LinqExtensions.TryParseBody(m.BodyText);
            inbox.DATA_INVIO = m.ReceivedDate.ToLocalTime();
            inbox.DATA_RICEZIONE = m.Date.ToLocalTime();
            inbox.STATUS_SERVER = (String.IsNullOrEmpty(m.MessageId)) ? ((int)MailStatusServer.DA_NON_CANCELLARE).ToString() : ((int)MailStatusServer.PRESENTE).ToString();
            inbox.STATUS_MAIL = (String.IsNullOrEmpty(m.MessageId)) ? ((int)MailStatus.SCARICATA_INCOMPLETA).ToString() : ((int)MailStatus.SCARICATA).ToString();
            inbox.MAIL_FILE = System.Text.Encoding.GetEncoding("iso-8859-1").GetString(m.OriginalData);
            inbox.FLG_ATTACHMENTS = Convert.ToInt16(m.Attachments.Count > 0).ToString();
            inbox.FOLDERID = String.IsNullOrEmpty(m.HeaderFields["X-Ricevuta"]) ? 1 : 3;
            inbox.FOLLOWS = LinqExtensions.TryParseFollows(m.HeaderFields);
            inbox.MSG_LENGTH = m.OriginalData.Length;
            inbox.FOLDERTIPO = "I";
            inbox.FOLLOWS = LinqExtensions.TryParseFollows(m.HeaderFields);
            return inbox;
        }

        internal static COMUNICAZIONI_SOTTOTITOLI MapToComunicazioniSottotitolo(SottoTitolo sottoTitolo,bool isInsert)
        {
            COMUNICAZIONI_SOTTOTITOLI s = new COMUNICAZIONI_SOTTOTITOLI()
            {
                REF_ID_TITOLO = sottoTitolo.RefIdTitolo,
                ACTIVE = Convert.ToDecimal(!sottoTitolo.Deleted),
                COM_CODE = sottoTitolo.ComCode,
                NOTE = sottoTitolo.Note,
                PROT_CODE = sottoTitolo.ProtocolloCode
            };            
            s.PROT_MANAGED = Convert.ToDecimal(sottoTitolo.UsaProtocollo);
            s.PROT_PWD = sottoTitolo.ProtocolloPassword;
            s.PROT_SUBCODE = sottoTitolo.ProtocolloSubCode;
            s.SOTTOTITOLO = sottoTitolo.Nome;
            if (sottoTitolo.TipiProcollo != null)
            { s.PROT_TIPI_AMMESSI = String.Join(";", sottoTitolo.TipiProcollo.Cast<string>().ToArray()); }
            if (sottoTitolo.ProtocolloLoadAllegati != null)
            { s.PROT_LOAD_ALLEGATI = Convert.ToDecimal(sottoTitolo.ProtocolloLoadAllegati); }
            if(!isInsert)
            { s.ID_SOTTOTITOLO = sottoTitolo.Id; }
            return s;           
        }

        internal static RUBR_CONTATTI_BACKEND MapToRubrContattiBackend(Model.ContactApplicationMapping.ContactsBackendMap entity)
        {

            RUBR_CONTATTI_BACKEND m = new RUBR_CONTATTI_BACKEND()
            {
                REF_ID_CANALE = (int)entity.Canale,
                REF_ID_BACKEND = (entity.Backend.Id != -1) ? (int)entity.Backend.Id : 0,
                REF_ID_CONTATTO = (entity.Contatto.IdContact != -1) ? (int)entity.Contatto.IdContact : 0,
                REF_ID_TITOLO = (entity.Titolo.Id != -1) ? (int)entity.Titolo.Id : 0
            };
            return m;

        }

        internal static SendersFolders MapToSendersFolders(DbDataReader dr)
        {
            SendersFolders sFold = new SendersFolders();

            sFold.IdSender = Convert.ToInt32(dr.GetValue("ID_SENDER"));
            //sFold.IdFolder = Convert.ToInt32(dr.GetValue("ID"));
            sFold.Nome = (string)dr.GetValue("NOME");
            sFold.Mail = (string)dr.GetValue("MAIL");
            sFold.IdNome = Convert.ToInt16(dr.GetValue("IDNOME"));
            //sFold.Tipo = (string)dr.GetValue("TIPO");
            sFold.System = Convert.ToInt16(dr.GetValue("SYSTEM"));

            return sFold;
        }

        internal static MailUser MapToMailUser(MAIL_SENDERS dr, MailServer s, List<Folder> l)
        {
            MailUser u = null;

            if (s != null)
                u = new MailUser(s);
            else u = new MailUser();
            u.UserId = dr.ID_SENDER;
            u.Casella = dr.MAIL;
            u.Dominus = (s == null) ? "" : s.Dominus;
            u.EmailAddress = dr.MAIL;
            u.LoginId = dr.USERNAME;
            u.Password = dr.PASSWORD;                
            u.Folders = l;
            if (dr.FLG_MANAGED != null)
            {u.FlgManaged = int.Parse(dr.FLG_MANAGED);}
            return u;
        }

        internal static MailServer MapToMailServer(IDataRecord dr)
        {
            MailServer r = new MailServer();
            r.Id = dr.GetDecimal("ID_SVR");
            r.DisplayName = dr.GetString("NOME");
            r.IncomingServer = dr.GetString("INDIRIZZO_IN");
            r.OutgoingServer = dr.GetString("INDIRIZZO_OUT");
            r.PortIncomingServer = int.Parse(dr.GetString("PORTA_IN"));
            r.PortIncomingChecked = !r.PortIncomingServer.Equals(110);
            r.PortOutgoingServer = int.Parse(dr.GetString("PORTA_OUT"));
            r.PortOutgoingChecked = !r.PortOutgoingServer.Equals(25);
            r.IsIncomeSecureConnection = bool.Parse(dr.GetString("SSL_IN"));
            r.IsOutgoingSecureConnection = bool.Parse(dr.GetString("SSL_OUT"));
            r.IsOutgoingWithAuthentication = bool.Parse(dr.GetString("AUTH_OUT"));
            r.IncomingProtocol = dr.GetString("PROTOCOLLO_IN");
            r.Dominus = dr.GetString("DOMINUS");
            r.IsPec = Convert.ToBoolean(int.Parse(dr.GetString("FLG_ISPEC")));

            return r;
        }

        internal static ActiveUp.Net.Common.DeltaExt.Action MapToAction(IDataRecord dr)
        {
            ActiveUp.Net.Common.DeltaExt.Action a = new ActiveUp.Net.Common.DeltaExt.Action();
            a.Id =(decimal) dr.GetDouble("ID");
            a.NomeAzione = dr.GetString("NOME_AZIONE");
            a.IdDestinazione = (decimal)dr.GetDouble("ID_NOME_DESTINAZIONE");
            a.NuovoStatus = dr.GetString("NUOVO_STATUS");
            a.TipoAzione = dr.GetString("TIPO_AZIONE");
            a.TipoDestinazione = dr.GetString("TIPO_DESTINAZIONE");
            if (dr.FieldCount > 7)
            { a.IdComp = dr.GetDecimal("IDFOLDER").ToString(); }
            a.IdFolderDestinazione = int.Parse(dr.GetDouble("ID_FOLDER_DESTINAZIONE").ToString());
            return a;
        }

        internal static Folder MapToFolder(IDataRecord dr, List<ActiveUp.Net.Common.DeltaExt.Action> l)
        {
            Folder f = new Folder();
            f.Id = dr.GetDecimal("ID");
            f.Nome = dr.GetString("NOME");
            f.Abilitata = dr.GetString("SYSTEM");
            f.TipoFolder = dr.GetString("TIPO");
            f.IdNome = dr.GetDouble("IDNOME").ToString();
            if (l != null)
            { f.Azioni = l; }
            return f;
        }
        internal static SimpleTreeItem MapToSimpleTreeItem(IDataRecord dr)
        {
            SimpleTreeItem t = new SimpleTreeItem();
            t.Value = dr["VALUE"].ToString();
            t.Text = dr["TEXT"].ToString();
            t.SubType = dr["SUBTYPE"].ToString();
            t.Source = dr["SOURCE"].ToString();
            t.Padre = dr["PADRE"].ToString();
            return t;
        }


        internal static MailHeaderExtended MapToMailHeaderExtended(IDataRecord dr)
        {
            MailHeaderExtended me = new MailHeaderExtended();
            me.CC = dr.GetString("MAIL_CC");
            me.CCn = dr.GetString("MAIL_CCN");
            me.Date = dr.GetDateTime("DATA_INVIO");
            me.From = dr.GetString("MAIL_FROM");
            me.MailPartialText = dr.GetString("MAIL_TEXT");
            me.MailStatus = (MailStatus)Enum.Parse(typeof(MailStatus), dr.GetString("STATUS_MAIL"));
            me.ReceiveDate = dr.GetDateTime("DATA_RICEZIONE");
            if (me.Date.ToString("dd/MM/yyyy") == "01/01/0001")
            { me.Date = me.ReceiveDate; }
            me.ServerStatus = (MailStatusServer)Enum.Parse(typeof(MailStatusServer), dr.GetString("STATUS_SERVER"));
            me.Subject = dr.GetString("MAIL_SUBJECT");
            me.To = dr.GetString("MAIL_TO");
            me.UniqueId = dr.GetString("MAIL_ID");
            me.MailRating =(int) dr.GetDecimal("FLG_RATING");
            me.HasAttachments = Convert.ToBoolean(int.Parse(dr.GetString("FLG_ATTACHMENTS")));
            me.Utente = dr.GetString("UTENTE");
            me.FolderId = dr.GetDecimal("FOLDERID");
            me.FolderTipo = dr.GetString("FOLDERTIPO");
            me.Dimensione = (int)dr.GetDecimal("msg_length");
            me.NomeFolder = dr.GetString("NOME");
            return me;
        }

        internal static RUBR_CONTATTI MapToRubrContatti(RubricaContatti entity,bool isInsert)
        {
            RUBR_CONTATTI oparams = new RUBR_CONTATTI
            {
                MAIL = entity.Mail,
                TELEFONO = entity.Telefono,
                FAX = entity.Fax,
                FLG_PEC = Convert.ToInt64(entity.IsPec),
                SOURCE = entity.Source,
                NOTE = entity.Note,
                CONTACT_REF = entity.ContactRef,
                REF_ID_REFERRAL = (decimal) entity.RefIdReferral
            };
            if (!isInsert)
            {
                oparams.ID_CONTACT =(decimal) entity.IdContact;
            }

            return oparams;
        }

        internal static RUBR_ENTITA MapToRubrEntita(RubricaEntita e,bool isInsert)
        {
            RUBR_ENTITA r = new RUBR_ENTITA();
            r.AFF_IPA = e.AffIPA;
            r.COD_FIS = e.CodiceFiscale;
            r.COGNOME = e.Cognome;
            r.DISAMB_POST = e.DisambPost;
            r.DISAMB_PRE = e.DisambPre;
            r.FLG_IPA = e.IsIPA.ToString();
            r.NOTE = e.Note;
            r.NOME = e.Nome;
            r.P_IVA = e.PartitaIVA;
            r.SITO_WEB = e.SitoWeb;        
            if (!isInsert)
               r.ID_REFERRAL =(decimal) e.IdReferral;
            else
            {
              r.ID_PADRE = e.IdPadre;
              r.REFERRAL_TYPE = e.ReferralType.ToString();
              r.RAGIONE_SOCIALE = e.RagioneSociale;
              r.REF_ID_ADDRESS = e.RefIdAddress;
              r.FLG_IPA = Convert.ToInt16(e.IsIPA).ToString();
              r.IPA_DN = e.IPAdn;
              r.IPA_ID = e.IPAId;
                r.DISAMB_PRE = e.DisambPre;
                r.DISAMB_POST = e.DisambPost;
               r.REF_ORG = e.RefOrg;
                r.AFF_IPA = e.AffIPA;
              
            }
            return r;
        }       
        
        
    }

}
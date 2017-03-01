using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using SendMail.Model;
using Com.Delta.Data.QueryModel;
using SendMail.Model.Wrappers;
using SendMail.Model.ComunicazioniMapping;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.Model.RubricaMapping;
using SendMail.Model.ContactApplicationMapping;
using ActiveUp.Net.Mail;

namespace SendMail.Data.Utilities
{
    public class DaoOracleDbHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static string TextSearch(CriteriaOperator co, object value)
        {
            string result = "";
            string val = value.ToString();
            if (co == CriteriaOperator.Like || co == CriteriaOperator.NotLike)
                result = "%" + val + "%";
            else if (co == CriteriaOperator.StartsWith)
                result = val + "%";
            else if (co == CriteriaOperator.EndsWith)
                result = "%" + val;
            else
                result = val;
            //result =  "%" + (string)value + "%";
            return result.ToUpper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Mappa un record su di un oggetto Richiesta "lazy".
        /// </summary>
        /// <param name="dr">datarecord</param>
        /// <returns></returns>
        internal static Rubrica MapToRubrica(IDataRecord dr)
        {
            Rubrica req = new Rubrica();
            req.IdRubrica = dr.GetDecimal("ID_RUB");
            req.Cap = dr.GetString("CAP");
            req.Certificato = dr.GetInt32("CERTIFIED");
            req.Citta = dr.GetString("CITTA");
            req.Fax = dr.GetString("FAX");
            req.Ufficio = dr.GetString("UFFICIO");
            req.Ragione_Sociale = dr.GetString("RAGIONESOCIALE");
            req.Provincia = dr.GetString("PROVINCIA");
            //req.Flag_IPA = dr.GetString("FLG_IPA");
            req.Mail = dr.GetString("MAIL");
            return req;
        }

        internal static RubricaContatti MapToRubricaContatti(IDataRecord dr)
        {
            RubricaContatti rc = new RubricaContatti();
            if (dr.IsDBNull("ID_CONTACT"))
                rc.IdContact = null;
            else
                rc.IdContact = dr.GetInt64("ID_CONTACT");
            rc.Mail = dr.GetString("MAIL");
            rc.Fax = dr.GetString("FAX");
            rc.Telefono = dr.GetString("TELEFONO");
            if (dr.IsDBNull("REF_ID_REFERRAL"))
                rc.RefIdReferral = null;
            else
                rc.RefIdReferral = dr.GetInt64("REF_ID_REFERRAL");
            rc.Source = dr.GetString("SOURCE");
            rc.IsIPA = false;
            if (!dr.IsDBNull("FLG_IPA"))
                rc.IsIPA = Convert.ToBoolean(int.Parse(dr.GetString("FLG_IPA")));
            rc.IPAdn = dr.GetString("IPA_DN");
            if (dr.IsDBNull("IPA_ID"))
                rc.IPAId = null;
            else
                rc.IPAId = dr.GetString("IPA_ID");
            rc.Note = dr.GetString("NOTE");
            rc.ContactRef = dr.GetString("CONTACT_REF");
            if (dr.IsDBNull("AFF_IPA"))
                rc.AffIPA = null;
            else
                rc.AffIPA = (short)dr.GetInt32("AFF_IPA");
            rc.IsPec = false;
            if (!dr.IsDBNull("FLG_PEC"))
                rc.IsPec = Convert.ToBoolean(dr.GetInt32("FLG_PEC"));
            return rc;
        }

        internal static RubricaEntita MapToRubricaEntita(IDataRecord dr)
        {
            RubricaEntita re = new RubricaEntita();
            re.IdReferral = (Nullable<Int64>)dr.GetValue("ID_REFERRAL");
            re.IdPadre = (Nullable<Int64>)dr.GetValue("ID_PADRE");
            re.ReferralType = (EntitaType)Enum.Parse(typeof(EntitaType), dr.GetString("REFERRAL_TYPE"));
            re.Cognome = dr.GetString("COGNOME");
            re.Nome = dr.GetString("NOME");
            re.CodiceFiscale = dr.GetString("COD_FIS");
            re.PartitaIVA = dr.GetString("P_IVA");
            re.RagioneSociale = dr.GetString("RAGIONE_SOCIALE");
            re.Ufficio = dr.GetString("UFFICIO");
            re.Note = dr.GetString("NOTE");
            re.RefIdAddress = (Nullable<Int64>)dr.GetValue("REF_ID_ADDRESS");
            re.IsIPA = (dr.IsDBNull("FLG_IPA") ? false : Convert.ToBoolean(int.Parse(dr.GetString("FLG_IPA"))));
            re.IPAdn = dr.GetString("IPA_DN");
            re.IPAId = (string)dr.GetValue("IPA_ID");
            re.DisambPre = dr.GetString("DISAMB_PRE");
            re.DisambPost = dr.GetString("DISAMB_POST");
            re.RefOrg = (Nullable<Int64>)dr.GetValue("REF_ORG");
            re.SitoWeb = dr.GetString("SITO_WEB");
            re.AffIPA = (Nullable<Int16>)dr.GetValue("AFF_IPA");
            return re;
        }

        internal static RubricaEntita MapIPAToRubricaEntita(IDataRecord dr)
        {
            RubricaEntita re = new RubricaEntita();
            re.IdReferral = (Nullable<Int64>)dr.GetDecimal("ID_RUB");
            re.Cognome = dr.GetString("COGNOME");
            re.Nome = dr.GetString("NOME");
            re.CodiceFiscale = dr.GetString("CODFIS");
            re.PartitaIVA = dr.GetString("PIVA");
            re.ReferralType = ModelHelper.ParseEnum<EntitaType>(dr.GetString("REFERRAL_TYPE"));
            re.RagioneSociale = dr.GetString("RAGIONESOCIALE");
            re.Ufficio = dr.GetString("UFFICIO");
            if (!dr.IsDBNull("MAIL") || !dr.IsDBNull("FAX") || !dr.IsDBNull("TELEFONO"))
            {
                re.Contatti = new List<RubricaContatti>();
                re.Contatti.Add(new RubricaContatti
                {
                    Mail = dr.GetString("MAIL"),
                    Fax = dr.GetString("FAX"),
                    Telefono = dr.GetString("TELEFONO")
                });

            }
            if (!dr.IsDBNull("STATO") || !dr.IsDBNull("PROVINCIA") || !dr.IsDBNull("CITTA")
                || !dr.IsDBNull("VIA") || !dr.IsDBNull("NUMERO") || !dr.IsDBNull("LETTERA")
                || !dr.IsDBNull("CAP"))
            {
                re.Address = new RubricaAddress
                {
                    Cap = dr.GetString("CAP"),
                    Civico = dr.GetString("NUMERO") + dr.GetString("LETTERA"),
                    CodIsoStato = dr.GetString("STATO"),
                    Comune = dr.GetString("CITTA"),
                    Indirizzo = string.Format("{0} {1}", dr.GetString("SEDIME"), dr.GetString("VIA")),
                    SiglaProvincia = dr.GetString("PROVINCIA")
                };
            }
            re.IPAId = dr.GetString("COD_UNIVOCO");
            re.Note = dr.GetString("NOTE");
            re.IsIPA = true;
            re.IPAdn = dr.GetString("DN");
            re.IdPadre = (Nullable<Int64>)dr.GetDecimal("ID_PADRE");
            return re;
        }

        internal static Titolo MapToTitolo(IDataRecord dr)
        {
            Titolo t = new Titolo();
            t.Id = dr.GetInt64("ID_TITOLO");
            t.Nome = dr.GetString("TITOLO");
            t.CodiceProtocollo = dr.GetString("PROT_CODE");
            t.AppCode = dr.GetString("APP_CODE");
            t.Note = dr.GetString("NOTE");

            decimal temp = dr.GetDecimal("ACTIVE");
            if (temp == 0)
                t.Deleted = true;
            else t.Deleted = false;

            return t;
        }

        internal static SimpleResultItem MapToSimpleResultItem(IDataRecord dr)
        {
            SimpleResultItem t = new SimpleResultItem();
            t.Value = dr["VALUE"].ToString();
            t.Text = dr["TEXT"].ToString();
            t.SubType = dr["SUBTYPE"].ToString();
            t.Source = dr["SOURCE"].ToString();
            t.Description = dr["DESCRIPTION"].ToString();
            t.SearchScore = dr.GetInt64("SCORE");
            return t;
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

        internal static Titolo MapToTitolo(IDataRecord dr, List<string> col)
        {
            Titolo t = new Titolo();
            if (col.Contains("ID_TITOLO"))
                t.Id = (Int32)dr.GetDecimal("ID_TITOLO");
            if (col.Contains("TITOLO"))
                t.Nome = dr.GetString("TITOLO");
            if (col.Contains("PROT_CODE"))
                t.CodiceProtocollo = dr.GetString("PROT_CODE");
            if (col.Contains("NOTE"))
                t.Note = dr.GetString("NOTE");
            if (col.Contains("APP_CODE"))
                t.AppCode = dr.GetString("APP_CODE");
            if (col.Contains("ACTIVE"))
            {
                decimal temp = dr.GetDecimal("ACTIVE");
                if (temp == 0)
                    t.Deleted = true;
                else t.Deleted = false;
            }
            return t;
        }
        /// <summary>
        /// Mappa un record su di un oggetto SottoTitolo.
        /// </summary>
        /// <param name="dr">datarecord</param>
        /// <returns></returns>
        internal static SottoTitolo MapToSottoTitolo(IDataRecord dr)
        {
            SottoTitolo st = new SottoTitolo();
            st.Id = (Int32)dr.GetDecimal("ID_SOTTOTITOLO");
            st.Titolo.Id = (Int32)dr.GetDecimal("REF_ID_TITOLO");
            st.Nome = dr.GetString("SOTTOTITOLO");
            st.ComCode = dr.GetString("COM_CODE");
            st.Note = dr.GetString("NOTE");
            if (dr.IsDBNull("ACTIVE"))
                st.Deleted = false;
            else
                st.Deleted = !Convert.ToBoolean(dr.GetDecimal("ACTIVE"));
            st.ProtocolloCode = dr.GetString("PROT_CODE");
            if (dr.IsDBNull("PROT_LOAD_ALLEGATI"))
                st.ProtocolloLoadAllegati = false;
            else
                st.ProtocolloLoadAllegati = Convert.ToBoolean(dr.GetDecimal("PROT_LOAD_ALLEGATI"));
            st.ProtocolloPassword = dr.GetString("PROT_PWD");
            st.ProtocolloSubCode = dr.GetString("PROT_SUBCODE");
            st.TipiProcollo = null;
            if (!dr.IsDBNull("PROT_TIPI_AMMESSI"))
            {
                st.TipiProcollo = (from t in dr.GetString("PROT_TIPI_AMMESSI").Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                   let exist = Enum.GetNames(typeof(ProtocolloTypes)).Contains(t)
                                   select ((exist) ? (ProtocolloTypes)Enum.Parse(typeof(ProtocolloTypes), t) : ProtocolloTypes.UNKNOWN)).ToList();

            }
            if (dr.IsDBNull("PROT_MANAGED"))
                st.UsaProtocollo = false;
            else
                st.UsaProtocollo = Convert.ToBoolean(dr.GetDecimal("PROT_MANAGED"));

            return st;
        }

        internal static BackEndRefCode MapToBackEndRefCode(IDataRecord dr)
        {
            BackEndRefCode st = new BackEndRefCode();
            st.Id = dr.GetDecimal("ID_BACKEND");
            st.Codice = dr.GetString("BACKEND_CODE");
            st.Descrizione = dr.GetString("BACKEND_DESCR");
            st.Categoria = dr.GetString("CATEGORY");
            return st;
        }

        /// <summary>
        /// Mappa un record su di un oggetto Nominativo in Rubrica.
        /// </summary>
        /// <param name="dr">datarecord</param>
        /// <returns></returns>
        //internal static Nominativo MapToNominativoRubrica(IDataRecord dr)
        //{
        //    return MapToNominativo(dr, "ID_CONTACT");
        //}
        /// <param name="dr">datarecord</param>
        /// <remarks>RIVEDERE</remarks>
        /// <summary> 
        /// Mappa un Dipartimento su di un oggetto BackendUser.
        /// </summary>/// <returns></returns>
        internal static BackendUser MapToDepartment(IDataRecord dr)
        {
            BackendUser bUser = new BackendUser();
            bUser.Department = dr.GetInt64("DEPARTMENT");
            return bUser;
        }

        /// <param name="dr">datarecord</param>
        /// <remarks>RIVEDERE</remarks>
        /// <summary> 
        /// Mappa un record su di un oggetto BackendUser.
        /// </summary>/// <returns></returns>
        internal static BackendUser MapToBackendUser(IDataRecord dr)
        {
            BackendUser bUser = new BackendUser();

            bUser.UserId = (long)dr.GetValue("ID_USER");
            bUser.UserName = dr.GetValue("USER_NAME").ToString();
            bUser.Department = dr.GetInt64("DEPARTMENT");

            if (dr.GetValue("MUNICIPIO") != null)
            {
                bUser.Municipio = dr.GetValue("MUNICIPIO").ToString();
            }
            else
            {
                bUser.Municipio = string.Empty;
            }

            bUser.Domain = dr.GetValue("DOMAIN").ToString();
            bUser.Cognome = dr.GetValue("COGNOME").ToString();
            bUser.Nome = dr.GetValue("NOME").ToString();
            bUser.UserRole = int.Parse(dr.GetValue("ROLE_USER").ToString());
            if (dr.GetValue("ROLE_MAIL") != null)
            {
                bUser.RoleMail = int.Parse(dr.GetValue("ROLE_MAIL").ToString());
            }

            return bUser;
        }

        /// <summary>
        /// Mappa un record in un oggetto SendersFolders
        /// Da controllare se c'è bisogno del campo Nome
        /// Ciro Cardone - 13/02/2014
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        internal static SendersFolders MapToSendersFolders(IDataRecord dr)
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
            me.MailRating = dr.GetInt32("FLG_RATING");
            me.HasAttachments = Convert.ToBoolean(int.Parse(dr.GetString("FLG_ATTACHMENTS")));
            me.Utente = dr.GetString("UTENTE");
            me.FolderId = dr.GetDecimal("FOLDERID");
            me.FolderTipo = dr.GetString("FOLDERTIPO");
            me.Dimensione = (int)dr.GetDecimal("msg_length");
            me.NomeFolder = dr.GetString("NOME");
            return me;
        }

        internal static ActiveUp.Net.Mail.Message MapToMailMessage(IDataRecord dr)
        {
            return ActiveUp.Net.Mail.Parser.ParseMessage(dr.GetString("MAIL_FILE"));
        }

        /// <summary>
        /// MODIFICATA PER GESTIONE FOLDERS
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        internal static System.Tuples.Tuple<ActiveUp.Net.Mail.Message, string, int, string> MapToMessageTuple(IDataRecord dr)
        {
            System.Tuples.Tuple<ActiveUp.Net.Mail.Message, string, int, string> tuple =
                new System.Tuples.Tuple<ActiveUp.Net.Mail.Message, string, int, string>();
            ActiveUp.Net.Mail.Message mex = new ActiveUp.Net.Mail.Message();
            if (!dr.IsDBNull("ID_MAIL"))
                mex.Id = (int)dr.GetInt64("ID_MAIL");
            mex.Uid = dr.GetString("MAIL_SERVER_ID");
            long lobSize = dr.GetChars(dr.GetOrdinal("MAIL_FILE"), 0, null, 0, 0);
            char[] fileChars = new char[lobSize];
            dr.GetChars(dr.GetOrdinal("MAIL_FILE"), 0, fileChars, 0, fileChars.Length);
            mex.OriginalData = Encoding.UTF8.GetBytes(fileChars);
            tuple.Element1 = mex;
            tuple.Element2 = dr.GetDecimal("FOLDERID").ToString();
            tuple.Element4 = dr.GetString("FOLDERTIPO").ToString();
            if (!dr.IsDBNull("FLG_RATING"))
                tuple.Element3 = dr.GetInt32("FLG_RATING");
            return tuple;
        }

        internal static Message MapOutboxToMailMessage(IDataRecord dr)
        {
            Encoding encoding = Codec.GetEncoding("iso-8859-1");
            ActiveUp.Net.Mail.Message msg = new ActiveUp.Net.Mail.Message();
            msg.Charset = encoding.BodyName;
            msg.ContentTransferEncoding = ContentTransferEncoding.QuotedPrintable;
            msg.ContentType = new ContentType();
            string appo = null;
            if (!String.IsNullOrEmpty(appo = dr.GetString("MAIL_CCN")))
            {
                msg.Bcc = Parser.ParseAddresses(appo);
                appo = null;
            }
            if (!String.IsNullOrEmpty(appo = dr.GetString("MAIL_CC")))
            {
                msg.Cc = Parser.ParseAddresses(appo);
                appo = null;
            }
            if (!String.IsNullOrEmpty(appo = dr.GetString("MAIL_TO")))
            {
                msg.To = Parser.ParseAddresses(appo);
                appo = null;
            }
            if (!String.IsNullOrEmpty(appo = dr.GetString("MAIL_TEXT")))
            {
                msg.BodyHtml = new ActiveUp.Net.Mail.MimeBody(ActiveUp.Net.Mail.BodyFormat.Html);
                msg.BodyHtml.Text = appo;
                appo = null;
            }
            msg.From = Parser.ParseAddress(dr.GetString("MAIL_SENDER"));
            msg.Id = (int)dr.GetInt64("ID_MAIL");
            msg.Subject = dr.GetString("MAIL_SUBJECT");

            if (!String.IsNullOrEmpty(appo = dr.GetString("ALLEG")))
            {
                var mime = appo.Split(new char[] { ';' }).Select(x =>
                {
                    ActiveUp.Net.Mail.MimePart mp = new ActiveUp.Net.Mail.MimePart();
                    string[] pp = x.Split('#');
                    mp.ContentId = pp[0];
                    mp.Filename = pp[1];
                    mp.ParentMessage = msg;
                    return mp;
                });
                foreach (var mm in mime)
                {
                    msg.Attachments.Add(mm);
                }
            }
            return msg;
        }


        internal static Folder MapToFolder(IDataRecord dr, List<ActiveUp.Net.Common.DeltaExt.Action> l)
        {
            Folder f = new Folder();
            f.Id = dr.GetDecimal("ID");
            f.Nome = dr.GetString("NOME");
            f.Abilitata = dr.GetString("SYSTEM");
            f.TipoFolder = dr.GetString("TIPO");
            f.IdNome = dr.GetDecimal("IDNOME").ToString();
            if (l != null)
            { f.Azioni = l; }
            return f;
        }

        internal static ActiveUp.Net.Common.DeltaExt.Action MapToAction(IDataRecord dr)
        {
            ActiveUp.Net.Common.DeltaExt.Action a = new ActiveUp.Net.Common.DeltaExt.Action();
            a.Id = dr.GetDecimal("ID");
            a.NomeAzione = dr.GetString("NOME_AZIONE");
            a.IdDestinazione = dr.GetDecimal("ID_NOME_DESTINAZIONE");
            a.NuovoStatus = dr.GetString("NUOVO_STATUS");
            a.TipoAzione = dr.GetString("TIPO_AZIONE");
            a.TipoDestinazione = dr.GetString("TIPO_DESTINAZIONE");
            if (dr.FieldCount > 7)
            { a.IdComp = dr.GetDecimal("IDFOLDER").ToString(); }
            a.IdFolderDestinazione = int.Parse(dr.GetDecimal("ID_FOLDER_DESTINAZIONE").ToString());
            return a;
        }

        internal static MailUser MapToMailUser(IDataRecord dr, MailServer s, List<Folder> l)
        {
            MailUser u = null;

            if (s != null)
                u = new MailUser(s);
            else u = new MailUser();
            u.UserId = dr.GetDecimal("ID_SENDER");
            u.Casella = dr.GetString("MAIL");
            u.Dominus = (s == null) ? "" : s.Dominus;
            u.EmailAddress = dr.GetString("MAIL");
            u.LoginId = dr.GetString("USERNAME");
            u.Password = dr.GetString("PASSWORD");
            u.FlgManaged = null;
            u.Folders = l;
            if (!dr.IsDBNull("FLG_MANAGED"))
                u.FlgManaged = int.Parse(dr.GetString("FLG_MANAGED"));
            //u.IsManaged = Convert.ToBoolean(int.Parse(dr.GetString("FLG_MANAGED")));
            return u;
        }
        /// <param name="dr">datarecord</param>
        /// <remarks>RIVEDERE</remarks>
        /// <summary> 
        /// Mappa un record su di un oggetto BackEndUserMailUserMapping.
        /// </summary>/// <returns></returns>
        internal static BackEndUserMailUserMapping MapToBackEndUserMailUserMapping(IDataRecord dr)
        {
            BackEndUserMailUserMapping b = new BackEndUserMailUserMapping();

            b.MailSenderId = (long)dr.GetValue("ID_SENDER");
            b.MailAccessLevel = int.Parse(dr.GetValue("ROLE_MAIL").ToString());

            return b;
        }

        /// <summary>
        /// Mappa un record su di un oggetto Nominativo.
        /// </summary>
        /// <param name="dr">datarecord</param>
        /// <remarks>RIVEDERE</remarks>
        /// <returns></returns>
        //internal static Nominativo MapToNominativo(IDataRecord dr, string keyField)
        //{
        //    Nominativo nr = (Nominativo)new NominativoFactory().CreateObject();
        //    nr.RubricaContatti = (Contatto)dr.GetValue("RUBR_CONTATTI");
        //    nr.RubricaEntita = (Entita)dr.GetValue("RUBR_ENTITA");
        //    nr.RubricaAddress = (Indirizzo)dr.GetValue("RUBR_ADDRESS");

        //    //nr.RubricaContatti.IdContact = (Int64)dr.GetDecimal(keyField);

        //    //nr.RubricaContatti.Nome = dr.GetString("NOME");
        //    //nr.Cognome = dr.GetString("COGNOME");
        //    //nr.CodiceFiscale = dr.GetString("CODFIS");

        //    //nr.RagioneSociale = dr.GetString("RAGIONESOCIALE");
        //    //nr.Ufficio = dr.GetString("UFFICIO");
        //    //nr.PartitaIVA = dr.GetString("PIVA");

        //    //nr.Fax = dr.GetString("FAX");
        //    //nr.Mail = dr.GetString("MAIL");
        //    //nr.Telefono = dr.GetString("TELEFONO");

        //    ///* INDIRIZZO */
        //    //nr.Recapito = new Indirizzo();
        //    //nr.Recapito.Stato = dr.GetString("STATO");
        //    //nr.Recapito.Provincia = dr.GetString("PROVINCIA");
        //    //nr.Recapito.Citta = dr.GetString("CITTA");
        //    //nr.Recapito.Sedime = dr.GetString("SEDIME");
        //    //nr.Recapito.Via = dr.GetString("VIA");
        //    //nr.Recapito.Numero = (Int32)dr.GetDecimal("NUMERO");
        //    //nr.Recapito.Lettera = dr.GetString("LETTERA");
        //    //nr.Recapito.CAP = dr.GetString("CAP");

        //    return nr;
        //}

        internal static ComAllegato MapToAllegatoComunicazione(IDataRecord dr)
        {
            ComAllegato alleg = new ComAllegato();
            if (dr.IsDBNull("ID_ALLEGATO"))
                alleg.IdAllegato = null;
            else
                alleg.IdAllegato = dr.GetInt64("ID_ALLEGATO");
            if (dr.IsDBNull("REF_ID_COM"))
                alleg.RefIdCom = null;
            else
                alleg.RefIdCom = dr.GetInt64("REF_ID_COM");
            alleg.AllegatoTpu = dr.GetString("ALLEGATO_TPU");
            alleg.AllegatoFile = dr.GetBytes("ALLEGATO_FILE");
            alleg.AllegatoExt = dr.GetString("ALLEGATO_EXT");
            alleg.FlgInsProt = (AllegatoProtocolloStatus)int.Parse(dr.GetString("FLG_INS_PROT"));
            alleg.FlgProtToUpl = (AllegatoProtocolloStatus)int.Parse(dr.GetString("FLG_PROT_TO_UPL"));
            //alleg.ProtRef = dr.GetString("PROT_REF");
            alleg.AllegatoName = dr.GetString("ALLEGATO_NAME");
            return alleg;
        }

        internal static BaseResultItem MapReferralTypeToResultItem(IDataRecord dr)
        {
            BaseResultItem res = new BaseResultItem();
            res.Value = dr.GetString("REFERRAL_TYPE");
            res.Description = dr.GetString("DESCRIPTION");
            return res;
        }

        internal static ContactsApplicationMapping MapToContactsApplicationMapping(IDataRecord dr)
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

        internal static ContactsBackendMap MapToContactsBackendMap(IDataRecord dr)
        {
            ContactsBackendMap cbm = new ContactsBackendMap();
            if (dr.IsDBNull("ID_MAP") == false)
                cbm.Id = (int)dr.GetDecimal("ID_MAP");
            else cbm.Id = -1;
            cbm.Canale = TipoCanale.UNKNOWN;
            if (dr.IsDBNull("REF_ID_CANALE") == false)
                cbm.Canale = (TipoCanale)(int)dr.GetDecimal("REF_ID_CANALE");
            if (dr.IsDBNull("REF_ID_BACKEND") == false)
                cbm.Backend = new BackEndRefCode { Id = dr.GetDecimal("REF_ID_BACKEND") };
            cbm.Contatto = new RubricaContatti();
            if (dr.IsDBNull("REF_ID_CONTATTO") == false)
                cbm.Contatto.IdContact = (long)dr.GetDecimal("REF_ID_CONTATTO");
            else
                cbm.Contatto.IdContact = -1;
            if (dr.IsDBNull("REF_ID_TITOLO") == false)
                cbm.Titolo = new Titolo { Id = dr.GetDecimal("REF_ID_TITOLO") };
            return cbm;
        }


        //internal static ActionsFolders MapToActionsFolders(IDataRecord r)
        //{
        //    ActionsFolders a = new ActionsFolders();
        //    a.IdAction =int.Parse(r.GetDecimal(0).ToString());
        //    if (dr.IsDBNull("ID_NOME_DESTINAZIONE") == false)
        //    { a.IdNomeDestinazione = int.Parse(r.GetDecimal(1).ToString()); }    
        //    a.FolderIdDestinazione = int.Parse(r.GetDecimal(2).ToString()); 
        //    a.Tipo = r.GetString(3);
        //    if (dr.IsDBNull("NUOVO_STATUS") == false)
        //    { a.NuovoStatus = r.GetString(4); }
        //    return a;
        //}

        internal static UserResultItem MapToUserResult(IDataRecord dr)
        {
            UserResultItem user = new UserResultItem();
            user.Account = dr.GetString("ACCOUNT");
            user.User = dr.GetString("UTE");
            user.Operazioni = dr.GetDecimal("TOT").ToString();
            return user;
        }
    }
}

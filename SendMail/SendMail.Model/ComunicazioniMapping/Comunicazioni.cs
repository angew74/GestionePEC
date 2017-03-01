using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ActiveUp.Net.Common.DeltaExt;
using System.Xml.Serialization;

namespace SendMail.Model.ComunicazioniMapping
{
    [Serializable]
    public class Comunicazioni : IDomainObject
    {
        #region IDomainObject Membri di
        public bool IsValid
        {
            get
            {
                return (this.MailComunicazione != null);
            }
        }

        public bool IsPersistent
        {
            get { return (this.IdComunicazione.HasValue && this.IdComunicazione.Value > 0); }
        }
        #endregion

        #region "Public Properties"

        #region "Pratica Properties"

        private string codiceIndividuale;

        public string CodiceIndividuale
        {
            get { return codiceIndividuale; }
            set { codiceIndividuale = value; }
        }

        private String annPrt;
        public String AnnPrt
        {
            get
            {
                return annPrt;
            }

        }

        private String numPrt;
        public String NumPrt
        {
            get
            {
                return numPrt;
            }
        }

        private String tipRic;
        public String TipRic
        {
            get
            {
                return tipRic;
            }
            set
            {
                tipRic = value;
            }
        }

        protected String stringaID;
        public String StringaID
        {
            get
            {
                return stringaID;
            }
            set
            {
                this.stringaID = value;
                if (!String.IsNullOrEmpty(value))
                {
                    this.OrigUID = value.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries)[0];
                }
            }
        }

        private String[] destinatariCode;
        public String[] DestinatariCode
        {
            get
            {
                return destinatariCode;
            }
            set
            {
                destinatariCode = value;
            }
        }
        #endregion

        #region "Comunicazioni Properties"
        public List<RubrEntitaUsed> RubricaEntitaUsed { get; set; }

        public virtual Nullable<Int64> RefIdSottotitolo { get; set; }

        public virtual String NoteTitolo { get; set; }

        public virtual String ComCode { get; set; }

        public Boolean IsToNotify { get; set; }

        public virtual String UtenteInserimento { get; set; }

        public virtual String MailNotifica { get; set; }

        public virtual int FolderId { get; set; }

        public virtual string FolderTipo { get; set; }

        [XmlIgnore]
        public Dictionary<TipoCanale, List<ComFlusso>> ComFlussi { get; set; }

        public List<DataItem<TipoCanale, List<ComFlusso>>> ComFlussoSerializable
        {
            get
            {
                if (ComFlussi == null) return null;
                return ComFlussi.Select(z => new DataItem<TipoCanale, List<ComFlusso>> { Key = z.Key, Value = z.Value }).ToList();
            }
            set
            {
                if (value == null)
                {
                    ComFlussi = null;
                }
                else
                {
                    ComFlussi = value.ToDictionary(z => z.Key, z => z.Value);
                }
            }
        }

        public List<ComFlussoProtocollo> ComFlussiProtocollo { get; set; }

        public virtual String Titolo { get; set; }

        public List<ComAllegato> ComAllegati { get; set; }

        public virtual String NoteSottotitolo { get; set; }

        public virtual String AppCode { get; set; }

        public virtual String Sottotitolo { get; set; }

        public MailContent MailComunicazione { get; set; }

        public virtual String CodAppInserimento { get; set; }

        public virtual Nullable<Int64> IdComunicazione { get; set; }

        public virtual String OrigUID { get; set; }

        public Boolean DaProtocollare { get; set; }

        public TipoComunicazione TipoCom { get; set; }

        public string UniqueId { get; set; }

        public DateTime? Delay { get; set; }

        public virtual ComunicazioniProtocollo ComunicazioniProtocollo { get; set; }
        #endregion
        #endregion

        #region "C.tor"

        public Comunicazioni()
        {
        }

        public Comunicazioni(String annoPrt, String numeroPrt, String tipoRic, String strId, String destCodes)
        {
            this.annPrt = annoPrt;
            this.numPrt = numeroPrt;
            this.TipRic = tipoRic;
            this.StringaID = strId;
            //if (!String.IsNullOrEmpty(strId))
            //{
            //    this.OrigUID = strId.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries)[0];
            //}
            this.destinatariCode = destCodes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public Comunicazioni(String annPrt, String numPrt, String tipRic, String strId, String destCodes,
            String senderMail, TipoCanale canaleType, String mailText, ICollection<ComAllegato> allegati,int folderId, string folderTipo)
            : this(annPrt, numPrt, tipRic, strId, destCodes)
        {
            this.ComFlussi = new Dictionary<TipoCanale, List<ComFlusso>>();
            this.ComFlussi.Add(canaleType, null);
            this.ComAllegati = (List<ComAllegato>)allegati;
            this.FolderTipo = folderTipo;
            this.FolderId = folderId;

            switch (canaleType)
            {
                case TipoCanale.MAIL:
                    this.MailComunicazione = new MailContent();
                    this.MailComunicazione.MailSender = senderMail;
                    this.MailComunicazione.MailText = mailText;
                    this.MailComunicazione.HasCustomRefs = false;
                    break;

                default:
                    break;
            }
        }

        public Comunicazioni(TipoCanale canaleType, List<DataRow> _list)
        {
            this.ComFlussi = new Dictionary<TipoCanale, List<ComFlusso>>();
            this.ComFlussi.Add(canaleType, null);
            if (_list[0].Table.Columns.Contains("ANN_PRT"))
            {
                this.annPrt = _list[0].Field<string>("ANN_PRT");
                this.numPrt = _list[0].Field<string>("NUM_PRT");
            }
            if (_list[0].Table.Columns.Contains("COD_IND"))
            { this.CodiceIndividuale = _list[0].Field<string>("COD_IND"); }
            this.ComCode = _list[0].Field<String>("TIP_RIC");
            this.StringaID = _list[0].Field<String>("STRINGA_ID");
            this.destinatariCode = _list[0].Field<String>("COD_DESTINATARIO").Split(';');
            string oggetto = string.Empty;
            List<ComAllegato> lAll = new List<ComAllegato>();
            StringBuilder dati = new StringBuilder();
            System.Collections.Generic.Dictionary<int, string> prus = new System.Collections.Generic.Dictionary<int, string>();
            System.Collections.Generic.Dictionary<int, string> tpus = new System.Collections.Generic.Dictionary<int, string>();

            if (_list == null && _list.Count == 0)
            {
                this.ComAllegati = null;
            }
            else
            {

                for (int i = 0; i < _list.Count; i++)
                {
                    if ((_list[i].Field<String>("NOME_TPU").ToUpper().Trim() == "TESTO_MAIL") || (_list[i].Field<String>("NOME_TPU").ToUpper().Trim() == "TESTO_MAIL_ELE"))
                        dati.Append(_list[i].Field<String>("DATI_PRU"));
                    if (_list[i].Field<string>("NOME_TPU").ToUpper().Trim() == "OGGETTO_MAIL_ELE")
                    {
                        oggetto += _list[i].Field<string>("DATI_PRU");
                        continue;
                    }

                    if (!(string.IsNullOrEmpty(_list[i].Field<String>("PROG_PRU")) || _list[i].Field<String>("NOME_TPU").ToLower().Equals("testo_mail") || _list[i].Field<String>("NOME_TPU").ToLower().Equals("testo_mail_ele")))
                    {
                        if (!prus.ContainsKey(int.Parse(_list[i].Field<String>("PROG_TPU").Trim())))
                        {
                            prus.Add(int.Parse(_list[i].Field<String>("PROG_TPU").Trim()), string.Empty);
                            tpus.Add(int.Parse(_list[i].Field<String>("PROG_TPU").Trim()), _list[i].Field<String>("NOME_TPU"));
                        }
                        prus[int.Parse(_list[i].Field<String>("PROG_TPU").Trim())] = prus[int.Parse(_list[i].Field<String>("PROG_TPU").Trim())] + _list[i].Field<String>("DATI_PRU");
                    }
                }

                for (int j = 0; j < tpus.Count; j++)
                {
                    ComAllegato a = new ComAllegato();
                    a.IdAllegato = null;
                    a.RefIdCom = null;
                    a.AllegatoTpu = tpus[j + 1];
                    a.T_Progr = j;
                    a.AllegatoExt = "PRU";
                    a.AllegatoFile = Encoding.GetEncoding("ISO-8859-1").GetBytes(prus[j + 1]);
                    a.AllegatoFile = new UTF8Encoding(false).GetBytes(prus[j + 1]);
                    string[] cmp = tpus[j + 1].Split('.');
                    a.AllegatoName = String.Format("{0}_{1}", String.Join(".", cmp, 0, cmp.Length - 1), (j + 1).ToString("D3"));
                    lAll.Add(a);
                }
                this.ComAllegati = lAll;
            }

            switch (canaleType)
            {
                case TipoCanale.MAIL:
                    this.MailComunicazione = new MailContent();
                    this.MailComunicazione.MailSender = _list[0].Field<String>("EMAIL");
                    this.MailComunicazione.MailText = dati.ToString();
                    this.MailComunicazione.HasCustomRefs = false;
                    if (!string.IsNullOrEmpty(oggetto))
                        this.MailComunicazione.MailSubject = oggetto.Trim();
                    break;

                default:
                    break;
            }
        }

        public Comunicazioni(TipoCanale canaleType, String sottoTitolo, ActiveUp.Net.Mail.Message msg, String utente,int folderId,string folderTipo)
        {
            #region "sottotitolo"

            if (!String.IsNullOrEmpty(sottoTitolo))
                this.RefIdSottotitolo = Convert.ToInt64(sottoTitolo);

            #endregion

            #region "mail"

            MailContent mail = new MailContent();
            this.MailComunicazione = mail;

            #region "sender"

            mail.MailSender = msg.From.Email;

            #endregion

            #region "refs"
            mail.MailRefs = new List<MailRefs>();
            List<MailRefs> lR = new List<MailRefs>();
            var to = from to0 in msg.To
                     select new MailRefs
                     {
                         MailDestinatario = to0.Email,
                         TipoRef = AddresseeType.TO
                     };
            if (to.Count() > 0) lR.AddRange(to);
            var cc = from cc0 in msg.Cc
                     select new MailRefs
                     {
                         MailDestinatario = cc0.Email,
                         TipoRef = AddresseeType.CC
                     };
            if (cc.Count() > 0) lR.AddRange(cc);
            var ccn = from ccn0 in msg.Bcc
                      select new MailRefs
                      {
                          MailDestinatario = ccn0.Email,
                          TipoRef = AddresseeType.CCN
                      };
            if (ccn.Count() > 0) lR.AddRange(ccn);
            if (lR.Count > 0) mail.MailRefs = lR;

            #endregion

            #region "subject"

            mail.MailSubject = msg.Subject;

            #endregion

            #region "body"

            mail.MailText = !String.IsNullOrEmpty(msg.BodyHtml.Text) ? msg.BodyHtml.Text : msg.BodyText.Text;

            #endregion

            #region "reply"
            if (!String.IsNullOrEmpty(msg.InReplyTo) && !String.IsNullOrEmpty(msg.InReplyTo.Trim()))
            {
                string follows = msg.InReplyTo.Trim();
                if (follows.StartsWith("<"))
                    follows = follows.Substring(1);
                if (follows.EndsWith(">"))
                    follows = follows.Substring(0, follows.Length - 1);
                long? flw = null;
                long flwout = 0;
                if (long.TryParse(follows.Split('.')[0], out flwout))
                {
                    flw = flwout;
                }
                mail.Follows = flw;
            }
            #endregion
            #endregion

            #region "attachments"

            List<ComAllegato> all = null;
            if (msg.Attachments.Count > 0)
            {
                all = new List<ComAllegato>();

                for (int j = 0; j < msg.Attachments.Count; j++)
                {
                    ActiveUp.Net.Mail.MimePart mp = msg.Attachments[j];
                    ComAllegato a = new ComAllegato();
                    a.IdAllegato = null;
                    a.AllegatoExt = System.IO.Path.GetExtension(mp.Filename);
                    a.AllegatoExt = a.AllegatoExt.Replace(".", string.Empty);
                    a.AllegatoFile = mp.BinaryContent;
                    a.AllegatoName = System.IO.Path.GetFileNameWithoutExtension(mp.Filename);
                    a.T_Progr = j;
                    a.FlgInsProt = AllegatoProtocolloStatus.UNKNOWN;
                    a.FlgProtToUpl = AllegatoProtocolloStatus.UNKNOWN;
                    a.RefIdCom = null;

                    all.Add(a);
                }
            }
            this.ComAllegati = all;

            #endregion

            #region "notifica"

            this.MailNotifica = msg.ReplyTo.Email;

            #endregion

            #region "utente inserimento"

            this.UtenteInserimento = utente;

            #endregion

            #region "flusso comunicazione"

            this.SetStatoComunicazione(canaleType, MailStatus.INSERTED, utente);

            #endregion

            #region Folder
            this.FolderId = folderId;
            this.FolderTipo = folderTipo;

            #endregion
        }

        public Comunicazioni(Comunicazioni c)
        {
            if (c == null) return;

            Type t = this.GetType();
            foreach (System.Reflection.PropertyInfo p in t.GetProperties())
            {
                if (p.CanWrite && p.CanRead)
                {
                    try
                    {
                        p.SetValue(this, p.GetValue(c, null), null);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
        #endregion

        #region "Public Methods"

        public void SetStatoComunicazione(TipoCanale tipoCanale, MailStatus statoCom, String utenteOperazione)
        {
            List<ComFlusso> flussoCom = null;
            if (this.ComFlussi == null)
                this.ComFlussi = new Dictionary<TipoCanale, List<ComFlusso>>();

            if (this.ComFlussi.ContainsKey(tipoCanale))
                flussoCom = this.ComFlussi[tipoCanale];

            ComFlusso cf = new ComFlusso();
            cf.Canale = tipoCanale;
            cf.RefIdComunicazione = this.IdComunicazione;
            if (flussoCom != null)
            {
                cf.StatoComunicazioneOld = this.ComFlussi[tipoCanale].Last().StatoComunicazioneNew;
            }
            else
            {
                cf.StatoComunicazioneOld = MailStatus.UNKNOWN;
            }
            cf.StatoComunicazioneNew = statoCom;
            cf.UtenteOperazione = utenteOperazione;
            if (flussoCom == null)
            {
                List<ComFlusso> l = new List<ComFlusso>();
                l.Add(cf);
                this.ComFlussi.Add(tipoCanale, l);
            }
            else
            {
                this.ComFlussi[tipoCanale].Add(cf);
            }
        }

        public void SetMailDestinatari(IEnumerable<ContactsApplicationMapping> rows, AddresseeType addType)
        {
            if (rows != null && rows.Count() > 0)
            {
                if (this.RubricaEntitaUsed == null)
                    this.RubricaEntitaUsed = new List<RubrEntitaUsed>();
                try
                {
                    rows.Select(cnt =>
                    {
                        RubrEntitaUsed r = new RubrEntitaUsed();
                        if (cnt.IdContact > 0)
                        {
                            r.IdEntUsed = cnt.IdContact;
                        }
                        else
                        {
                            r.IdEntUsed = null;
                        }

                        r.Mail = cnt.Mail;
                        r.Fax = cnt.Fax;
                        r.Telefono = cnt.Telefono;
                        if (cnt.RefIdReferral > 0)
                        {
                            r.IdReferral = cnt.RefIdReferral;
                        }
                        else
                        {
                            r.IdReferral = null;
                        }
                        r.TipoContatto = addType;
                        return r;
                    }).ToList().ForEach(re => this.RubricaEntitaUsed.Add(re));
                }
                catch
                {
                    this.RubricaEntitaUsed = null;
                }

                this.AppCode = this.CodAppInserimento = rows.ElementAt(0).AppCode;
                this.ComCode = rows.ElementAt(0).ComCode;
                this.RefIdSottotitolo = rows.ElementAt(0).IdSottotitolo;
                this.Sottotitolo = rows.ElementAt(0).Sottotitolo;
                this.Titolo = rows.ElementAt(0).Titolo;
            }
        }

        #endregion
    }
}

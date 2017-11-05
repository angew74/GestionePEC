using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using AutoMapper;
using SendMail.Model;
using SendMail.Model.ComunicazioniMapping;
using SendMail.Model.ContactApplicationMapping;
using SendMail.Model.RubricaMapping;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Mapping
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Comunicazioni, COMUNICAZIONI>().ForMember(d => d.COD_APP_INS,
                opt => opt.MapFrom(c => c.AppCode))
                .ForMember(d => d.ID_COM, opt => opt.MapFrom(c => c.IdComunicazione))
                .ForMember(d => d.ORIG_UID, opt => opt.MapFrom(c => c.OrigUID))
                .ForMember(d => d.REF_ID_SOTTOTITOLO, opt => opt.MapFrom(c => c.RefIdSottotitolo))
                .ForMember(d => d.UNIQUE_ID_MAPPER, opt => opt.MapFrom(c => c.UniqueId))
                .ForMember(d => d.UTE_INS, opt => opt.MapFrom(c => c.UtenteInserimento))
                .ForMember(d => d.MAIL_NOTIFICA, opt => opt.MapFrom(c => c.MailNotifica))
                .ForMember(d => d.COD_APP_INS, opt => opt.MapFrom(c => c.CodAppInserimento))
                .ForMember(d => d.COMUNICAZIONI_ALLEGATI, opt => opt.MapFrom(c => c.ComAllegati))
                .ForMember(d => d.COMUNICAZIONI_ENTITA_USED, opt => opt.MapFrom(c => c.RubricaEntitaUsed))
                .ForMember(d => d.COMUNICAZIONI_FLUSSO, opt => opt.MapFrom(c => c.ComFlussi))
                .ForMember(d => d.COMUNICAZIONI_FLUSSO_PROT, opt => opt.MapFrom(c => c.ComFlussiProtocollo))
                .ForMember(d => d.MAIL_CONTENT, opt => opt.MapFrom(c => c.MailComunicazione))
                );
            Mapper.Initialize(cfg => cfg.CreateMap<MailContent, MAIL_CONTENT>().ForMember(d => d.FOLLOWS, opt => opt.MapFrom(c => c.Follows))
                .ForMember(d => d.ID_MAIL, opt => opt.MapFrom(c => c.IdMail))
                .ForMember(d => d.MAIL_SENDER, opt => opt.MapFrom(c => c.MailSender))
                .ForMember(d => d.MAIL_SUBJECT, opt => opt.MapFrom(c => c.MailSubject))
                .ForMember(d => d.MAIL_TEXT, opt => opt.MapFrom(c => c.MailText))
                .ForMember(d => d.REF_ID_COM, opt => opt.MapFrom(c => c.RefIdComunicazione)));          
            Mapper.Initialize(cfg => cfg.CreateMap<ComFlusso, COMUNICAZIONI_FLUSSO>().ForMember(d => d.CANALE, opt => opt.MapFrom(c => c.Canale))
              .ForMember(d => d.DATA_OPERAZIONE, opt => opt.MapFrom(c => c.DataOperazione))
              .ForMember(d => d.ID_FLUSSO, opt => opt.MapFrom(c => c.IdFlusso))
              .ForMember(d => d.REF_ID_COM, opt => opt.MapFrom(c => c.RefIdComunicazione))
              .ForMember(d => d.STATO_COMUNICAZIONE_NEW, opt => opt.MapFrom(c => c.StatoComunicazioneNew))
              .ForMember(d => d.STATO_COMUNICAZIONE_OLD, opt => opt.MapFrom(c => c.StatoComunicazioneOld))
              .ForMember(d => d.UTE_OPE, opt => opt.MapFrom(c => c.UtenteOperazione))
              );
            Mapper.Initialize(cfg => cfg.CreateMap<ComFlussoProtocollo, COMUNICAZIONI_FLUSSO_PROT>().ForMember(d => d.DATA_OPERAZIONE, opt => opt.MapFrom(c => c.DataOperazione))
           .ForMember(d => d.REF_ID_COM, opt => opt.MapFrom(c => c.RefIdComunicazione))
           .ForMember(d => d.STATO_NEW, opt => opt.MapFrom(c => c.StatoNew))
           .ForMember(d => d.STATO_OLD, opt => opt.MapFrom(c => c.StatoOld))
           .ForMember(d => d.UTE_OPE, opt => opt.MapFrom(c => c.UtenteOperazione))
           );
            Mapper.Initialize(cfg => cfg.CreateMap<Titolo, COMUNICAZIONI_TITOLI>().ForMember(d => d.APP_CODE, opt => opt.MapFrom(c => c.AppCode))
            .ForMember(d => d.ID_TITOLO, opt => opt.MapFrom(c => c.Id))
            .ForMember(d => d.NOTE, opt => opt.MapFrom(c => c.Note))
            .ForMember(d => d.PROT_CODE, opt => opt.MapFrom(c => c.CodiceProtocollo))
            );
            Mapper.Initialize(cfg => cfg.CreateMap<SottoTitolo, COMUNICAZIONI_SOTTOTITOLI>().ForMember(d => d.COM_CODE, opt => opt.MapFrom(c => c.ComCode))
              .ForMember(d => d.ID_SOTTOTITOLO, opt => opt.MapFrom(c => c.Id))
              .ForMember(d => d.NOTE, opt => opt.MapFrom(c => c.Note))
              .ForMember(d => d.PROT_CODE, opt => opt.MapFrom(c => c.CodiceProtocollo))
              .ForMember(d => d.REF_ID_TITOLO, opt => opt.MapFrom(c => c.RefIdTitolo))
              );
            Mapper.Initialize(cfg => cfg.CreateMap<Titolo, COMUNICAZIONI_TITOLI>().ForMember(d => d.APP_CODE, opt => opt.MapFrom(c => c.AppCode))
              .ForMember(d => d.ID_TITOLO, opt => opt.MapFrom(c => c.Id))
              .ForMember(d => d.NOTE, opt => opt.MapFrom(c => c.Note))
              .ForMember(d => d.PROT_CODE, opt => opt.MapFrom(c => c.CodiceProtocollo))
              );
            Mapper.Initialize(cfg => cfg.CreateMap<ComAllegato, COMUNICAZIONI_ALLEGATI>().ForMember(d => d.ALLEGATO_EXT, opt => opt.MapFrom(c => c.AllegatoExt))
                 .ForMember(d => d.ALLEGATO_FILE, opt => opt.MapFrom(c => c.AllegatoFile))
                 .ForMember(d => d.ALLEGATO_TPU, opt => opt.MapFrom(c => c.AllegatoTpu))
                 .ForMember(d => d.FLG_INS_PROT, opt => opt.MapFrom(c => c.FlgInsProt))
                 .ForMember(d => d.FLG_PROT_TO_UPL, opt => opt.MapFrom(c => c.FlgProtToUpl))
                 .ForMember(d => d.ID_ALLEGATO, opt => opt.MapFrom(c => c.IdAllegato))
                 .ForMember(d => d.REF_ID_COM, opt => opt.MapFrom(c => c.RefIdCom))
                 );
            // Mapping Inverso
            Mapper.Initialize(cfg => cfg.CreateMap<COMUNICAZIONI, Comunicazioni>().ForMember(d => d.AppCode,
               opt => opt.MapFrom(c => c.COD_APP_INS))
               .ForMember(d => d.IdComunicazione, opt => opt.MapFrom(c => c.ID_COM))
               .ForMember(d => d.OrigUID, opt => opt.MapFrom(c => c.ORIG_UID))
               .ForMember(d => d.RefIdSottotitolo, opt => opt.MapFrom(c => c.REF_ID_SOTTOTITOLO))
               .ForMember(d => d.UniqueId, opt => opt.MapFrom(c => c.UNIQUE_ID_MAPPER))
               .ForMember(d => d.UtenteInserimento, opt => opt.MapFrom(c => c.UTE_INS))
               .ForMember(d => d.MailNotifica, opt => opt.MapFrom(c => c.MAIL_NOTIFICA))
               .ForMember(d => d.CodAppInserimento, opt => opt.MapFrom(c => c.COD_APP_INS))
               .ForMember(d => d.ComAllegati, opt => opt.MapFrom(c => c.COMUNICAZIONI_ALLEGATI))
               .ForMember(d => d.RubricaEntitaUsed, opt => opt.MapFrom(c => c.COMUNICAZIONI_ENTITA_USED))
               .ForMember(d => d.ComFlussi, opt => opt.MapFrom(c => c.COMUNICAZIONI_FLUSSO))
               .ForMember(d => d.ComFlussiProtocollo, opt => opt.MapFrom(c => c.COMUNICAZIONI_FLUSSO_PROT))
               .ForMember(d => d.MailComunicazione, opt => opt.MapFrom(c => c.MAIL_CONTENT))
               );
            Mapper.Initialize(cfg => cfg.CreateMap<MAIL_CONTENT, MailContent>().ForMember(d => d.Follows, opt => opt.MapFrom(c => c.FOLLOWS))
                .ForMember(d => d.IdMail, opt => opt.MapFrom(c => c.ID_MAIL))
                .ForMember(d => d.MailSender, opt => opt.MapFrom(c => c.MAIL_SENDER))
                .ForMember(d => d.MailSubject, opt => opt.MapFrom(c => c.MAIL_SUBJECT))
                .ForMember(d => d.MailText, opt => opt.MapFrom(c => c.MAIL_TEXT))
                .ForMember(d => d.RefIdComunicazione, opt => opt.MapFrom(c => c.REF_ID_COM)));
                  
            Mapper.Initialize(cfg => cfg.CreateMap<ComFlusso, COMUNICAZIONI_FLUSSO>().ForMember(d => d.CANALE, opt => opt.MapFrom(c => c.Canale))
              .ForMember(d => d.DATA_OPERAZIONE, opt => opt.MapFrom(c => c.DataOperazione))
              .ForMember(d => d.ID_FLUSSO, opt => opt.MapFrom(c => c.IdFlusso))
              .ForMember(d => d.REF_ID_COM, opt => opt.MapFrom(c => c.RefIdComunicazione))
              .ForMember(d => d.STATO_COMUNICAZIONE_NEW, opt => opt.MapFrom(c => c.StatoComunicazioneNew))
              .ForMember(d => d.STATO_COMUNICAZIONE_OLD, opt => opt.MapFrom(c => c.StatoComunicazioneOld))
              .ForMember(d => d.UTE_OPE, opt => opt.MapFrom(c => c.UtenteOperazione))
              );
            Mapper.Initialize(cfg => cfg.CreateMap<ComFlussoProtocollo, COMUNICAZIONI_FLUSSO_PROT>().ForMember(d => d.DATA_OPERAZIONE, opt => opt.MapFrom(c => c.DataOperazione))
           .ForMember(d => d.REF_ID_COM, opt => opt.MapFrom(c => c.RefIdComunicazione))
           .ForMember(d => d.STATO_NEW, opt => opt.MapFrom(c => c.StatoNew))
           .ForMember(d => d.STATO_OLD, opt => opt.MapFrom(c => c.StatoOld))
           .ForMember(d => d.UTE_OPE, opt => opt.MapFrom(c => c.UtenteOperazione))
           );
            Mapper.Initialize(cfg => cfg.CreateMap<Titolo, COMUNICAZIONI_TITOLI>().ForMember(d => d.APP_CODE, opt => opt.MapFrom(c => c.AppCode))
            .ForMember(d => d.ID_TITOLO, opt => opt.MapFrom(c => c.Id))
            .ForMember(d => d.NOTE, opt => opt.MapFrom(c => c.Note))
            .ForMember(d => d.PROT_CODE, opt => opt.MapFrom(c => c.CodiceProtocollo))
            );
            Mapper.Initialize(cfg => cfg.CreateMap<SottoTitolo, COMUNICAZIONI_SOTTOTITOLI>().ForMember(d => d.COM_CODE, opt => opt.MapFrom(c => c.ComCode))
              .ForMember(d => d.ID_SOTTOTITOLO, opt => opt.MapFrom(c => c.Id))
              .ForMember(d => d.NOTE, opt => opt.MapFrom(c => c.Note))
              .ForMember(d => d.PROT_CODE, opt => opt.MapFrom(c => c.CodiceProtocollo))
              .ForMember(d => d.REF_ID_TITOLO, opt => opt.MapFrom(c => c.RefIdTitolo))
              );
            Mapper.Initialize(cfg => cfg.CreateMap<Titolo, COMUNICAZIONI_TITOLI>().ForMember(d => d.APP_CODE, opt => opt.MapFrom(c => c.AppCode))
              .ForMember(d => d.ID_TITOLO, opt => opt.MapFrom(c => c.Id))
              .ForMember(d => d.NOTE, opt => opt.MapFrom(c => c.Note))
              .ForMember(d => d.PROT_CODE, opt => opt.MapFrom(c => c.CodiceProtocollo))
              );
            Mapper.Initialize(cfg => cfg.CreateMap<ComAllegato, COMUNICAZIONI_ALLEGATI>().ForMember(d => d.ALLEGATO_EXT, opt => opt.MapFrom(c => c.AllegatoExt))
                 .ForMember(d => d.ALLEGATO_FILE, opt => opt.MapFrom(c => c.AllegatoFile))
                 .ForMember(d => d.ALLEGATO_TPU, opt => opt.MapFrom(c => c.AllegatoTpu))
                 .ForMember(d => d.FLG_INS_PROT, opt => opt.MapFrom(c => c.FlgInsProt))
                 .ForMember(d => d.FLG_PROT_TO_UPL, opt => opt.MapFrom(c => c.FlgProtToUpl))
                 .ForMember(d => d.ID_ALLEGATO, opt => opt.MapFrom(c => c.IdAllegato))
                 .ForMember(d => d.REF_ID_COM, opt => opt.MapFrom(c => c.RefIdCom))
                 );

        }

        internal static RubricaContatti MapToRubrContatti(RUBR_CONTATTI r)
        {
            RubricaContatti c = new RubricaContatti();
            c.Entita = AutoMapperConfiguration.MapToRubrEntita(r.RUBR_ENTITA);
            if (r.AFF_IPA != null)
            { c.AffIPA = (short)r.AFF_IPA; }
            if (r.CONTACT_REF != null)
            { c.ContactRef = r.CONTACT_REF; }
            c.Fax = r.FAX;
            c.IdContact = (long)r.ID_CONTACT;
            c.IPAdn = r.IPA_DN;
            c.IPAId = r.IPA_ID;
            c.IsIPA = (r.FLG_IPA == "0") ? false : true;
            c.IsPec = (r.FLG_PEC == 0) ? false : true;
            c.Mail = r.MAIL;
            c.Note = r.NOTE;
            c.RefIdReferral = (long)r.REF_ID_REFERRAL;
            c.Src = "R";
            c.Source = null;
            c.Telefono = r.TELEFONO;
            c.T_Ufficio = r.RUBR_ENTITA.UFFICIO;
            c.T_RagioneSociale = r.RUBR_ENTITA.RAGIONE_SOCIALE;
            return c;
        }

        internal static Folder MapToFolder(FOLDERS a)
        {
            Folder folder = new Folder()
            {
                Id = a.ID,
                IdNome = a.IDNOME.ToString(),
                Nome = a.NOME,
                TipoFolder = a.SYSTEM                
            };
            return folder;
        }

        internal static ActiveUp.Net.Common.DeltaExt.Action MapToAction(ACTIONS a)
        {
            ActiveUp.Net.Common.DeltaExt.Action action = new ActiveUp.Net.Common.DeltaExt.Action()
            {
                Id = (decimal)a.ID,
                IdDestinazione = (decimal)a.ID_NOME_DESTINAZIONE,
                IdFolderDestinazione = (int)a.ID_FOLDER_DESTINAZIONE,
                TipoAzione = a.TIPO_AZIONE,
                NuovoStatus = a.NUOVO_STATUS,
                TipoDestinazione = a.TIPO_DESTINAZIONE,
                NomeAzione = a.NOME_AZIONE
            };
            return action;
        }

        internal static MAILSERVERS FromMailServerToDto(MailServer r, bool isInsert)
        {
            MAILSERVERS m = new MAILSERVERS()
            {
                NOME = r.DisplayName,
                INDIRIZZO_IN = r.IncomingServer,
                PROTOCOLLO_IN = r.IncomingProtocol,
                PORTA_IN = r.PortIncomingServer.ToString(),
                SSL_IN = r.IsIncomeSecureConnection.ToString(),
                INDIRIZZO_OUT = r.OutgoingServer,
                PORTA_OUT = r.PortOutgoingServer.ToString(),
                SSL_OUT = r.IsOutgoingSecureConnection.ToString(),
                AUTH_OUT = r.IsOutgoingWithAuthentication.ToString(),
                DOMINUS = r.Dominus
            };
           if (r.IsPec)
            {
                m.FLG_ISPEC = "1";
            }
            else if (!r.IsPec)
            {
                m.FLG_ISPEC = "0";                
            }

            if (!isInsert)
            { m.ID_SVR = (double)r.Id; }
            return m;
        }

        internal static COMUNICAZIONI_TITOLI MapToComunicazioniTitoli(Titolo titolo,bool isInsert)
        {
            COMUNICAZIONI_TITOLI t = new COMUNICAZIONI_TITOLI()
            {
                TITOLO = titolo.Nome,
                APP_CODE = titolo.AppCode,
                ACTIVE = LinqExtensions.TryParseDecimalBool(titolo.Deleted),
                NOTE = titolo.Note,
                PROT_CODE = titolo.CodiceProtocollo
            };
            if(!isInsert)
            { t.ID_TITOLO = titolo.Id; }
            return t;
        }

        internal static Titolo MapSottotitoliToTitoli(COMUNICAZIONI_SOTTOTITOLI s)
        {
            Titolo a = new Titolo();
            a.AppCode = s.COM_CODE;
            a.CodiceProtocollo = s.PROT_CODE;
            a.Id = s.ID_SOTTOTITOLO;
            a.Nome = s.SOTTOTITOLO;
            a.Note = s.NOTE;
            decimal temp = s.ACTIVE;
            if (temp == 0)
                  a.Deleted = true;
                else a.Deleted = false;            
            return a;
        }

        internal static RubricaContatti MapToRubrContatti(RUBR_CONTATTI r, List<decimal> ListTitoli)
        {
            RubricaContatti c = new RubricaContatti();
            c.Entita = AutoMapperConfiguration.MapToRubrEntita(r.RUBR_ENTITA);
            if (r.AFF_IPA != null)
            { c.AffIPA = (short)r.AFF_IPA; }
            if (r.CONTACT_REF != null)
            { c.ContactRef = r.CONTACT_REF; }
            c.Fax = r.FAX;
            c.IdContact =(long) r.ID_CONTACT;
            c.IPAdn = r.IPA_DN;
            c.IPAId = r.IPA_ID;
            c.IsIPA = (r.FLG_IPA == "0") ? false : true;
            c.IsPec = (r.FLG_PEC == 0) ? false : true;
            c.Mail = r.MAIL;
            c.Note = r.NOTE;
            c.RefIdReferral =(long) r.REF_ID_REFERRAL;
            c.Src = "R";
            c.Source = null;
            c.Telefono = r.TELEFONO;
            c.T_Ufficio = r.RUBR_ENTITA.UFFICIO;
            c.T_RagioneSociale = r.RUBR_ENTITA.RAGIONE_SOCIALE;
            c.MappedAppsId = new List<long>();
            foreach (int i in ListTitoli)
            {
                if (!(c.MappedAppsId.Contains(i)))
                { c.MappedAppsId.Add(i); }
            }

            return c;
        }

        internal  static ContactsApplicationMapping MapToContactsApplicationModel(V_MAP_APPL_CONTATTI_NEW v)
        {
            ContactsApplicationMapping m = new ContactsApplicationMapping();
            m.AppCode = v.APP_CODE;
            m.BackendCode = v.BACKEND_CODE;
            m.BackendDescr = v.BACKEND_DESCR;
            m.Category = v.CATEGORY;
            m.Codice = v.CODICE;
            m.ComCode = v.CODICE;
            m.DescrPlus = v.DESCR_PLUS;
            m.Fax = v.FAX;
            m.IdBackend = (long)v.ID_BACKEND;
            m.IdCanale = (long)v.ID_CANALE;
            m.IdContact = (long)v.ID_CONTACT;
            m.IdMap = (long)v.ID_MAP;
            m.IdSottotitolo = (long)v.ID_SOTTOTITOLO;
            m.IdTitolo = (long)v.ID_TITOLO;
            m.IsSottotitoloActive =LinqExtensions.TryParseBoolDecimal(v.SOTTOTITOLO_ACTIVE);
            m.IsTitoloActive = LinqExtensions.TryParseBoolDecimal(v.TITOLO_ACTIVE);
            m.Mail = v.MAIL;
            m.RefIdReferral = (long)v.REF_ID_REFERRAL;
            m.RefOrg = (long)v.REF_ORG;
            m.Sottotitolo = v.SOTTOTITOLO;
            m.SottotitoloCode = v.SOTTOTITOLO_PROT_CODE;
            m.Telefono = v.TELEFONO;
            m.Titolo = v.TITOLO;
            m.TitoloCode = v.TITOLO_PROT_CODE;
            return m;
        }
        internal static RubricaEntita MapToRubrEntita(RUBR_ENTITA e)
        {
            RubricaEntita re = new RubricaEntita();
            re.IdReferral = (Nullable<Int64>)e.ID_REFERRAL;
            re.IdPadre = (Nullable<Int64>)e.ID_PADRE;
            re.ReferralType = (EntitaType)Enum.Parse(typeof(EntitaType), e.REFERRAL_TYPE);
            re.Cognome = e.COGNOME;
            re.Nome = e.NOME;
            re.CodiceFiscale = e.COD_FIS;
            re.PartitaIVA = e.P_IVA;
            re.RagioneSociale = e.RAGIONE_SOCIALE;
            re.Ufficio = e.UFFICIO;
            re.Note = e.NOTE;
            re.RefIdAddress = (Nullable<Int64>)e.REF_ID_ADDRESS;
            re.IsIPA = (e.FLG_IPA == null) ? false : Convert.ToBoolean(int.Parse(e.FLG_IPA));
            re.IPAdn = e.IPA_DN;
            re.IPAId = e.IPA_ID;
            re.DisambPre = e.DISAMB_PRE;
            re.DisambPost = e.DISAMB_POST;
            re.RefOrg = (Nullable<Int64>)e.REF_ORG;
            re.SitoWeb = e.SITO_WEB;
            re.AffIPA = (Nullable<Int16>)e.AFF_IPA;
            return re;
        }
        internal static ActiveUp.Net.Mail.DeltaExt.MailServer FromMailServersToModel(MAILSERVERS m)
    {
            MailServer ms = new MailServer();
            ms.Id = (decimal)m.ID_SVR;
            ms.DisplayName = m.NOME;
            ms.Dominus = m.DOMINUS;
            ms.IncomingProtocol = m.PROTOCOLLO_IN;
            ms.IncomingServer = m.INDIRIZZO_IN;
            ms.IsIncomeSecureConnection = bool.Parse(m.SSL_IN);
            ms.IsOutgoingSecureConnection = bool.Parse(m.SSL_OUT);
            ms.IsPec = Convert.ToBoolean(int.Parse(m.FLG_ISPEC));
            ms.OutgoingServer = m.INDIRIZZO_OUT;
            ms.IsOutgoingWithAuthentication = bool.Parse(m.AUTH_OUT);
            ms.PortIncomingChecked = !m.PORTA_IN.Equals(110);
            ms.PortIncomingServer = int.Parse(m.PORTA_IN);
            ms.PortOutgoingChecked = !m.PORTA_OUT.Equals(25);
            ms.PortOutgoingServer = int.Parse(m.PORTA_OUT);      
        return ms;
    }


         public static COMUNICAZIONI_DESTINATARI fromRubrContattiToComunicazioniDestinatari(V_RUBR_CONTATTI v_rubr_contatti)
        {
            COMUNICAZIONI_DESTINATARI destinatari = new COMUNICAZIONI_DESTINATARI
            {
                CAP = v_rubr_contatti.CAP,
                CIVICO = v_rubr_contatti.CIVICO,
                COD_FIS = v_rubr_contatti.COD_FIS,
                COD_ISO_STATO = v_rubr_contatti.COD_ISO_STATO,
                COGNOME = v_rubr_contatti.COGNOME,
                COMUNE = v_rubr_contatti.COMUNE,
                CONTACT_REF = v_rubr_contatti.CONTACT_REF,
                FAX = v_rubr_contatti.FAX,
                ID_REFERRAL = v_rubr_contatti.REF_ID_REFERRAL,
                INDIRIZZO = v_rubr_contatti.INDIRIZZO,
                MAIL = v_rubr_contatti.MAIL,
                NOME = v_rubr_contatti.NOME,
                P_IVA = v_rubr_contatti.P_IVA,
                RAGIONE_SOCIALE = v_rubr_contatti.RAGIONE_SOCIALE,
                REFERRAL_TYPE = v_rubr_contatti.REFERRAL_TYPE,
                SIGLA_PROV = v_rubr_contatti.SIGLA_PROV,
                TELEFONO = v_rubr_contatti.TELEFONO,
                UFFICIO = v_rubr_contatti.UFFICIO
            };
            return destinatari;
        }
        

        public static COMUNICAZIONI fromComunicazioniToDto(Comunicazioni entity, bool withattach, bool withcontent)
        {
            COMUNICAZIONI comunicazioni = new COMUNICAZIONI();
            // creo oggetto principale
            comunicazioni = new COMUNICAZIONI
            {
                COD_APP_INS = entity.AppCode,
                ORIG_UID = entity.OrigUID,
                IN_OUT = LinqExtensions.TryParseDecimalString(((int)entity.TipoCom).ToString()),
                FLG_NOTIFICA = LinqExtensions.TryParseNotityBool(entity.IsToNotify),
                MAIL_NOTIFICA = entity.MailNotifica,
                REF_ID_SOTTOTITOLO = LinqExtensions.TryParseInt(entity.RefIdSottotitolo),
                UNIQUE_ID_MAPPER = entity.UniqueId,
                UTE_INS = entity.UtenteInserimento
            };
            if (entity.IdComunicazione.HasValue)
            { comunicazioni.ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
            // creo entità collegate di allegati
            if (withattach)
            {
                if (entity.ComAllegati != null && entity.ComAllegati.Count > 0)
                {
                    foreach (ComAllegato comAllegati in entity.ComAllegati)
                    {
                        COMUNICAZIONI_ALLEGATI allegato = new COMUNICAZIONI_ALLEGATI
                        {
                            ALLEGATO_EXT = comAllegati.AllegatoExt,
                            ALLEGATO_FILE = comAllegati.AllegatoFile,
                            ALLEGATO_NAME = comAllegati.AllegatoName,
                            ALLEGATO_TPU = comAllegati.AllegatoTpu,
                            FLG_INS_PROT = ((int)comAllegati.FlgInsProt).ToString(),
                            FLG_PROT_TO_UPL = ((int)comAllegati.FlgProtToUpl).ToString()
                        };
                        if (entity.IdComunicazione.HasValue)
                        { allegato.REF_ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
                        if (comAllegati.IdAllegato.HasValue)
                        { allegato.ID_ALLEGATO = LinqExtensions.TryParseInt(comAllegati.IdAllegato); }
                        comunicazioni.COMUNICAZIONI_ALLEGATI.Add(allegato);
                    }
                }
            }
            // creo entità collegate di flusso
            if (entity.ComFlussi != null && entity.ComFlussi.Count > 0)
            {
                var list = entity.ComFlussi.Where(x => x.Key == TipoCanale.MAIL).SelectMany(z => z.Value);
                foreach (ComFlusso comFlusso in list)
                {
                    COMUNICAZIONI_FLUSSO flusso = new COMUNICAZIONI_FLUSSO
                    {
                        CANALE = comFlusso.Canale.ToString(),
                        DATA_OPERAZIONE = (comFlusso.DataOperazione == null ? DateTime.Now : Convert.ToDateTime(comFlusso.DataOperazione)),                               
                        STATO_COMUNICAZIONE_NEW =((int) comFlusso.StatoComunicazioneNew).ToString(),
                        STATO_COMUNICAZIONE_OLD =((int) comFlusso.StatoComunicazioneOld).ToString(),
                        UTE_OPE = comFlusso.UtenteOperazione
                    };
                    if (entity.IdComunicazione.HasValue)
                    { flusso.REF_ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
                    if (comFlusso.IdFlusso.HasValue)
                    { flusso.ID_FLUSSO = LinqExtensions.TryParseDouble(comFlusso.IdFlusso); }
                    comunicazioni.COMUNICAZIONI_FLUSSO.Add(flusso);
                }
            }
            // creo entità collegate di protocollo
            if (entity.ComFlussiProtocollo != null && entity.ComFlussiProtocollo.Count > 0)
            {
                foreach (ComFlussoProtocollo comFlussoProtocollo in entity.ComFlussiProtocollo)
                {
                    COMUNICAZIONI_FLUSSO_PROT flussoprotocollo = new COMUNICAZIONI_FLUSSO_PROT
                    {
                        DATA_OPERAZIONE = (DateTime)comFlussoProtocollo.DataOperazione,
                        STATO_NEW = LinqExtensions.TryParseByte(comFlussoProtocollo.StatoNew.ToString()),
                        STATO_OLD = LinqExtensions.TryParseByte(comFlussoProtocollo.StatoOld.ToString()),
                        UTE_OPE = comFlussoProtocollo.UtenteOperazione
                    };
                    if (entity.IdComunicazione.HasValue)
                    { flussoprotocollo.REF_ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
                    comunicazioni.COMUNICAZIONI_FLUSSO_PROT.Add(flussoprotocollo);
                }
            }
            if (withcontent)
            {
                MAIL_CONTENT content = new MAIL_CONTENT()
                {
                    FLG_ANNULLAMENTO = "0",
                    FOLDERID = entity.FolderId,
                    FOLDERTIPO = entity.FolderTipo,
                    FOLLOWS = (int?)entity.MailComunicazione.Follows,
                    MAIL_SENDER = entity.MailComunicazione.MailSender,
                    MAIL_SUBJECT = entity.MailComunicazione.MailSubject,
                    MAIL_TEXT = entity.MailComunicazione.MailText,
                    FLG_CUSTOM_REFS = LinqExtensions.TryParseString(entity.MailComunicazione.HasCustomRefs)
                };

                if (entity.IdComunicazione.HasValue)
                { content.REF_ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
                comunicazioni.MAIL_CONTENT.Add(content);
            }
            return comunicazioni;
        }

        public static COMUNICAZIONI fromComunicazioniToSimpleDto(Comunicazioni entity)
        {
            COMUNICAZIONI comunicazioni = new COMUNICAZIONI();
            // creo oggetto principale
            comunicazioni = new COMUNICAZIONI
            {
                COD_APP_INS = entity.AppCode,
                ORIG_UID = entity.OrigUID,
                IN_OUT = LinqExtensions.TryParseDecimalString(((int)entity.TipoCom).ToString()),
                FLG_NOTIFICA = LinqExtensions.TryParseNotityBool(entity.IsToNotify),
                MAIL_NOTIFICA = entity.MailNotifica,
                REF_ID_SOTTOTITOLO = LinqExtensions.TryParseInt(entity.RefIdSottotitolo),
                UNIQUE_ID_MAPPER = entity.UniqueId,
                UTE_INS = entity.UtenteInserimento
            };
            if (entity.IdComunicazione.HasValue)
            { comunicazioni.ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
            return comunicazioni;
        }

        internal static MAIL_CONTENT FromComunicazioniToMailContent(Comunicazioni entity)
        {
            MAIL_CONTENT content = new MAIL_CONTENT()
            {
                FLG_ANNULLAMENTO = "0",
                FOLDERID = entity.FolderId,
                FOLDERTIPO = entity.FolderTipo,
                FOLLOWS = (int?)entity.MailComunicazione.Follows,
                MAIL_SENDER = entity.MailComunicazione.MailSender,
                MAIL_SUBJECT = entity.MailComunicazione.MailSubject,
                MAIL_TEXT = entity.MailComunicazione.MailText,
                FLG_CUSTOM_REFS = LinqExtensions.TryParseString(entity.MailComunicazione.HasCustomRefs)
            };

            if (entity.IdComunicazione.HasValue)
            { content.REF_ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }           
            return content;
        }

        internal static COMUNICAZIONI_FLUSSO FromComFlussoToDto(ComFlusso comFlusso)
        {
            COMUNICAZIONI_FLUSSO flusso = new COMUNICAZIONI_FLUSSO
            {

                CANALE = comFlusso.Canale.ToString(),
                DATA_OPERAZIONE = (comFlusso.DataOperazione == null) ? System.DateTime.Now : (DateTime)comFlusso.DataOperazione,
                STATO_COMUNICAZIONE_NEW = ((int)comFlusso.StatoComunicazioneNew).ToString(),
                STATO_COMUNICAZIONE_OLD = ((int)comFlusso.StatoComunicazioneOld).ToString(),
                REF_ID_COM = (comFlusso.RefIdComunicazione == null) ? 0 : decimal.Parse(comFlusso.RefIdComunicazione.ToString()),
                UTE_OPE = comFlusso.UtenteOperazione
            };
            if (comFlusso.IdFlusso.HasValue)
            {
                flusso.ID_FLUSSO = LinqExtensions.TryParseDouble(comFlusso.IdFlusso);
            }
            if (comFlusso.RefIdComunicazione.HasValue)
            { flusso.REF_ID_COM = LinqExtensions.TryParseInt(comFlusso.RefIdComunicazione); }
            return flusso;
        }

        internal static COMUNICAZIONI_ALLEGATI FromComAllegatoToDto(ComAllegato comAllegati)
        {
            COMUNICAZIONI_ALLEGATI allegato = new COMUNICAZIONI_ALLEGATI
            {
                ALLEGATO_EXT = comAllegati.AllegatoExt,
                ALLEGATO_FILE = comAllegati.AllegatoFile,
                ALLEGATO_NAME = comAllegati.AllegatoName,
                ALLEGATO_TPU = comAllegati.AllegatoTpu,
                FLG_INS_PROT =(comAllegati.FlgInsProt == AllegatoProtocolloStatus.UNKNOWN) ? "0" : ((int)comAllegati.FlgInsProt).ToString(),
                FLG_PROT_TO_UPL = (comAllegati.FlgProtToUpl == AllegatoProtocolloStatus.UNKNOWN) ? "0" : ((int)comAllegati.FlgProtToUpl).ToString(),
            };
            if (comAllegati.RefIdCom.HasValue)
            { allegato.REF_ID_COM = LinqExtensions.TryParseInt(comAllegati.RefIdCom); }
            if (comAllegati.IdAllegato.HasValue)
            { allegato.ID_ALLEGATO = LinqExtensions.TryParseInt(comAllegati.IdAllegato); }
            return allegato;
        }

        internal static ComAllegato FromAllegatoToModel(COMUNICAZIONI_ALLEGATI allegato)
        {
            ComAllegato comAllegato = new ComAllegato()
            {
                AllegatoExt = allegato.ALLEGATO_EXT,
                AllegatoFile = allegato.ALLEGATO_FILE,
                AllegatoName = allegato.ALLEGATO_NAME,
                AllegatoTpu = allegato.ALLEGATO_TPU,
                FlgInsProt = LinqExtensions.TryParsEnumProt(allegato.FLG_INS_PROT),
                FlgProtToUpl = LinqExtensions.TryParsEnumProt(allegato.FLG_PROT_TO_UPL),
                IdAllegato = LinqExtensions.TryParseLong(allegato.ID_ALLEGATO),
                RefIdCom = LinqExtensions.TryParseLong(allegato.REF_ID_COM)
            };
            return comAllegato;
        }


        internal static Comunicazioni fromComunicazioniCompleteToDto(MAIL_CONTENT content)
        {
            Comunicazioni comunicazione = new Comunicazioni()
            {
                AppCode = content.COMUNICAZIONI.COMUNICAZIONI_SOTTOTITOLI.COMUNICAZIONI_TITOLI.APP_CODE,
                CodAppInserimento = content.COMUNICAZIONI.COD_APP_INS,
                ComCode = content.COMUNICAZIONI.COMUNICAZIONI_SOTTOTITOLI.COM_CODE,
                FolderId = (int) content.FOLDERID,
                FolderTipo = content.FOLDERTIPO,
                IdComunicazione = LinqExtensions.TryParseLong(content.REF_ID_COM),
                MailNotifica = content.COMUNICAZIONI.MAIL_NOTIFICA,
                OrigUID = content.COMUNICAZIONI.ORIG_UID,
                RefIdSottotitolo = LinqExtensions.TryParseLong(content.COMUNICAZIONI.REF_ID_SOTTOTITOLO),
                Sottotitolo = content.COMUNICAZIONI.COMUNICAZIONI_SOTTOTITOLI.SOTTOTITOLO,
                // tipocom
                UniqueId = content.COMUNICAZIONI.UNIQUE_ID_MAPPER,
                UtenteInserimento = content.COMUNICAZIONI.UTE_INS
            };
            MailContent ccon = new MailContent()
            {
                Follows =(content.FOLLOWS == null) ? null : (long?) (content.FOLLOWS),
                HasCustomRefs = (content.FLG_CUSTOM_REFS == "0") ? false : true,
                IdMail =(long) content.ID_MAIL,
                MailSender = content.MAIL_SENDER,
                MailSubject = content.MAIL_SUBJECT,
                MailText = content.MAIL_TEXT,

                RefIdComunicazione =(long) content.REF_ID_COM

            };
            if (content.MAIL_REFS_NEW != null && content.MAIL_REFS_NEW.Count > 0)
            {
                ccon.MailRefs = new List<Model.ComunicazioniMapping.MailRefs>();
                foreach (var r in content.MAIL_REFS_NEW)
                {
                    ccon.MailRefs.Add(FromMailRefsNewToObject(r));
                }
            }
            comunicazione.MailComunicazione = ccon;
            List<ComAllegato> listAllegati = new List<ComAllegato>();
            foreach (COMUNICAZIONI_ALLEGATI allegato in content.COMUNICAZIONI.COMUNICAZIONI_ALLEGATI)
            {
                ComAllegato com = AutoMapperConfiguration.FromAllegatoToModel(allegato);
                listAllegati.Add(com);
            }
            comunicazione.ComAllegati = listAllegati;
            List<ComFlussoProtocollo> listProtocolli = new List<ComFlussoProtocollo>();
            foreach (COMUNICAZIONI_FLUSSO_PROT protocollo in content.COMUNICAZIONI.COMUNICAZIONI_FLUSSO_PROT)
            {
                ComFlussoProtocollo prot = AutoMapperConfiguration.FromProtocolloToModel(protocollo);
                listProtocolli.Add(prot);
            }
            comunicazione.ComFlussiProtocollo = listProtocolli;
            if (content.COMUNICAZIONI.COMUNICAZIONI_FLUSSO != null)
            {
                Dictionary<TipoCanale, List<ComFlusso>> comFlussi = new Dictionary<TipoCanale, List<ComFlusso>>();
                List<ComFlusso> listFlussi = new List<ComFlusso>();
                foreach (COMUNICAZIONI_FLUSSO flusso in content.COMUNICAZIONI.COMUNICAZIONI_FLUSSO)
                {
                    ComFlusso comFlusso = AutoMapperConfiguration.fromFlussoToModel(flusso);
                    listFlussi.Add(comFlusso);
                }
                comFlussi.Add(TipoCanale.MAIL, listFlussi);
                comunicazione.ComFlussi = comFlussi;
            }
            List<RubrEntitaUsed> listRubr = new List<RubrEntitaUsed>();
            if (content.COMUNICAZIONI.COMUNICAZIONI_ENTITA_USED != null)
            {
                foreach (COMUNICAZIONI_ENTITA_USED coment in content.COMUNICAZIONI.COMUNICAZIONI_ENTITA_USED)
                {
                    COMUNICAZIONI_DESTINATARI dest = coment.COMUNICAZIONI_DESTINATARI;
                    RubrEntitaUsed rubrEntita = new RubrEntitaUsed()
                    {
                        Cap = dest.CAP,
                        Civico = dest.CIVICO,
                        CodiceFiscale = dest.COD_FIS,
                        CodISOStato = dest.COD_ISO_STATO,
                        Cognome = dest.COGNOME,
                        Comune = dest.COMUNE,
                        ContactRef = dest.CONTACT_REF,
                        Fax = dest.FAX,
                        IdEntUsed = LinqExtensions.TryParseLong(coment.REF_ID_ENT_USED),
                        IdReferral =LinqExtensions.TryParseLongNullable(dest.ID_REFERRAL),
                        Indirizzo = dest.INDIRIZZO,
                        Mail = dest.MAIL,
                        Nome = dest.NOME,
                        Note = dest.NOTE,
                        PartitaIVA = dest.P_IVA,
                        RagioneSociale = dest.RAGIONE_SOCIALE,
                        ReferralType = LinqExtensions.TryParseEntitaType(dest.REFERRAL_TYPE),
                        SiglaProvincia = dest.SIGLA_PROV,
                        Telefono = dest.TELEFONO,
                        Ufficio = dest.UFFICIO
                    };
                    listRubr.Add(rubrEntita);
                }
                comunicazione.RubricaEntitaUsed = listRubr;
            }
            return comunicazione;
        }

        internal static List<COMUNICAZIONI_ALLEGATI> fromComunicazioniToAllegati(Comunicazioni entity, decimal idcomnew)
        {
            List<COMUNICAZIONI_ALLEGATI> allegati = new List<COMUNICAZIONI_ALLEGATI>();
            if (entity.ComAllegati != null && entity.ComAllegati.Count > 0)
            {
                foreach (ComAllegato comAllegati in entity.ComAllegati)
                {
                    COMUNICAZIONI_ALLEGATI allegato = new COMUNICAZIONI_ALLEGATI
                    {
                        ALLEGATO_EXT = comAllegati.AllegatoExt,
                        ALLEGATO_FILE = comAllegati.AllegatoFile,
                        ALLEGATO_NAME = comAllegati.AllegatoName,
                        ALLEGATO_TPU = comAllegati.AllegatoTpu,
                        FLG_INS_PROT = ((int)comAllegati.FlgInsProt).ToString(),
                        FLG_PROT_TO_UPL = ((int)comAllegati.FlgProtToUpl).ToString()
                    };
                    if (entity.IdComunicazione.HasValue)
                    { allegato.REF_ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
                    if (comAllegati.IdAllegato.HasValue)
                    { allegato.ID_ALLEGATO = LinqExtensions.TryParseInt(comAllegati.IdAllegato); }
                    allegati.Add(allegato);
                }
            }
            return allegati;
        }

        internal static MAIL_INBOX_FLUSSO MapToMailInboxFlussoDto(decimal id, string oldStatus, string newStatus, DateTime? data, string ute)
        {

            MAIL_INBOX_FLUSSO inboxFlusso = new MAIL_INBOX_FLUSSO()
            {

                REF_ID_MAIL = id,
                STATUS_MAIL_OLD = (!String.IsNullOrEmpty(oldStatus)) ? oldStatus : null,
                STATUS_MAIL_NEW = newStatus,
                UTE_OPE = ute
            };
            return inboxFlusso;
        }

        internal static MAIL_INBOX MapToMailInBoxDto(MailUser us, ActiveUp.Net.Mail.Message m)
        {
            MAIL_INBOX inbox = new MAIL_INBOX()
            {
                DATA_RICEZIONE = m.Date.ToLocalTime(),
                MAIL_SERVER_ID = m.Uid.Replace(',', '§'),
                MAIL_ACCOUNT = us.EmailAddress,
                MAIL_FROM = m.From.ToString(),
                MAIL_TO = String.Join(";", m.To.Select(to => to.Email).ToArray()),
                MAIL_CC = String.Join(";", m.Cc.Select(cc => cc.Email).ToArray()),
                MAIL_CCN = String.Join(";", m.Bcc.Select(bcc => bcc.Email).ToArray()),
                MAIL_SUBJECT = m.Subject,
                MAIL_FOLDER = String.IsNullOrEmpty(m.HeaderFields["X-Ricevuta"]) ? "1" : "2",
                MAIL_TEXT = LinqExtensions.TryParseBody(m.BodyText),
                DATA_INVIO = m.ReceivedDate.ToLocalTime(),
                STATUS_SERVER = (String.IsNullOrEmpty(m.MessageId)) ? ((int)MailStatusServer.DA_NON_CANCELLARE).ToString() : ((int)MailStatusServer.PRESENTE).ToString(),
                STATUS_MAIL = (String.IsNullOrEmpty(m.MessageId)) ? ((int)MailStatus.SCARICATA_INCOMPLETA).ToString() : ((int)MailStatus.SCARICATA).ToString(),
                FLG_INTEG_APPL = "0",
                MAIL_FILE = System.Text.Encoding.GetEncoding("iso-8859-1").GetString(m.OriginalData),
                FLG_ATTACHMENTS = Convert.ToInt16(m.Attachments.Count > 0).ToString(),
                FOLDERID = String.IsNullOrEmpty(m.HeaderFields["X-Ricevuta"]) ? 1 : 3,
                FOLDERTIPO = "I",
                FOLLOWS = LinqExtensions.TryParseFollows(m.HeaderFields),
                MSG_LENGTH = m.OriginalData.Length               
            };
            return inbox;
        }

        internal static System.Tuples.Tuple<Message, string, int, string> MapToMailTuple(MAIL_INBOX inbox)
        {
            System.Tuples.Tuple<ActiveUp.Net.Mail.Message, string, int, string> tuple =
               new System.Tuples.Tuple<ActiveUp.Net.Mail.Message, string, int, string>();
            ActiveUp.Net.Mail.Message mex = new ActiveUp.Net.Mail.Message();
            if (inbox.ID_MAIL != 0)
            { mex.Id = (int)inbox.ID_MAIL; }
            mex.Uid = inbox.MAIL_SERVER_ID;
            int lobSize = inbox.MAIL_FILE.ToCharArray().Count();
            char[] file = new char[lobSize];
            file = inbox.MAIL_FILE.ToCharArray();
            mex.OriginalData = Encoding.UTF8.GetBytes(file);
            tuple.Element1 = mex;
            tuple.Element2 = inbox.FOLDERID.ToString();
            tuple.Element4 = inbox.FOLDERTIPO;
            tuple.Element3 = int.Parse(inbox.FLG_RATING.ToString()); // ??? booooooooooooo
            return tuple;
        }

        internal static Message MapToMessageModel(MAIL_INBOX inbox)
        {
            int lobSize = inbox.MAIL_FILE.ToCharArray().Count();
            char[] file = new char[lobSize];
            file = inbox.MAIL_FILE.ToCharArray();
            Message msg = new Message();
            // msg = Parser.ParseMessage(new string(file));
            msg.OriginalData = Encoding.UTF8.GetBytes(file);
            msg.Uid = inbox.MAIL_SERVER_ID;
            msg.Id = (int)inbox.ID_MAIL;
            msg.FolderId = (decimal)inbox.FOLDERID;
            msg.FolderTipo = inbox.FOLDERTIPO;
            msg.ParentFolder = "1";
            return msg;
        }

        internal static Message MapToMessageModelOut(MAIL_CONTENT content, List<COMUNICAZIONI_ALLEGATI> allegati)
        {

            Encoding encoding = Codec.GetEncoding("iso-8859-1");
            ActiveUp.Net.Mail.Message msg = new ActiveUp.Net.Mail.Message();
            msg.Charset = encoding.BodyName;
            msg.ContentTransferEncoding = ContentTransferEncoding.QuotedPrintable;
            msg.ContentType = new ContentType();
            string appo = null;
            foreach (MAIL_REFS_NEW refs in content.MAIL_REFS_NEW)
            {
                switch (refs.TIPO_REF)
                {
                    case "TO":
                        msg.To += Parser.ParseAddresses(refs.MAIL_DESTINATARIO);
                        break;
                    case "CC":
                        msg.Cc += Parser.ParseAddresses(refs.MAIL_DESTINATARIO);
                        break;
                    case "CCN":
                        msg.Bcc += Parser.ParseAddresses(refs.MAIL_DESTINATARIO);
                        break;

                }
            }
            if (!String.IsNullOrEmpty(appo = content.MAIL_TEXT))
            {
                msg.BodyHtml = new ActiveUp.Net.Mail.MimeBody(ActiveUp.Net.Mail.BodyFormat.Html);
                msg.BodyHtml.Text = appo;
                appo = null;
            }
            msg.From = Parser.ParseAddress(content.MAIL_SENDER);
            msg.Id = (int)content.ID_MAIL;
            msg.Subject = content.MAIL_SUBJECT;
            foreach (COMUNICAZIONI_ALLEGATI allegato in allegati)
            {
                ActiveUp.Net.Mail.MimePart mp = new ActiveUp.Net.Mail.MimePart();
                mp.ContentId = allegato.ID_ALLEGATO.ToString();
                mp.Filename = allegato.ALLEGATO_NAME;
                mp.ParentMessage = msg;
                msg.Attachments.Add(mp);

            }
            return msg;
        }


        internal static BackendUser MapToBackendUser(System.Data.Common.DbDataReader r)
        {
            BackendUser bUser = new BackendUser();
            bUser.UserId =(long) r.GetDecimal("ID_USER");
            bUser.UserName = r.GetString("USER_NAME").ToString();           
            bUser.Department =(long) r.GetDecimal("DEPARTMENT");
            if (r.GetString("MUNICIPIO") != null)
            {
                bUser.Municipio = r.GetString("MUNICIPIO").ToString();
            }
            else
            {
                bUser.Municipio = string.Empty;
            }

            bUser.Domain = r.GetString("DOMAIN");
            bUser.Cognome = r.GetString("COGNOME");
            bUser.Nome = r.GetString("NOME");
            bUser.UserRole = int.Parse(r.GetString("ROLE_USER"));
            if (!string.IsNullOrEmpty(r.GetString("ROLE_MAIL")))
            {
                bUser.RoleMail = int.Parse(r.GetString("ROLE_MAIL"));
            }

            return bUser;
        }

        internal static Titolo MapToTitolo(COMUNICAZIONI_TITOLI t)
        {
            Titolo a = new Titolo();
            a.AppCode = t.APP_CODE;
            a.CodiceProtocollo = t.PROT_CODE;
            a.Id = t.ID_TITOLO;
            a.Nome = t.TITOLO;
            a.Note = t.NOTE;          
            if (t.ACTIVE == 0)
                a.Deleted = true;
            else a.Deleted = false;
            return a;
        }

        internal static RubricaContatti MapToRubrContatti(RUBR_CONTATTI r, RUBR_ENTITA e, COMUNICAZIONI_TITOLI t, List<decimal> lt)
        {
            RubricaContatti c = new RubricaContatti();
            if (r.AFF_IPA != null)
            { c.AffIPA = (short)r.AFF_IPA; }
            if (r.CONTACT_REF != null)
            { c.ContactRef = r.CONTACT_REF; }
            c.Fax = r.FAX;
            c.IdContact = (Nullable<long>) r.ID_CONTACT;
            c.IPAdn = r.IPA_DN;
            c.IPAId = r.IPA_ID;
            c.IsIPA = (r.FLG_IPA == "0") ? false : true;
            c.IsPec = (r.FLG_PEC == 0) ? false : true;
            c.Mail = r.MAIL;
            c.Note = r.NOTE;
            c.RefIdReferral =(long) r.REF_ID_REFERRAL;
            c.Src = "R";
            c.Source = null;
            c.Telefono = r.TELEFONO;
            c.T_Ufficio = e.UFFICIO;
            c.T_RagioneSociale =e.RAGIONE_SOCIALE;
            c.T_MappedAppName = t.TITOLO;
            c.T_MappedAppID =(long) t.ID_TITOLO;
            c.MappedAppsId = new List<long>();
            foreach (int i in lt)
            {
                if (!(c.MappedAppsId.Contains(i)))
                { c.MappedAppsId.Add(i); }
            }

            return c;
            // c.T_isMappedAppDefault

            //c.TipoContatto

        }

        internal static ContactsBackendMap MapToContactsBackendMap(RUBR_CONTATTI_BACKEND dr,RUBR_CONTATTI l, RUBR_ENTITA e, List<decimal> t)
        {
            ContactsBackendMap cbm = new ContactsBackendMap();
            cbm.Contatto = AutoMapperConfiguration.MapToRubrContatti(l, e, dr.COMUNICAZIONI_TITOLI, t);
            cbm.Titolo = AutoMapperConfiguration.MapToTitolo(dr.COMUNICAZIONI_TITOLI);

            if (dr.ID_MAP != null)
            { cbm.Id = (int)dr.ID_MAP; }
            else { cbm.Id = -1; }
            cbm.Canale = TipoCanale.UNKNOWN;
            if (dr.REF_ID_CANALE != null)
            { cbm.Canale = (TipoCanale)(int)dr.REF_ID_CANALE; }
            if (dr.REF_ID_BACKEND != null)
            { cbm.Backend = new BackEndRefCode { Id = (decimal)dr.REF_ID_BACKEND }; }
            cbm.Contatto = new RubricaContatti();
            if (dr.REF_ID_CONTATTO != null)
                cbm.Contatto.IdContact = (long)dr.REF_ID_CONTATTO;
            else
                cbm.Contatto.IdContact = -1;
            if (dr.REF_ID_TITOLO != null)
                cbm.Titolo = new Titolo { Id = (decimal)dr.REF_ID_TITOLO };
            return cbm;
        }

        internal static UserResultItem MapToUserResult(IDataRecord dr)
        {
            UserResultItem user = new UserResultItem();
            user.Account = dr.GetString("ACCOUNT");
            user.User = dr.GetString("UTE");
            user.Operazioni = dr.GetDecimal("TOT").ToString();
            return user;
        }

        internal static BackendUser MapToDepartment(IDataRecord dr)
        {
            BackendUser bUser = new BackendUser();
            bUser.Department = dr.GetInt64("DEPARTMENT");
            return bUser;
        }


        internal static BackendUser MapToDepartmentModel(MAIL_USERS_BACKEND m)
        {
            BackendUser bUser = new BackendUser();
            bUser.Department =(long) m.DEPARTMENT;
            return bUser;
        }

        internal static BackendUser FromMailUsersBackendToModel(MAIL_USERS_BACKEND d)
        {
            BackendUser bUser = new BackendUser();
            bUser.UserId =(long) d.ID_USER;
            bUser.UserName = d.USER_NAME;
            bUser.Department =(long) d.DEPARTMENT;
            bUser.Municipio = string.IsNullOrEmpty(d.MUNICIPIO) ? string.Empty : d.MUNICIPIO;
            bUser.Domain = d.DOMAIN;
            bUser.Cognome = d.COGNOME;
            bUser.Nome = d.NOME;
            bUser.UserRole = string.IsNullOrEmpty(d.ROLE) ? 0 : int.Parse(d.ROLE);
            bUser.RoleMail = 0;
            return bUser;
        }



        internal static RUBR_BACKEND FromModelToRubrBackendDto(BackEndRefCode entity, RUBR_BACKEND b)
        {
            b.BACKEND_CODE = entity.Codice;
            b.BACKEND_DESCR = entity.Descrizione;
            b.CATEGORY = entity.Categoria;
            b.DESCR_PLUS = entity.DescrizionePlus;
            return b;
        }

        private static ComFlusso fromFlussoToModel(COMUNICAZIONI_FLUSSO flusso)
        {
            ComFlusso comFlusso = new ComFlusso()
            {
                Canale = TipoCanale.MAIL,
                DataOperazione = flusso.DATA_OPERAZIONE,
                IdFlusso = LinqExtensions.TryParseLong(flusso.ID_FLUSSO),
                RefIdComunicazione =LinqExtensions.TryParseLong(flusso.REF_ID_COM),
                StatoComunicazioneNew = LinqExtensions.TryParsEnumStatus(flusso.STATO_COMUNICAZIONE_NEW),
                StatoComunicazioneOld = LinqExtensions.TryParsEnumStatus(flusso.STATO_COMUNICAZIONE_OLD),
                UtenteOperazione = flusso.UTE_OPE
            };
            return comFlusso;
        }

        internal static BackEndRefCode FromRubrBackendToSingleModel(RUBR_BACKEND b)
        {
            BackEndRefCode st = new BackEndRefCode();
            st.Id = b.ID_BACKEND;
            st.Codice = b.BACKEND_CODE;
            st.Descrizione = b.BACKEND_DESCR;
            st.Categoria = b.CATEGORY;
            return st;
        }

        internal static RUBR_BACKEND FromRubrBackendToDto(BackEndRefCode entity)
        {
            RUBR_BACKEND r = new RUBR_BACKEND()
            {
                BACKEND_CODE = entity.Codice,
                BACKEND_DESCR = entity.Descrizione,
                CATEGORY = entity.Categoria.ToString(),
                DESCR_PLUS = entity.DescrizionePlus,
            };
            return r;
        }

        internal static List<BackEndRefCode> FromRubrBackendToModel(List<RUBR_BACKEND> allEntity)
        {
            List<BackEndRefCode> l = new List<BackEndRefCode>();
            foreach (RUBR_BACKEND b in allEntity)
            {
                BackEndRefCode st = new BackEndRefCode();
                st.Id = b.ID_BACKEND;
                st.Codice = b.BACKEND_CODE;
                st.Descrizione = b.BACKEND_DESCR;
                st.Categoria = b.CATEGORY;
                l.Add(st);
            }
            return l;
        }

        internal static ComFlussoProtocollo FromProtocolloToModel(COMUNICAZIONI_FLUSSO_PROT protocollo)
        {
            ComFlussoProtocollo comFlussoProt = new ComFlussoProtocollo()
            {
                DataOperazione = protocollo.DATA_OPERAZIONE,
                RefIdComunicazione = LinqExtensions.TryParseLong(protocollo.REF_ID_COM),
                StatoNew = LinqExtensions.TryParsEnumFlusso(protocollo.STATO_NEW.ToString()),
                StatoOld = LinqExtensions.TryParsEnumFlusso(protocollo.STATO_OLD.ToString()),
                UtenteOperazione = protocollo.UTE_OPE
            };
            return comFlussoProt;
        }

        internal static Model.MailRefs FromMailRefsNewToModel(MAIL_REFS_NEW r)
        {
            SendMail.Model.MailRefs refs = new Model.MailRefs()
            {
                IdRef = LinqExtensions.TryParseLong(r.ID_REF),
                RefIdMail = LinqExtensions.TryParseLong(r.REF_ID_MAIL),
                AddresseeMail = r.MAIL_DESTINATARIO,
                AddresseeClass = LinqExtensions.TryParsEnumAddressee(r.TIPO_REF)
            };
            return refs;
        }


        internal static Model.ComunicazioniMapping.MailRefs FromMailRefsNewToObject(MAIL_REFS_NEW r)
        {
            Model.ComunicazioniMapping.MailRefs refs = new Model.ComunicazioniMapping.MailRefs()
            {
                MailDestinatario = r.MAIL_DESTINATARIO,
                TipoRef = LinqExtensions.TryParsEnumAddressee(r.TIPO_REF)
            };
            return refs;
        }

        internal static MAIL_REFS_NEW FromMailRefsNewToDto(Model.MailRefs entity)
        {
            MAIL_REFS_NEW refs = new MAIL_REFS_NEW()
            {
                REF_ID_MAIL = int.Parse(entity.RefIdMail.ToString()),
                MAIL_DESTINATARIO = entity.AddresseeMail,
                TIPO_REF = entity.AddresseeClass.ToString()
            };
            return refs;
        }

    }
}

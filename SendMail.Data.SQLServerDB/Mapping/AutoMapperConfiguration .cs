using AutoMapper;
using SendMail.Model;
using SendMail.Model.ComunicazioniMapping;
using System;
using System.Collections.Generic;
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
            Mapper.Initialize(cfg => cfg.CreateMap<SendMail.Model.ComunicazioniMapping.MailRefs, MAIL_REFS>().ForMember(d => d.MAIL_DESTINATARIO, opt => opt.MapFrom(c => c.MailDestinatario))
               .ForMember(d => d.TIPO_REF, opt => opt.MapFrom(c => c.TipoRef)));
            Mapper.Initialize(cfg => cfg.CreateMap<SendMail.Model.MailRefs, MAIL_REFS>().ForMember(d => d.REF_ID_MAIL, opt => opt.MapFrom(c => c.RefIdMail))
           .ForMember(d => d.ID_REF, opt => opt.MapFrom(c => c.IdRef)));
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
            Mapper.Initialize(cfg => cfg.CreateMap<SendMail.Model.ComunicazioniMapping.MailRefs, MAIL_REFS>().ForMember(d => d.MAIL_DESTINATARIO, opt => opt.MapFrom(c => c.MailDestinatario))
               .ForMember(d => d.TIPO_REF, opt => opt.MapFrom(c => c.TipoRef)));
            Mapper.Initialize(cfg => cfg.CreateMap<SendMail.Model.MailRefs, MAIL_REFS>().ForMember(d => d.REF_ID_MAIL, opt => opt.MapFrom(c => c.RefIdMail))
           .ForMember(d => d.ID_REF, opt => opt.MapFrom(c => c.IdRef)));
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

        public static COMUNICAZIONI fromComunicazioniToDto(Comunicazioni entity)
        {
            COMUNICAZIONI comunicazioni = new COMUNICAZIONI();
            // creo oggetto principale
            comunicazioni = new COMUNICAZIONI
            {
                COD_APP_INS = entity.AppCode,
                ORIG_UID = entity.OrigUID,
                IN_OUT = LinqExtensions.TryParseDecimalString(entity.TipoCom.ToString()),
                FLG_NOTIFICA = entity.IsToNotify.ToString(),
                MAIL_NOTIFICA = entity.MailNotifica,
                REF_ID_SOTTOTITOLO = LinqExtensions.TryParseInt(entity.RefIdSottotitolo),
                UNIQUE_ID_MAPPER = entity.UniqueId,
                UTE_INS = entity.UtenteInserimento
            };
            if (entity.IdComunicazione.HasValue)
            { comunicazioni.ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
            // creo entità collegate di allegati
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
                        FLG_INS_PROT = comAllegati.FlgInsProt.ToString(),
                        FLG_PROT_TO_UPL = comAllegati.FlgProtToUpl.ToString()
                    };
                    if (entity.IdComunicazione.HasValue)
                    { allegato.REF_ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
                    if (comAllegati.IdAllegato.HasValue)
                    { allegato.ID_ALLEGATO = LinqExtensions.TryParseInt(comAllegati.IdAllegato); }
                    comunicazioni.COMUNICAZIONI_ALLEGATI.Add(allegato);
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
                        DATA_OPERAZIONE = (DateTime)comFlusso.DataOperazione,
                        STATO_COMUNICAZIONE_NEW = comFlusso.StatoComunicazioneNew.ToString(),
                        STATO_COMUNICAZIONE_OLD = comFlusso.StatoComunicazioneOld.ToString(),
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
            return comunicazioni;
        }

        internal static COMUNICAZIONI_FLUSSO FromComFlussoToDto(ComFlusso comFlusso)
        {
            COMUNICAZIONI_FLUSSO flusso = new COMUNICAZIONI_FLUSSO
            {

                CANALE = comFlusso.Canale.ToString(),
                DATA_OPERAZIONE = (DateTime)comFlusso.DataOperazione,
                STATO_COMUNICAZIONE_NEW = comFlusso.StatoComunicazioneNew.ToString(),
                STATO_COMUNICAZIONE_OLD = comFlusso.StatoComunicazioneOld.ToString(),
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
                FLG_INS_PROT = comAllegati.FlgInsProt.ToString(),
                FLG_PROT_TO_UPL = comAllegati.FlgProtToUpl.ToString()
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
                FolderId = LinqExtensions.TryParseIntFromDouble(content.FOLDERID),
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

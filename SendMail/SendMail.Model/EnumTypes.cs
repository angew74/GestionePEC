using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SendMail.Model
{


    /// <summary>
    /// Enumerativo dei possibili tipi di canali.
    /// </summary>
    public enum TipoCanale
    {
        [Description("NON DEFINITO")]
        UNKNOWN = 0,
        FAX = 2,
        MAIL = 1,
        POSTA = 3,
        [Description("A MANO")]
        A_MANO = 4,
        POSTA_INTERNA = 5
    }

    public enum ProtocolloStatus
    {
        [Description("NON DEFINITO")]
        UNKNOWN = -1,

        DA_PROTOCOLLARE = 0,

        ERRORE_PROTOCOLLAZIONE = 1,

        PROTOCOLLATA = 2,

        ALLEGARE_DOCUMENTI = 3,

        ERRORE_DOCUMENTI_ALLEGATI = 4,

        DOCUMENTI_ALLEGATI = 5
    }

    public enum ProtocolloTypes
    {
        UNKNOWN = 0,
        I,
        M,
        U
    }

    public enum AllegatoProtocolloStatus
    {
        [Description("NON DEFINITO")]
        UNKNOWN = -1,

        FALSE = 0,

        TRUE = 1,

        DONE = 2
    }

    public enum TipoComunicazione
    {
        [Description("NON DEFINITO")]
        UNKNOWN = -1,
        IN = 0,
        OUT = 1
    }
    
    [Serializable]       
    public enum EntitaType
    {
        [Description("NON DEFINITO")]
        UNKNOWN = 0,
        [Description("Tutti i tipi")]
        ALL,
        [Description("Raggruppamento Omogeneo")]
        GRP,
        [Description("Persona Fisica")]
        PF,
        [Description("Persona giuridica")]
        PG,
        [Description("Ente Pubblico")]
        PA,
        [Description("Ente Pubblico di Ente Publico")]
        PA_SUB,
        [Description("Gruppo di Uffici di Ente pubblico")]
        PA_GRP,
        [Description("Azienda concessionaria di pubblico servizio")]
        AZ_PS,
        [Description("Azienda privata")]
        AZ_PRI,
        [Description("GRuppo di Uffici di Azienda privata")]
        AZ_GRP,
        [Description("Ufficio di Ente Pubblico")]
        PA_UFF,
        [Description("Persona fisica di Ente Pubblico")]
        PA_PF,
        [Description("Persona fisica di Ufficio di Ente Pubblico")]
        PA_UFF_PF,
        [Description("Ufficio di azienda")]
        AZ_UFF,
        [Description("Persona fisica di azienda")]
        AZ_PF,
        [Description("Persona fisica di ufficio di azienda")]
        AZ_UFF_PF
    }

    public enum FastIndexedAttributes
    {
        [DescriptionAttribute("Mail")]
        MAIL,
        [DescriptionAttribute("Fax")]
        FAX,
        [DescriptionAttribute("Telefono")]
        TELEFONO,
        [DescriptionAttribute("Ragione Sociale")]
        RAGIONE_SOCIALE,
        [DescriptionAttribute("Ufficio")]
        UFFICIO,
        [DescriptionAttribute("Cognome e Nome")]
        COGNOME
    }

    public enum MailIndexedSearch
    {
        [Description("Non definito")]
        UNKNOWN = 0,
        MAIL,
        SUBJECT,
        STATUS_MAIL
    }

    public enum MailTypeSearch
    {
        TipoBox,
        DataInzio,
        DataFine,
        Utente,
        Titolo,
        SottoTitolo,
        Mail,
        Oggetto,
        Status,
        Marcatori,
        StatusInbox
    }

    public enum IndexedCatalogs
    {
        ALL,
        IPA,
        RUBR
    }

    public enum MailPecForCheck
    {
        pec,
        cert,
        legalmail,
        legalpec,
        postacert
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace ActiveUp.Net.Common.DeltaExt
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public enum AddresseeType : short
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("NON DEFINITO")]
        UNDEFINED,
        /// <summary>
        /// 
        /// </summary>
        TO,
        /// <summary>
        /// 
        /// </summary>
        CC,
        /// <summary>
        /// 
        /// </summary>
        CCN
    }
    /// <summary>
    /// 
    /// </summary>
    public enum MailStatus
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("IGNOTO")]
        UNKNOWN = -1,
        // mail receive
        /// <summary>
        /// 
        /// </summary>
        SCARICATA = 0,
        /// <summary>
        /// 
        /// </summary>
        LETTA = 1,
        /// <summary>
        /// 
        /// </summary>
        INOLTRATA = 2,
        /// <summary>
        /// 
        /// </summary>
        REPLY_ONE = 3,
        /// <summary>
        /// 
        /// </summary>
        REPLY_ALL = 4, 
        /// <summary>
        /// 
        /// </summary>
        CANCELLATA = 5,

        /// <summary>
        /// 
        /// </summary>
        SCARICATA_INCOMPLETA = 6,
        //INVIATA = 6,
        //mail send
        /// <summary>
        /// 
        /// </summary>
        INSERTED = 20,
        /// <summary>
        /// 
        /// </summary>
        PROCESSING = 21,
        /// <summary>
        /// 
        /// </summary>
        SEND_AGAIN = 22,
        /// <summary>
        /// 
        /// </summary>
        [Description("INVIATA AL SERVER")]
        SENT = 23,
        /// <summary>
        /// 
        /// </summary>
        ERROR = 24,
        /// <summary>
        /// 
        /// </summary>
        CANCELLED = 25,
        /// <summary>
        /// 
        /// </summary>
        [Description("ACCETTATA DAL SERVER PEC")]
        ACCETTAZIONE = 26,
        /// <summary>
        /// 
        /// </summary>
        [Description("NON ACCETTATA DAL SERVER PEC")]
        NON_ACCETTAZIONE = 27,
        /// <summary>
        /// 
        /// </summary>
        [Description("AVVENUTA CONSEGNA AL SERVER PEC DI DESTINAZIONE")]
        AVVENUTA_CONSEGNA = 28,
        /// <summary>
        /// 
        /// </summary>
        [Description("ERRORE CONSEGNA AL SERVER PEC DI DESTINAZIONE")]
        ERRORE_CONSEGNA = 29,
        /// <summary>
        /// 
        /// </summary>
        [Description("COMUNICAZIONE DA INVIARE AL PROTOCOLLO")]
        IN_ATTESA_PROTOCOLLO = 30,
        /// <summary>
        /// 
        /// </summary>
        [Description("COMUNINCAZIONE PROTOCOLLATA")]
        PROTOCOLLATA = 31,
        /// <summary>
        /// 
        /// </summary>
        [Description("ERRORE PROTOCOLLO")]
        ERRORE_PROTOCOLLO = 32,
        /// <summary>
        /// 
        /// </summary>
        ARCHIVIATA = 99
    }
    /// <summary>
    /// 
    /// </summary>
    public enum MailStatusServer
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("IGNOTO")]
        UNKNOWN = -1,
        /// <summary>
        /// 
        /// </summary>
        PRESENTE,
        /// <summary>
        /// 
        /// </summary>
        CANCELLATA,
        /// <summary>
        /// 
        /// </summary>
        DA_NON_CANCELLARE
    }
    /// <summary>
    /// 
    /// </summary>
    public enum MailFolder
    {
        /// <summary>
        /// 
        /// </summary>
        Tutte = 0,
        /// <summary>
        /// 
        /// </summary>
        INBOX = 1,
        /// <summary>
        /// 
        /// </summary>
        OUTBOX = 2,
        /// <summary>
        /// 
        /// </summary>
        RICEVUTE_PEC = 3,
         
        INBOX_C = 4,
         
        INBOX_A = 5,
       
        OUTBOX_C= 6,

        OUTBOX_A =7,

        RICEVUTE_PEC_C =8,

        RICEVUTE_PEC_A=9,      

        TUTTE = 99
       
    }

    public enum MailTipo
    {
        [StringValue("I")] 
        INBOX,
        [StringValue("O")] 
        OUTBOX,
        [StringValue("C")] 
        CESTINO,
        [StringValue("A")] 
        ARCHIVIO 
    }
    /// <summary>
    /// 
    /// </summary>
    public enum MailActions
    {
        /// <summary>
        /// 
        /// </summary>
        NONE,
        /// <summary>
        /// 
        /// </summary>
        SEND,
        /// <summary>
        /// 
        /// </summary>
        REPLY_TO,
        /// <summary>
        /// 
        /// </summary>
        FORWARD,
        /// <summary>
        /// 
        /// </summary>
        REPLY_ALL,
        /// <summary>
        /// 
        /// </summary>
        ACQUIRE,
        /// <summary>
        /// 
        /// </summary>
        RE_SEND
    }

    //public enum TipoCanale
    //{
    //    [Description("NON DEFINITO")]
    //    UNKNOWN = 0,
    //    FAX = 1,
    //    MAIL = 2,
    //    POSTA = 3,
    //    [Description("A MANO")]
    //    AMANO = 4,
    //    POSTA_INTERNA = 5
    //}

}

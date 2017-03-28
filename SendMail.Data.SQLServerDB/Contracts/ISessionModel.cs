using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SendMail.Model;
using SendMailApp.OracleCore.Contracts;
using SendMail.Data.Contracts.Mail;
using SendMail.Data.Contracts;

namespace SendMail.Data.SQLServerDB
{
    public interface ISessionModel
    {
        //IAllegatoDao AllegatoDao { get; }
        IMailDao MailDao { get; }
        IMailAddresseDao MailAddresseeDao { get; }
        IBackendUserDao BackendUserDao { get; }
        IMailRefsDao MailRefsDao { get; }
        IRubricaDao RubricaDao { get; }
        ITitoloDao TitoloDao { get; }
        ISottoTitoloDao SottoTitoloDao { get; }
        //INominativoRubricaDao NominativoRubricaDao { get; }
        IBackEndCodeDao BackEndCodeDao { get; }
        IContactsApplicationMappingDao ContactsApplicationMappingDao { get; }
        IContattoDao ContattoDao { get; }
        IRubricaEntitaDao RubricaEntitaDao { get; }
        IComunicazioniDao ComunicazioniDao { get; }
        IComFlussoDao ComFlussoDao { get; }
        IComAllegatoDao ComAllegatoDao { get; }
        IMailServerDao MailServerDao { get; }
        IMailAccountDao MailAccountDao { get; }
        IMailMessageDao MailMessageDao { get; }
        IMailHeaderDao MailHeaderDao { get; }
        IReferralTypeDao ReferralTypeDao { get; }
        IContactsBackendDao ContactsBackendDao { get; }
        ISendersFoldersDao SendersFoldersDao { get; }
    }
}

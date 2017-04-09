using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.BusinessEF.Contracts;
using SendMail.Model;
using SendMail.Business;
using ActiveUp.Net.Mail.DeltaExt;

namespace SendMail.Contracts
{
    /// <summary>
    /// Intefaccia per il recupero degli oggetti Service.
    /// </summary>
    public interface IServiceFactory
    {
        IMailService MailService { get; }
        //IAllegatoService AllegatoService { get; }
        IRubricaService RubricaService { get; }
        IMailRefs MailRefsService{get ;}
        //SendMail.BusinessEF.Contracts.PrintDirectorTpu.ITpuService TpuService { get; }
        ITitolarioService<T> TitolarioService<T>() where T : Titolo;
        IBackEndDictionaryService BackEndDictionaryService { get; }
        //INominativoRubricaService NominativoRubricaService { get; }
        IContactsMappingService ContactsMappingService { get; }
        IContattoService ContattoService { get; }
        IRubricaEntitaService RubricaEntitaService { get; }
        IComunicazioniService ComunicazioniService { get; }
        Com.Delta.Mail.Facades.IMailServerConfigFacade getMailServerConfigFacade();
        Com.Delta.Mail.Facades.IMailServerFacade getMailServerFacade(MailUser user);
        //IMailServerService MailServerService { get; }//????
        IMailAccountService MailAccountService { get; }//????
        IBackendUserService BackendUserService { get; }//????
        SendMail.BusinessEF.MailFacedes.MailLocalService MailLocalService { get; }
        IContactsBackendService ContactsBackendService { get; }
        ISendersFoldersService SendersFoldersService { get; }//Ciro Cardone - 13/02/2014

    }
}

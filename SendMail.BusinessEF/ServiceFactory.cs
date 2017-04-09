using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SendMail.Contracts;
using SendMail.BusinessEF.Contracts;
using SendMail.Model;
using SendMail.BusinessEF.MailFacedes;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.BusinessEF.Contracts;

namespace SendMail.BusinessEFEF
{
    /// <summary>
    /// Implementazione concreta dell'interfaccia IServiceFactory.
    /// </summary>
    public class ServiceFactory : IServiceFactory
    {
        public IMailService MailService
        {
            get { return BusinessEF.MailService.Instance; }
        }

        public IRubricaService RubricaService
        {
            get { return BusinessEF.RubricaService.Instance; }
        }

        public IMailAccountService MailAccountService
        {
            get { return SendMail.BusinessEF.MailFacedes.MailAccountService.Instance; }
        }

        public IBackendUserService BackendUserService
        {
            get { return BusinessEF.BackendUserService.Instance; }
        }

        public IMailRefs MailRefsService
        {
            get { return SendMail.BusinessEF.MailRefsService.Instance; }
        }

        public ITitolarioService<T> TitolarioService<T>() where T : Titolo
        {
            return SendMail.BusinessEF.TitolarioService<T>.Instance;
        }

        //public INominativoRubricaService NominativoRubricaService
        //{
        //    //get { return SendMail.BusinessEF.NominativoRubricaService.Instance; }
        //    get { return null; }
        //}

        public IBackEndDictionaryService BackEndDictionaryService
        {
            get { return SendMail.BusinessEF.BackEndDictionaryService.Instance; }
        }

        public IContactsMappingService ContactsMappingService
        {
            get { return SendMail.BusinessEF.ContactsMappingService.Instance; }
        }

        public IContattoService ContattoService
        {
            get { return SendMail.BusinessEF.ContattoService.Instance; }
        }

        public IRubricaEntitaService RubricaEntitaService
        {
            get { return SendMail.BusinessEF.RubricaEntitaService.Instance; }
        }

        public IComunicazioniService ComunicazioniService
        {
            get { return SendMail.BusinessEF.ComunicazioniService.Instance; }
        }

        //public SendMail.BusinessEF.Contracts.PrintDirectorTpu.ITpuService TpuService
        //{
        //    get { return SendMail.BusinessEF.PrintDirectorTpu.TpuService.Instance; }
        //}

        public Com.Delta.Mail.Facades.IMailServerFacade getMailServerFacade(MailUser user)
        {
            return MailServerFacade.GetInstance(user);
        }

        public Com.Delta.Mail.Facades.IMailServerConfigFacade getMailServerConfigFacade()
        {
            return MailServerConfigFacade.GetInstance();
        }

        public MailLocalService MailLocalService
        {
            get { return SendMail.BusinessEF.MailFacedes.MailLocalService.Instance; }
        }

       
        public ISendersFoldersService SendersFoldersService
        {
            get { return SendMail.BusinessEF.SendersFoldersService.Instance; }
        }

        #region IServiceFactory Membri di


        public IContactsBackendService ContactsBackendService
        {
            get { return SendMail.BusinessEF.ContactsBackendService.Instance; }
        }

        #endregion
    }
}
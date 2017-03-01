using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SendMail.Contracts;
using SendMail.Business.Contracts;
using SendMail.Model;
using SendMail.Business.MailFacades;
using ActiveUp.Net.Mail.DeltaExt;


namespace SendMail.Business
{
    /// <summary>
    /// Implementazione concreta dell'interfaccia IServiceFactory.
    /// </summary>
    public class ServiceFactory : SendMail.Contracts.IServiceFactory
    {
        public IMailService MailService
        {
            get { return SendMail.Business.MailService.Instance; }
        }

        public IRubricaService RubricaService
        {
            get { return SendMail.Business.RubricaService.Instance; }
        }

        public IMailAccountService MailAccountService
        {
            get { return SendMail.Business.MailFacades.MailAccountService.Instance; }
        }

        public IBackendUserService BackendUserService
        {
            get { return SendMail.Business.BackendUserService.Instance; }
        }

        public IMailRefs MailRefsService
        {
            get { return SendMail.Business.MailRefsService.Instance; }
        }

        public ITitolarioService<T> TitolarioService<T>() where T : Titolo
        {
            return SendMail.Business.TitolarioService<T>.Instance;
        }

        //public INominativoRubricaService NominativoRubricaService
        //{
        //    //get { return SendMail.Business.NominativoRubricaService.Instance; }
        //    get { return null; }
        //}

        public IBackEndDictionaryService BackEndDictionaryService
        {
            get { return SendMail.Business.BackEndDictionaryService.Instance; }
        }

        public IContactsMappingService ContactsMappingService
        {
            get { return SendMail.Business.ContactsMappingService.Instance; }
        }

        public IContattoService ContattoService
        {
            get { return SendMail.Business.ContattoService.Instance; }
        }

        public IRubricaEntitaService RubricaEntitaService
        {
            get { return SendMail.Business.RubricaEntitaService.Instance; }
        }

        public IComunicazioniService ComunicazioniService
        {
            get { return SendMail.Business.ComunicazioniService.Instance; }
        }

        //public SendMail.Business.Contracts.PrintDirectorTpu.ITpuService TpuService
        //{
        //    get { return SendMail.Business.PrintDirectorTpu.TpuService.Instance; }
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
            get { return SendMail.Business.MailFacades.MailLocalService.Instance; }
        }

        /// <summary>
        /// Ciro Cardone - 13/02/2014
        /// </summary>
        public ISendersFoldersService SendersFoldersService
        {
            get { return SendMail.Business.SendersFoldersService.Instance; }
        }

        #region IServiceFactory Membri di


        public IContactsBackendService ContactsBackendService
        {
            get { return SendMail.Business.ContactsBackendService.Instance; }
        }

        #endregion
    }
}
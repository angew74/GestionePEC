using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Business.Base;
using SendMail.Business.Contracts;
using SendMail.Model;
using SendMail.DataContracts.Interfaces;
using SendMail.Model.Wrappers;

namespace SendMail.Business
{
    class BackendUserService : BaseSingletonService<BackendUserService>, IBackendUserService
    {
        public BackendUser GetByUserName(String UserName)
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                return dao.GetByUserName(UserName);
            }
        }
        public List<BackendUser> GetDipartimentiByAdmin(string UserName)
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                return dao.GetDipartimentiByAdmin(UserName);
            }
        }

        public List<BackEndUserMailUserMapping> GetMailUserByUserId(long userId, int userRole)
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                return dao.GetMailUserByUserId(userId, userRole);
            }
        }

        public List<BackendUser> GetAllDipartimentiByMailAdmin(string UserName)
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                return dao.GetAllDipartimentiByMailAdmin(UserName);
            }

        }

        public List<BackendUser> GetAllDipartimenti()
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                return dao.GetAllDipartimenti();
            }
        }

        public List<BackendUser> GetDipendentiDipartimentoNONAbilitati(String dipartimento, Decimal idSender)
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                return dao.GetDipendentiDipartimentoNONAbilitati(dipartimento,idSender);
            }
        }

        public List<BackendUser> GetDipendentiDipartimentoAbilitati(Decimal idSender)
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                return dao.GetDipendentiDipartimentoAbilitati(idSender);
            }
        }

        public void InsertAbilitazioneEmail(Decimal UserId, Decimal idSender, int role)
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                dao.InsertAbilitazioneEmail(UserId, idSender, role);
            }
        }

        public void RemoveAbilitazioneEmail(Decimal UserId, Decimal idSender)
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                dao.RemoveAbilitazioneEmail(UserId, idSender);
            }
        }

        public void UpdateAbilitazioneEmail(decimal userId, decimal idsender, int level)
        {
            using (IBackendUserDao dao = this.getDaoContext().DaoImpl.BackendUserDao)
            {
                dao.UpdateAbilitazioneEmail(userId, idsender,level);
            }
        }

        public List<UserResultItem> GetStatsInBox(string account, string utente, string datainizio, string datafine)
        {
            using (IBackendUserDao dao = getDaoContext().DaoImpl.BackendUserDao)
            {
                return dao.GetStatsInBox(account, utente, datainizio, datafine);
            }
        }

        public string GetTotalePeriodoAccount(string account, string datainizio, string datafine)
        {
            using (IBackendUserDao dao = getDaoContext().DaoImpl.BackendUserDao)
            {
                return dao.GetTotalePeriodoAccount(account, datainizio, datafine);
            }
        }

    }
}

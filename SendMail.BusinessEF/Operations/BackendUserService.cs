using System;
using System.Collections.Generic;
using SendMail.BusinessEF.Base;
using SendMail.BusinessEF.Contracts;
using SendMail.Model;
using SendMail.Model.Wrappers;
using SendMail.Data.SQLServerDB;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF
{
   public class BackendUserService : BaseSingletonService<BackendUserService>, IBackendUserService
    {
        public BackendUser GetByUserName(String UserName)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                return dao.GetByUserName(UserName);
            }
        }
        public List<BackendUser> GetDipartimentiByAdmin(string UserName)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                return dao.GetDipartimentiByAdmin(UserName);
            }
        }

        public List<BackEndUserMailUserMapping> GetMailUserByUserId(long userId, int userRole)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                return dao.GetMailUserByUserId(userId, userRole);
            }
        }

        public List<BackendUser> GetAllDipartimentiByMailAdmin(string UserName)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                return dao.GetAllDipartimentiByMailAdmin(UserName);
            }

        }

        public List<BackendUser> GetAllDipartimenti()
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                return dao.GetAllDipartimenti();
            }
        }

        public List<BackendUser> GetDipendentiDipartimentoNONAbilitati(String dipartimento, Decimal idSender)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                return dao.GetDipendentiDipartimentoNONAbilitati(dipartimento,idSender);
            }
        }

        public List<BackendUser> GetDipendentiDipartimentoAbilitati(Decimal idSender)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                return dao.GetDipendentiDipartimentoAbilitati(idSender);
            }
        }

        public void InsertAbilitazioneEmail(Decimal UserId, Decimal idSender, int role)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                dao.InsertAbilitazioneEmail(UserId, idSender, role);
            }
        }

        public void RemoveAbilitazioneEmail(Decimal UserId, Decimal idSender)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                dao.RemoveAbilitazioneEmail(UserId, idSender);
            }
        }

        public void UpdateAbilitazioneEmail(decimal userId, decimal idsender, int level)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                dao.UpdateAbilitazioneEmail(userId, idsender,level);
            }
        }

        public List<UserResultItem> GetStatsInBox(string account, string utente, string datainizio, string datafine)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                return dao.GetStatsInBox(account, utente, datainizio, datafine);
            }
        }

        public string GetTotalePeriodoAccount(string account, string datainizio, string datafine)
        {
            using (BackEndUserSQLDb dao = new BackEndUserSQLDb())
            {
                return dao.GetTotalePeriodoAccount(account, datainizio, datafine);
            }
        }

    }
}

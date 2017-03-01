using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;

namespace SendMail.DataContracts.Interfaces
{
    public interface IBackendUserDao : IDao<BackendUser, Int64>
    {
        BackendUser GetByUserName(String UserName);
        List<BackEndUserMailUserMapping> GetMailUserByUserId(long userId, int userRole);
        List<BackendUser> GetAllDipartimenti();
        List<BackendUser> GetDipendentiDipartimentoNONAbilitati(String dipartimento, Decimal idSender);
        List<BackendUser> GetDipendentiDipartimentoAbilitati(Decimal idSender);
        void InsertAbilitazioneEmail(Decimal UserId, Decimal idSender, int role);
        void RemoveAbilitazioneEmail(Decimal UserId, Decimal idSender);
        List<BackendUser> GetDipartimentiByAdmin(string UserName);
        List<SendMail.Model.Wrappers.UserResultItem> GetStatsInBox(string account, string utente, string datainizio, string datafine);
        List<BackendUser> GetAllDipartimentiByMailAdmin(string UserName);
        string GetTotalePeriodoAccount(string account, string datainizio, string datafine);
        void UpdateAbilitazioneEmail(decimal userId, decimal idsender, int level);
    }
}

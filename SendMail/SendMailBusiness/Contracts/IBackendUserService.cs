using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;
using SendMail.Model.Wrappers;

namespace SendMail.Business.Contracts
{
    public interface IBackendUserService
    {
        BackendUser GetByUserName(String UserName);
        List<BackEndUserMailUserMapping> GetMailUserByUserId(long userId, int userRole);
        List<BackendUser> GetAllDipartimenti();
        List<BackendUser> GetDipartimentiByAdmin(string UserName);
        List<BackendUser> GetDipendentiDipartimentoNONAbilitati(String dipartimento, Decimal idSender);
        List<BackendUser> GetDipendentiDipartimentoAbilitati(Decimal idSender);
        void InsertAbilitazioneEmail(Decimal UserId, Decimal idSender, int role);
        void RemoveAbilitazioneEmail(Decimal UserId, Decimal idSender);

        List<BackendUser> GetAllDipartimentiByMailAdmin(string UserName);

        List<UserResultItem> GetStatsInBox(string account, string utente, string datainizio, string datafine);
        string GetTotalePeriodoAccount(string account, string datainizio, string datafine);
        void UpdateAbilitazioneEmail(decimal userId, decimal idsender, int level);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.ComunicazioniMapping;
using SendMail.Model.RubricaMapping;
using SendMail.Model.WebserviceMappings;

namespace SendMail.DataContracts.Interfaces
{
    public interface IComunicazioniDao : IDao<Comunicazioni, Int64>
    {
        Int32 GetCountComunicazioniInviate(SendMail.Model.TipoCanale tipoCanale, string utente);
        Int32 GetCountComunicazioniNonInviate(SendMail.Model.TipoCanale tipoCanale, string utente);
        ICollection<Comunicazioni> GetComunicazioniInviate(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente);
        ICollection<Comunicazioni> GetComunicazioniNonInviate(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente);
        ICollection<Comunicazioni> GetComunicazioniDaInviare(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente);
        ICollection<Comunicazioni> GetComunicazioniConAllegati();
        ICollection<Comunicazioni> GetComunicazioniSenzaAllegati();
        Comunicazioni GetComunicazioneByIdMail(Int64 idMail);

        void Insert(Int64 idSottotitolo, Int64 idCanale, Boolean isToNotify,
            String mailNotifica, String utenteInserimento, IList<ComAllegato> allegati,
            String mailSender, String oggetto, String testo, IList<RubricaContatti> refs);

        void UpdateFlussoComunicazione(SendMail.Model.TipoCanale tipoCanale, Comunicazioni comunicazione);
        void UpdateAllegati(SendMail.Model.TipoCanale tipoCanale, Comunicazioni comunicazione);
        void UpdateMailBody(long idMail, string mailBody);
        ICollection<StatoComunicazioneItem> GetStatoComunicazione(string originalUid);

        ICollection<StatoComunicazioneItem> GetComunicazioniByProtocollo(ComunicazioniProtocollo prot);
    }
}

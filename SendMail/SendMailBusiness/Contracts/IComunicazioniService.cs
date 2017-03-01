using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.ComunicazioniMapping;
using SendMail.Model.RubricaMapping;
using System.Xml;
using SendMail.Model.WebserviceMappings;

namespace SendMail.Business.Contracts
{
    public interface IComunicazioniService
    {
        //SendMail.Model.RawMessage LoadRawMessage(string appCode, string uid);
        // SendMail.Model.ComunicazioniMapping.Comunicazioni LoadComunicazione(string appCode, string uid);       
        byte[] GetPdfTpuStampeBUS(string tpu, byte[] pru, string pathFolder);
        byte[] GetPdfTpuStampeBUS(string appCode, string stringa_id, int progAllegato, string pathFolder);      
        byte[] GetPdfTpuStampeBUS(XmlDocument tpu, XmlDocument pru);

        Int32 LoadCountComunicazioniInviate(SendMail.Model.TipoCanale tipoCanale, string utente);
        Int32 LoadCountComunicazioniNonInviate(SendMail.Model.TipoCanale tipoCanale, string utente);

        ICollection<Comunicazioni> LoadComunicazioniInviate(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente);
        ICollection<Comunicazioni> LoadComunicazioniNonInviate(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente);
        ICollection<Comunicazioni> LoadComunicazioniDaInviare(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente);
        ICollection<Comunicazioni> LoadComunicazioniConAllegati();
        ICollection<Comunicazioni> LoadComunicazioniSenzaAllegati();
        Comunicazioni LoadComunicazioneByIdMail(Int64 idMail);
        ComAllegato LoadAllegatoComunicazioneById(long idAllegato);

        void InsertComunicazione(Int64 idSottotitolo, Int64 idCanale, Boolean isToNotify,
            String mailNotifica, String utenteInserimento, IList<ComAllegato> allegati,
            String mailSender, String oggetto, String testo, IList<RubricaContatti> refs);
        void InsertComunicazione(Comunicazioni entity);

        void UpdateFlussoComunicazione(SendMail.Model.TipoCanale tipoCanale, Comunicazioni comunicazione);
        void UpdateAllegati(SendMail.Model.TipoCanale tipoCanale, Comunicazioni comunicazione);
        void UpdateMailBody(long idMail, string mailBody);
        ICollection<Comunicazioni> GetAll();
        ICollection<StatoComunicazioneItem> GetComunicazioneStatus(string originalUid);

        ICollection<StatoComunicazioneItem> GetComunicazioniByProtocollo(ComunicazioniProtocollo prot);
    }
}

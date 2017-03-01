using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.DataContracts.Interfaces;
using Oracle.DataAccess.Client;

namespace SendMail.Data.OracleDb
{
    public class ComFlussoOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IComFlussoDao
    {
        #region "Private command text"

        private const string cmdInsert = "INSERT INTO COMUNICAZIONI_FLUSSO VALUES ("
                                       + ":p_ref_id_com, :p_stato_com_old, :p_stato_com_new, :p_data_ope, :p_ute_ope, :p_canale,null)";
        private const string cmdInsertNoDate = "INSERT INTO COMUNICAZIONI_FLUSSO(REF_ID_COM, STATO_COMUNICAZIONE_OLD, STATO_COMUNICAZIONE_NEW, UTE_OPE, CANALE) VALUES ("
                                       + ":p_ref_id_com, :p_stato_com_old, :p_stato_com_new, :p_ute_ope, :p_canale)";
        private const string cmdUpdate = "UPDATE COMUNICAZIONI_FLUSSO"
                                       + " SET STATO_COMUNICAZIONE_OLD = :p_STATO_COMUNICAZIONE_OLD,"
                                           + " STATO_COMUNICAZIONE_NEW = :p_STATO_COMUNICAZIONE_NEW,"
                                           + " DATA_OPERAZIONE = LOCALTIMESTAMP,"
                                           + " UTE_OPE = :p_UTE_OPE"
                                       + " WHERE ID_FLUSSO = :p_ID_FLUSSO";   //aggiornato il comando update da Alberto Coletti perché non utilizzava la chiave peimaria per localizzare il record da aggiornare

        #endregion

        #region "C.tor"

        private OracleSessionManager context;
        public ComFlussoOracleDb(OracleSessionManager daoContext)
            : base(daoContext)
        {
            this.context = daoContext;
            if (!base.Context.Session_isActive())
            {
                base.Context.Session_Init();
                base.CurrentConnection.Open();
            }
        }

        #endregion

        #region IDao<ComFlusso,long> Membri di

        public ICollection<SendMail.Model.ComunicazioniMapping.ComFlusso> GetAll()
        {
            throw new NotImplementedException();
        }

        public SendMail.Model.ComunicazioniMapping.ComFlusso GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Insert(SendMail.Model.ComunicazioniMapping.ComFlusso entity)
        {
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                if (entity.DataOperazione.HasValue)
                    oCmd.CommandText = cmdInsert;
                else
                    oCmd.CommandText = cmdInsertNoDate;
                oCmd.BindByName = true;
                oCmd.Parameters.Add("p_ref_id_com", entity.RefIdComunicazione);
                oCmd.Parameters.Add("p_stato_com_old", ((int)(entity.StatoComunicazioneOld)));
                oCmd.Parameters.Add("p_stato_com_new", ((int)(entity.StatoComunicazioneNew)));
                if (entity.DataOperazione.HasValue)
                {
                    oCmd.Parameters.Add("p_data_ope", entity.DataOperazione.Value);
                }
                //else
                //{
                //    oCmd.Parameters.Add(new OracleParameter("p_data_ope", null));
                //}
                oCmd.Parameters.Add("p_ute_ope", entity.UtenteOperazione);
                oCmd.Parameters.Add("p_canale", entity.Canale.ToString());

                try
                {
                    oCmd.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Update(SendMail.Model.ComunicazioniMapping.ComFlusso entity)
        {
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = cmdUpdate;
                oCmd.BindByName = true;
                oCmd.Parameters.Add("p_STATO_COMUNICAZIONE_OLD", (int)entity.StatoComunicazioneOld);
                oCmd.Parameters.Add("p_STATO_COMUNICAZIONE_NEW", (int)entity.StatoComunicazioneNew);
                oCmd.Parameters.Add("p_UTE_OPE", entity.UtenteOperazione);
                oCmd.Parameters.Add("p_ID_FLUSSO", entity.IdFlusso);
                
                try
                {
                    int rows = oCmd.ExecuteNonQuery();
                    if (rows != 1)
                        throw new Exception("Nessun record aggiornato");
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            if (base.Context.TransactionModeOn == false)
                base.CurrentConnection.Close();
        }

        #endregion
    }
}

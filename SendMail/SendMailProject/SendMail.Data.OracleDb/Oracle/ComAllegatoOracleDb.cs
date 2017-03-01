using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.DataContracts.Interfaces;
using Oracle.DataAccess.Client;

namespace SendMail.Data.OracleDb
{
    public class ComAllegatoOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IComAllegatoDao
    {
        #region "Private command string"

        private const string cmdUpdateAllegato = "UPDATE COMUNICAZIONI_ALLEGATI SET"
                                              + " ALLEGATO_FILE = :p_allegato_file"
                                              + ", ALLEGATO_EXT = :p_allegato_ext"
                                              + " WHERE ID_ALLEGATO = :p_id_all";

        #endregion

        #region "C.tor"

        private OracleSessionManager context;
        public ComAllegatoOracleDb(OracleSessionManager daoContext)
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

        #region IDao<ComAllegato,long> Membri di

        public ICollection<SendMail.Model.ComunicazioniMapping.ComAllegato> GetAll()
        {
            throw new NotImplementedException();
        }

        public SendMail.Model.ComunicazioniMapping.ComAllegato GetById(long id)
        {
            SendMail.Model.ComunicazioniMapping.ComAllegato alleg = null;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = "SELECT ID_ALLEGATO"
                                 + ", REF_ID_COM"
                                 + ", ALLEGATO_TPU"
                                 + ", ALLEGATO_FILE"
                                 + ", ALLEGATO_EXT"
                                 + ", FLG_INS_PROT"
                                 + ", FLG_PROT_TO_UPL"
                                 //+ ", PROT_REF"
                                 + ", ALLEGATO_NAME FROM COMUNICAZIONI_ALLEGATI WHERE ID_ALLEGATO = " + id;
                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        while (r.Read())
                        {
                            alleg = SendMail.Data.Utilities.DaoOracleDbHelper.MapToAllegatoComunicazione(r);
                        }
                    }
                }
                catch { }
            }
            return alleg;
        }

        public void Insert(SendMail.Model.ComunicazioniMapping.ComAllegato entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SendMail.Model.ComunicazioniMapping.ComAllegato entity)
        {
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = cmdUpdateAllegato;
                oCmd.Parameters.Add("p_allegato_file", entity.AllegatoFile);
                oCmd.Parameters.Add("p_allegato_ext", entity.AllegatoExt);
                oCmd.Parameters.Add("p_id_allegato", entity.IdAllegato.Value);

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

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComAllegatoDao

        public void Update(IList<SendMail.Model.ComunicazioniMapping.ComAllegato> entites)
        {
            if (!this.Context.TransactionModeOn)
                this.Context.StartTransaction(this.GetType());
            try
            {
                ((List<SendMail.Model.ComunicazioniMapping.ComAllegato>)entites).ForEach(x => Update(x));
            }
            catch
            {
                if (this.Context.TransactionRootElement == this.GetType())
                {
                    this.Context.RollBackTransaction(this.GetType());
                }
                throw;
            }

            if (this.Context.TransactionRootElement == this.GetType())
            {
                this.Context.EndTransaction(this.GetType());
            }
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

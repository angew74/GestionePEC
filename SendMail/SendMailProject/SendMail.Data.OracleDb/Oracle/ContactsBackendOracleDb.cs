using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.Contracts;
using SendMail.Model.ContactApplicationMapping;
using SendMail.Data.OracleDb;
using SendMail.DataContracts.Interfaces;
using log4net;
using Oracle.DataAccess.Client;
using SendMail.Data.Utilities;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;

namespace SendMail.Data.Oracle
{
    public class ContactsBackendOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>,
                                           IContactsBackendDao
    {
        private static ILog _log = LogManager.GetLogger("ContactsBackendOracleDb");

        #region Private fields
        private const string selectCmdBase = "SELECT * FROM RUBR_CONTATTI_BACKEND";
        private const string insertCmdBase = "INSERT INTO RUBR_CONTATTI_BACKEND("
                                            + " REF_ID_CANALE"
                                           + ", REF_ID_BACKEND"
                                           + ", REF_ID_CONTATTO"
                                           + ", REF_ID_TITOLO)"
                                           + " VALUES (:pID_CANALE, :pID_BACKEND, :pID_CONTATTO, :pID_TITOLO)"
                                           + " RETURNING ID_MAP INTO :pID_MAP";
        private const string updateCmdBase = " UPDATE RUBR_CONTATTI_BACKEND SET"
                                           + " REF_ID_CANALE = :pID_CANALE"
                                          + ", REF_ID_BACKEND = :pID_BACKEND"
                                          + ", REF_ID_CONTATTO = :pID_CONTATTO"
                                          + ", REF_ID_TITOLO = :pID_TITOLO"
                                           + " WHERE ID_MAP = :pID_MAP";
        private const string deleteCmdBase = "DELETE FROM RUBR_CONTATTI_BACKEND"
                                          + " WHERE ID_MAP = :pID_MAP";
        #endregion

        #region C.tor
        public ContactsBackendOracleDb(OracleSessionManager daoContext)
            : base(daoContext)
        {
            if (!Context.Session_isActive())
            {
                Context.Session_Init();
                CurrentConnection.Open();
            }
        }
        #endregion

        #region IContactsBackendDao Membri di

        public ICollection<ContactsBackendMap> GetPerEntita(int idEntita)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("WITH T_BACK AS (");
            sb.Append(" SELECT REF_ID_BACKEND");
            sb.Append(" FROM RUBR_CONTATTI_BACKEND");
            sb.Append(" WHERE REF_ID_ENTITA = :pID_ENTITA),");
            sb.Append("T_CONT_BACK AS (");
            sb.Append(" SELECT *");
            sb.Append(" FROM RUBR_CONTATTI_BACKEND");
            sb.Append(" WHERE REF_ID_BACKEND IN ");
            sb.Append("     (SELECT DISTINCT REF_ID_BACKEND");
            sb.Append("      FROM T_BACK))");
            sb.Append(" SELECT *");
            sb.Append(" FROM T_CONT_BACK");
            sb.Append(" UNION");
            sb.Append(" SELECT NULL, 1, REF_ID_BACKEND, NULL, ID_TITOLO, NULL");
            sb.Append(" FROM COMUNICAZIONI_TITOLI CT, T_BACK TB");
            sb.Append(" WHERE ID_TITOLO != 0");
            sb.Append(" AND NOT EXISTS (");
            sb.Append("     SELECT 1");
            sb.Append("     FROM T_CONT_BACK TCB");
            sb.Append("     WHERE TCB.REF_ID_TITOLO = CT.ID_TITOLO");
            sb.Append("     AND TCB.REF_ID_BACKEND = TB.REF_ID_BACKEND)");
            sb.Append(" ORDER BY 3, 5");

            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.Add("pID_ENTITA", OracleDbType.Decimal, idEntita, System.Data.ParameterDirection.Input);

                    ICollection<ContactsBackendMap> contacts = DaoOracleDbHelper<ContactsBackendMap>.ExecSelectCommand(cmd,
                        DaoOracleDbHelper.MapToContactsBackendMap);
                    if (contacts != null)
                    {
                        foreach (ContactsBackendMap c in contacts)
                        {
                            Complete(c);
                        }
                    }
                    return contacts;
                }
            }
            catch
            {
                throw;
            }
        }

        public ICollection<ContactsBackendMap> GetPerTitoloEntita(int idTitolo, int idEntita)
        {
            StringBuilder sb = new StringBuilder(selectCmdBase);
            sb.Append(" WHERE REF_ID_ENTITA = :pID_ENTITA");
            sb.Append(" AND REF_ID_TITOLO = :pID_TITOLO");
            sb.Append(" ORDER BY REF_ID_BACKEND, REF_ID_TITOLO");
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.Add("pID_ENTITA", OracleDbType.Decimal, idEntita, System.Data.ParameterDirection.Input);
                    cmd.Parameters.Add("pID_TITOLO", OracleDbType.Decimal, idTitolo, System.Data.ParameterDirection.Input);

                    ICollection<ContactsBackendMap> contacts = DaoOracleDbHelper<ContactsBackendMap>.ExecSelectCommand(cmd,
                        DaoOracleDbHelper.MapToContactsBackendMap);
                    if (contacts != null)
                    {
                        foreach (ContactsBackendMap c in contacts)
                        {
                            Complete(c);
                        }
                    }
                    return contacts;
                }
            }
            catch
            {
                throw;
            }
        }

        public ICollection<ContactsBackendMap> GetPerCanaleTitoloEntita(int idCanale, int idTitolo, int idEntita)
        {
            StringBuilder sb = new StringBuilder(selectCmdBase);
            sb.Append(" WHERE REF_ID_ENTITA = :pID_ENTITA");
            sb.Append(" AND REF_ID_TITOLO = :pID_TITOLO");
            sb.Append(" AND REF_ID_CANALE = :pID_CANALE");
            sb.Append(" ORDER BY REF_ID_BACKEND, REF_ID_TITOLO");
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.Add("pID_ENTITA", OracleDbType.Decimal, idEntita, System.Data.ParameterDirection.Input);
                    cmd.Parameters.Add("pID_TITOLO", OracleDbType.Decimal, idTitolo, System.Data.ParameterDirection.Input);
                    cmd.Parameters.Add("pID_CANALE", OracleDbType.Decimal, idCanale, System.Data.ParameterDirection.Input);

                    ICollection<ContactsBackendMap> contacts = DaoOracleDbHelper<ContactsBackendMap>.ExecSelectCommand(cmd,
                        DaoOracleDbHelper.MapToContactsBackendMap);
                    if (contacts != null)
                    {
                        foreach (ContactsBackendMap c in contacts)
                        {
                            Complete(c);
                        }
                    }
                    return contacts;
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region IDao<ContactsBackendMap,int> Membri di

        public ICollection<ContactsBackendMap> GetAll()
        {
            StringBuilder sb = new StringBuilder(selectCmdBase);
            sb.Append(" ORDER BY REF_ID_BACKEND, REF_ID_TITOLO");
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();

                    ICollection<ContactsBackendMap> contacts = DaoOracleDbHelper<ContactsBackendMap>.ExecSelectCommand(cmd,
                        DaoOracleDbHelper.MapToContactsBackendMap);
                    if (contacts != null)
                    {
                        foreach (ContactsBackendMap c in contacts)
                        {
                            Complete(c);
                        }
                    }
                    return contacts;
                }
            }
            catch
            {
                throw;
            }
        }

        public ContactsBackendMap GetById(int id)
        {
            StringBuilder sb = new StringBuilder(selectCmdBase);
            sb.Append(" WHERE ID_MAP = :pID_MAP");
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.Add("pID_MAP", OracleDbType.Decimal, id, System.Data.ParameterDirection.Input);

                    ContactsBackendMap contact = DaoOracleDbHelper<ContactsBackendMap>.ExecSelectCommand(cmd,
                        DaoOracleDbHelper.MapToContactsBackendMap).FirstOrDefault();
                    if (contact != null)
                    {
                        Complete(contact);
                    }
                    return contact;
                }
            }
            catch
            {
                throw;
            }
        }

        public void Insert(ContactsBackendMap entity)
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = insertCmdBase;
                    cmd.BindByName = true;
                    cmd.Parameters.AddRange(MapToParams(entity));
                    cmd.Parameters.Add("pID_MAP", OracleDbType.Decimal, System.Data.ParameterDirection.Output);

                    int risp = cmd.ExecuteNonQuery();
                    if (risp == 1)
                        entity.Id = Convert.ToInt32(cmd.Parameters["pID_MAP"].Value.ToString());
                }
            }
            catch
            {
                throw;
            }
        }

        public void Update(ContactsBackendMap entity)
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = updateCmdBase;
                    cmd.BindByName = true;
                    cmd.Parameters.AddRange(MapToParams(entity));
                    cmd.Parameters.Add("pID_MAP", OracleDbType.Decimal, entity.Id, System.Data.ParameterDirection.InputOutput);

                    int risp = cmd.ExecuteNonQuery();
                    if(risp!= 1)
                        throw new InvalidOperationException("Errore nell'aggiornamento");
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR009", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = Convert.ToString(entity.Id);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = deleteCmdBase;
                    cmd.BindByName = true;
                    cmd.Parameters.Add("pID_MAP", OracleDbType.Decimal, id, System.Data.ParameterDirection.Input);
                    int risp = cmd.ExecuteNonQuery();
                    if (risp != 1)
                        throw new InvalidOperationException("Errore nella cancellazione");
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR010", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = Convert.ToString(id);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        #endregion

        #region IDisposable Membri di
        public void Dispose()
        {
            if (Context.TransactionModeOn == false)
                CurrentConnection.Close();
        }
        #endregion

        #region Private methods
        private void Complete(ContactsBackendMap contact)
        {
            if (contact == null) return;
            if (contact.Backend != null)
                contact.Backend = Context.DaoImpl.BackEndCodeDao.GetById(contact.Backend.Id);
            if (contact.Contatto != null)
                contact.Contatto = Context.DaoImpl.ContattoDao.GetById(contact.Contatto.IdContact.Value);
            if (contact.Titolo != null)
                contact.Titolo = Context.DaoImpl.TitoloDao.GetById(contact.Titolo.Id);
        }

        private static OracleParameter[] MapToParams(ContactsBackendMap c)
        {
            OracleParameter[] pars = new OracleParameter[4];
            pars[0] = new OracleParameter("pID_CANALE", OracleDbType.Decimal, System.Data.ParameterDirection.Input);
            if (c.Canale != SendMail.Model.TipoCanale.UNKNOWN)
            {
                pars[0].Value = (int)c.Canale;
            }
            pars[1] = new OracleParameter("pID_BACKEND", OracleDbType.Decimal, System.Data.ParameterDirection.Input);
            if (c.Backend.Id != -1)
            {
                pars[1].Value = c.Backend.Id;
            }
            pars[2] = new OracleParameter("pID_CONTATTO", OracleDbType.Decimal, System.Data.ParameterDirection.Input);
            if (c.Contatto.IdContact.Value != -1)
            {
                pars[2].Value = c.Contatto.IdContact.Value;
            }
            pars[3] = new OracleParameter("pID_TITOLO", OracleDbType.Decimal, System.Data.ParameterDirection.Input);
            if (c.Titolo.Id != -1)
            {
                pars[3].Value = c.Titolo.Id;
            }
            return pars;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;
using Com.Delta.Data.QueryModel;
using Oracle.DataAccess.Client;
using SendMail.Data.Utilities;
using System.Data;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;

namespace SendMail.Data.OracleDb
{
    public class ContactsApplicationMappingOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>,
                                                      IContactsApplicationMappingDao
    {
        private static ILog _log = LogManager.GetLogger(typeof(ContactsApplicationMapping));
        #region "Private Fields"

        private const string selFields = "ID_MAP, ID_TITOLO, APP_CODE, TITOLO, TITOLO_PROT_CODE, TITOLO_ACTIVE"
                                     + ", ID_SOTTOTITOLO, SOTTOTITOLO, SOTTOTITOLO_PROT_CODE, SOTTOTITOLO_ACTIVE, COM_CODE"
                                     + ", ID_CONTACT, MAIL, FAX, TELEFONO, REF_ID_REFERRAL, FLG_PEC, REF_ORG, ID_CANALE"
                                     + ", CODICE, ID_BACKEND, BACKEND_CODE, BACKEND_DESCR, CATEGORY, DESCR_PLUS";

        private const String cmdSelectRoot = "SELECT " + selFields + " FROM V_MAP_APPL_CONTATTI_NEW";
        private const String cmdSetDefaultContact = " UPDATE RUBR_CONTATTI_BACKEND SET REF_ID_CONTATTO = :pCONTATTO WHERE " +
          " REF_ID_TITOLO = :pIDTITOLO AND REF_ID_ENTITA= :pIDENTITA ";

        #endregion

        #region "C.tor"

        /// <summary>
        /// C.tor della classe
        /// </summary>
        /// <param name="daoContext"></param>
        public ContactsApplicationMappingOracleDb(OracleSessionManager daoContext)
            : base(daoContext)
        {
            if (!base.Context.Session_isActive())
            {
                base.Context.Session_Init();
                base.CurrentConnection.Open();
            }
        }

        #endregion

        #region IContactsApplicationMappingDao Membri di

        public ICollection<ContactsApplicationMapping> GetContactsByCriteria(QueryCmp query)
        {
            List<ContactsApplicationMapping> listContacts = null;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = cmdSelectRoot + TranslateQuery(query, oCmd);
                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listContacts = new List<ContactsApplicationMapping>();
                            while (r.Read())
                            {
                                listContacts.Add((ContactsApplicationMapping)DataMapper.FillObject(r, typeof(ContactsApplicationMapping)));
                            }
                        }
                    }
                }
                catch
                {
                    //todo
                    throw;
                }
            }
            return listContacts;
        }

        public ICollection<ContactsApplicationMapping> FindByBackendCodeAndCodComunicazione(IEnumerable<string> codes, string codCom)
        {
            List<ContactsApplicationMapping> listContacts = null;
            string cmdText = cmdSelectRoot
                           + " WHERE"
                           + " BACKEND_CODE in ('" + String.Join("','", codes.ToArray()) + "')";
            if (!string.IsNullOrEmpty(codCom)) cmdText = cmdText + " and COM_CODE = '" + codCom + "'";
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = cmdText;
                using (OracleDataReader r = oCmd.ExecuteReader())
                {
                    //if (r.HasRows)
                    //{
                        listContacts = new List<ContactsApplicationMapping>();
                        while (r.Read())
                        {
                            listContacts.Add((ContactsApplicationMapping)DataMapper.FillObject(r, typeof(ContactsApplicationMapping)));
                        }
                    //}
                }
            }
            return listContacts;
        }

        public ICollection<ContactsApplicationMapping> GetContactsByIdContact(long idContact)
        {
            List<ContactsApplicationMapping> res = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT * FROM V_MAP_APPL_CONTATTI_NEW WHERE ID_CONTACT = " + idContact;

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            res = new List<ContactsApplicationMapping>();
                            while (r.Read())
                            {
                                res.Add((ContactsApplicationMapping)DataMapper.FillObject(r, typeof(ContactsApplicationMapping)));
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return res;
        }

        public void setDefaultContact(long idTitolo, long refOrg, long idContatto)
        {
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {

                oCmd.CommandText = cmdSetDefaultContact;
                oCmd.Parameters.Add(new OracleParameter("pCONTATTO", OracleDbType.Int64, idContatto, ParameterDirection.Input));
                oCmd.Parameters.Add(new OracleParameter("pIDTITOLO", OracleDbType.Int64, idTitolo, ParameterDirection.Input));
                oCmd.Parameters.Add(new OracleParameter("pIDENTITA", OracleDbType.Int64, refOrg, ParameterDirection.Input));
                oCmd.BindByName = true;
                try
                {
                    int ret = oCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //TASK: Allineamento log - Ciro
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException(ex.Message,
                            "ORA_ERR007", string.Empty, string.Empty, ex);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        _log.Error(err);
                        throw mEx;
                    }
                    else throw ex;
                }
            }
        }
        #endregion

        #region IDao<ContactsApplicationMapping,long> Membri di

        public ICollection<ContactsApplicationMapping> GetAll()
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = cmdSelectRoot;
                    return DaoOracleDbHelper<ContactsApplicationMapping>.ExecSelectCommand(cmd,
                        DaoOracleDbHelper.MapToContactsApplicationMapping);
                }
            }
            catch
            {
                throw;
            }
        }

        public ContactsApplicationMapping GetById(long id)
        {
            StringBuilder sb = new StringBuilder(cmdSelectRoot);
            sb.Append(" WHERE ID_MAP = :pID_MAP");
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.Add("pID_MAP", OracleDbType.Decimal, id, ParameterDirection.Input);

                    return DaoOracleDbHelper<ContactsApplicationMapping>.ExecSelectCommand(cmd,
                        DaoOracleDbHelper.MapToContactsApplicationMapping).FirstOrDefault();

                }
            }
            //catch (OracleException oex)
            //{
            //    throw oex;
            //}
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR008", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = Convert.ToString(id);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        public void Insert(ContactsApplicationMapping entity)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO RUBR_CONTATTI_BACKEND(");
            sb.Append("REF_ID_CANALE, REF_ID_BACKEND, REF_ID_CONTATTO, REF_ID_TITOLO");
            sb.Append(") VALUES (");
            sb.Append(":pID_CANALE, :pID_BACKEND, :pID_CONTATTO, :pID_TITOLO");
            sb.Append(")");
            sb.Append(" RETURNING ID_MAP INTO :pID_MAP");
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.AddRange(MapToOracleParameter(entity));
                    cmd.Parameters.Add("pID_MAP", OracleDbType.Decimal, ParameterDirection.Output);
                    int risp = cmd.ExecuteNonQuery();
                    if (risp == 1)
                        entity.IdMap = (long)cmd.Parameters["pID_MAP"].Value;
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
                    err.objectID = Convert.ToString(entity.RefIdReferral);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        public void Update(ContactsApplicationMapping entity)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE RUBR_CONTATTI_BACKEND");
            sb.Append(" SET REF_ID_CANALE = :pID_CANALE");
            sb.Append(", REF_ID_BACKEND = :pID_BACKEND");
            sb.Append(", REF_ID_CONTATTO = :pID_CONTATTO");
            sb.Append(", REF_ID_TITOLO = :pID_TITOLO");
            sb.Append(" WHERE ID_MAP = :pID_MAP");

            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.AddRange(MapToOracleParameter(entity));
                    cmd.Parameters.Add("pID_MAP", OracleDbType.Decimal, entity.IdMap, ParameterDirection.Input);
                    int risp = cmd.ExecuteNonQuery();
                    if (risp != 1)
                        throw new InvalidOperationException("Aggiornamento non effettuato");
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
                    err.objectID = Convert.ToString(entity.RefIdReferral);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        public void Delete(long id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM RUBR_CONTATTI_BACKEND");
            sb.Append(" WHERE ID_MAP = :pID_MAP");
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.Add("pID_MAP", OracleDbType.Decimal, id, ParameterDirection.Input);
                    int risp = cmd.ExecuteNonQuery();
                    if (risp != 1)
                        throw new InvalidOperationException("Cancellazione non effettuata");
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

        /// <summary>
        /// Dispose della classe.
        /// </summary>
        public void Dispose()
        {
            if (base.Context.TransactionModeOn == false)
                base.CurrentConnection.Close();
        }

        #endregion

        #region "Private methods"

        private string TranslateQuery(QueryCmp q, OracleCommand oCmd)
        {
            string cmdText = String.Empty;

            if (q.WhereClause != null)
            {
                cmdText += " WHERE";
                int k = 0;
                cmdText += TranslateIClause(q.WhereClause, oCmd, ref k);
            }

            return cmdText;
        }

        private string TranslateIClause(IClause clause, OracleCommand oCmd, ref int k)
        {
            String cmdText = "";
            if (clause is SingleClause)
            {
                SingleClause sc = clause as SingleClause;

                if (sc.Operator == CriteriaOperator.IsNull || sc.Operator == CriteriaOperator.IsNotNull)
                {
                    cmdText += String.Format("({0} {1})", sc.PropertyName, DaoOracleDbHelper.TranslateOperator(sc.Operator));
                }
                else
                {
                    string par = String.Format("p{0}", k++);
                    cmdText += String.Format("({0} {1} :{2})", sc.PropertyName, DaoOracleDbHelper.TranslateOperator(sc.Operator), par);
                    try
                    {
                        oCmd.Parameters.Add(par, ((SingleClause)clause).Value.ToString());
                    }
                    catch
                    {
                    }
                }
            }
            else if (clause is CombinedClause)
            {
                cmdText += "(";
                CombinedClause cc = clause as CombinedClause;
                for (int j = 0; j < cc.SubClauses.Count() - 1; j++)
                {
                    cmdText += String.Format("{0} {1} ", TranslateIClause(cc.SubClauses.ElementAt(j), oCmd, ref k),
                        cc.ClauseType.ToString().ToUpper());
                }
                cmdText += String.Format("{0})", TranslateIClause(cc.SubClauses.Last(), oCmd, ref k));
            }
            return cmdText;
        }

        private String GetColumn(String propertyName)
        {
            String col = "";
            switch (propertyName)
            {
                case "appCode":
                    col = "App_CODE";
                    break;

                case "mail":
                    col = "MAIL";
                    break;

                case "fax":
                    col = "FAX";
                    break;

                case "telefono":
                    col = "TELEFONO";
                    break;

                case "contactRef":
                    col = "CONTACT_REF";
                    break;

                case "codiceCanale":
                    col = "CODICE";
                    break;

                case "codiceBackend":
                    col = "BACKEND_CODE";
                    break;

                case "descrBackend":
                    col = "BACKEND_DESCR";
                    break;

                case "descrBackendPlus":
                    col = "DESCR_PLUS";
                    break;

                case "categoria":
                    col = "CATEGORY";
                    break;
            }
            return col;
        }

        private OracleParameter[] MapToOracleParameter(ContactsApplicationMapping map)
        {
            OracleParameter[] pars = new OracleParameter[4];
            pars[0] = new OracleParameter("pID_CANALE", OracleDbType.Decimal, map.IdCanale, ParameterDirection.Input);
            pars[1] = new OracleParameter("pID_BACKEND", OracleDbType.Decimal, map.IdBackend, ParameterDirection.Input);
            pars[2] = new OracleParameter("pID_CONTATTO", OracleDbType.Decimal, map.IdContact, ParameterDirection.Input);
            pars[3] = new OracleParameter("pID_TITOLO", OracleDbType.Decimal, map.IdTitolo, ParameterDirection.Input);
            return pars;
        }

        #endregion
    }
}

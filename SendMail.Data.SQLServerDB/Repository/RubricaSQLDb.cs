using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class RubricaSQLDb : IRubricaDao
    {
        private static readonly ILog log = LogManager.GetLogger("RubricaSQLDb");
        #region IDao<Rubrica> Membri di 

        public Rubrica GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Rubrica dao)
        {
            throw new NotImplementedException();
        }

        public void Delete(long Id)
        {
            throw new NotImplementedException();
        }

        public void Update(Rubrica dao)
        {
            throw new NotImplementedException();
        }

        public int Save(Rubrica dao)
        {
            throw new NotImplementedException();
        }

        public ICollection<Rubrica> GetAll()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRuricaDao Membri di

        public Rubrica GetRubricaItem(Int64 idrub)
        {
            throw new NotImplementedException();
            //Rubrica dao = new Rubrica();

            //try
            //{
            //    using (var dbcontext = new FAXPECContext())
            //    {
            //        dbcontext.ru
            //        oCmd.CommandText = cmdSelectIdRubrica;
            //        oCmd.Parameters.Add("RUBID", idrub);
            //        using (OracleDataReader r = oCmd.ExecuteReader())
            //        {
            //            dao = (Rubrica)DataMapper.FillObject(r, typeof(Rubrica));
            //        }

            //    }
            //}           
            //catch (Exception ex)
            //{
            //    //TASK: Allineamento log - Ciro
            //    if (!ex.GetType().Equals(typeof(ManagedException)))
            //    {
            //        ManagedException mEx = new ManagedException("Errore nel caricamento dal database di un elemento della rubrica Data Layer E035 Dettagli Errore: " + ex.Message,
            //            "ERR_035", string.Empty, string.Empty, ex.InnerException);
            //        ErrorLogInfo err = new ErrorLogInfo(mEx);

            //        //err.objectID = string.Empty;
            //        //err.userID = System.DateTime.Now.Date.ToString() + " - " + System.DateTime.Now.Ticks.ToString();
            //        log.Error(err);
            //        throw mEx;
            //    }
            //    else
            //        throw ex;
            //    
            //}

            //return dao;
        }

        public ICollection<Rubrica> GetRubricaItems(string[] codici)
        {
            throw new NotImplementedException();
            //ICollection<Rubrica> list = null;
            //for (int i = 0; i < codici.Length; i++)
            //{
            //    try
            //    {
            //        Rubrica dao = new Rubrica();
            //        string idrub = codici[i];
            //        using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            //        {

            //            oCmd.CommandText = PrepareQueryStatement(codici);
            //            using (OracleDataReader r = oCmd.ExecuteReader())
            //            {

            //                list = (DaoOracleDbHelper<Rubrica>.ExecSelectCommand(oCmd, DaoOracleDbHelper.MapToRubrica) as ICollection<Rubrica>);

            //            }

            //        }
            //    }                
            //    catch (Exception ex)
            //    {
                    
            //        if (!ex.GetType().Equals(typeof(ManagedException)))
            //        {
            //            ManagedException mEx = new ManagedException("Errore nel caricamento dal database degli elementi della rubrica Data Layer E034 Dettagli Errore: " + ex.Message,
            //                "ERR_034", string.Empty, string.Empty, ex.InnerException);
            //            ErrorLogInfo err = new ErrorLogInfo(mEx);

            //            log.Error(err);
            //            throw mEx;
            //        }
            //        else
            //            throw ex;                    
            //    }
            //}
            //return list;
        }

        public Dictionary<string, SimpleTreeItem> LoadTree(Int64? startNode, SendMail.Model.IndexedCatalogs catalog, int? levels)
        {
            Dictionary<string, SimpleTreeItem> list = null;
            StringBuilder query = new StringBuilder();
             switch (catalog)
                {
                    case IndexedCatalogs.RUBR:
                        query.Append("select ID_REFERRAL AS VALUE, ");
                        query.Append("CASE WHEN REFERRAL_TYPE in ('PA','AZ_PRI','AZ_CP','GRP') then RAGIONE_SOCIALE else UFFICIO end as TEXT, ");
                        query.Append("REFERRAL_TYPE AS SUBTYPE, ");

                        query.Append("'RUBR' AS SOURCE, ");
                        query.Append("ID_PADRE AS PADRE ");
                        query.Append("from rubr_entita ");
                        query.Append("where REFERRAL_TYPE in ('PA','PA_SUB','PA_UFF','AZ_PRI','AZ_CP','AZ_UFF','GRP') ");
                        if (levels.HasValue) query.Append("and level <=" + levels);
                        if (startNode.HasValue) query.Append("start with ID_REFERRAL = :pID ");
                        else query.Append("start with ID_REFERRAL = 0 ");
                        query.Append("connect by NOCYCLE prior ID_REFERRAL = ID_PADRE ");
                        break;
                    case IndexedCatalogs.IPA:
                        query.Append("select ID_RUB AS VALUE, ");
                        query.Append("'IPA' AS SOURCE, ");

                        query.Append("CASE WHEN ( substr(DN,1,2)= 'c=' or substr(DN,1,2) = 'o=') then RAGIONESOCIALE ");
                        query.Append("else UFFICIO end AS TEXT, ");
                        query.Append("CASE WHEN substr(DN,1,2)= 'c=' then 'GRP' ");
                        query.Append("WHEN substr(DN,1,2)= 'o=' then 'PA' ");
                        query.Append("else 'PA_UFF' end AS SUBTYPE, ");
                        query.Append("ID_PADRE AS PADRE ");
                        query.Append("from IPA ");
                        if (levels.HasValue) query.Append("where level <="+ levels.Value);
                        if (startNode.HasValue) query.Append("start with ID_RUB = "+ startNode.Value);
                        else query.Append("start with ID_RUB = 1 ");
                        query.Append("connect by NOCYCLE prior ID_RUB = ID_PADRE");

                        break;
                }
            using (var dbcontext = new FAXPECContext())
            {
                using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                {
                    try
                    {
                        oCmd.CommandText = query.ToString();                      
                        using (var r = oCmd.ExecuteReader())
                            if (r.HasRows)
                            {
                                list = new Dictionary<string, SimpleTreeItem>();
                                bool skip = false;
                                while (r.Read())
                                {
                                    if (!skip)
                                    {
                                        SimpleTreeItem it = DaoSQLServerDBHelper.MapToSimpleTreeItem(r);
                                        list.Add(it.ExtendedValue, it);
                                    }
                                }
                            }
                    }
                    catch (Exception ex)
                    {

                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel caricamento dal database dell'albero Data Layer E035 Dettagli Errore: " + ex.Message,
                                "ERR_035", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);

                            log.Error(err);
                            throw mEx;
                        }
                        else
                            throw ex;

                    }
                }
            }
            return list;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Per rilevare chiamate ridondanti

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminare lo stato gestito (oggetti gestiti).
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di un finalizzatore.
                // TODO: impostare campi di grandi dimensioni su Null.

                disposedValue = true;
            }
        }

        // TODO: eseguire l'override di un finalizzatore solo se Dispose(bool disposing) include il codice per liberare risorse non gestite.
        // ~RubricaSQLDb() {
        //   // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
        //   Dispose(false);
        // }

        // Questo codice viene aggiunto per implementare in modo corretto il criterio Disposable.
        public void Dispose()
        {
            // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
            Dispose(true);
            // TODO: rimuovere il commento dalla riga seguente se è stato eseguito l'override del finalizzatore.
            // GC.SuppressFinalize(this);
        }
        #endregion

        //private string PrepareQueryStatement(string[] codici)
        //{
        //    StringBuilder query = new StringBuilder();
        //    query.Append(" SELECT * from RUBRICA WHERE  ");
        //    query.Append("ID_RUB = " + Int64.Parse(codici[0]));
        //    for (int i = 1; i < codici.Length; i++)
        //    {
        //        query.Append(" or ID_RUB = " + Int64.Parse(codici[i]));
        //    }
        //    return query.ToString();
        //}


        #endregion
    }
}

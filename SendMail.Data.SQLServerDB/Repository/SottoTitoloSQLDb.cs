﻿using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model;
using SendMailApp.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class SottoTitoloSQLDb : ISottoTitoloDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SottoTitoloSQLDb));
        #region "ISottoTitoloDao members"

        /// <summary>
        /// Restituisce l'intera collezione degli oggetti SottoTitolo.
        /// </summary>
        /// <returns></returns>
        public ICollection<SottoTitolo> GetAll()
        {
            List<SottoTitolo> sottotitoli = null;
            using (var dbcontext = new FAXPECContext())
            {               
                try
                {
                  var l =  dbcontext.COMUNICAZIONI_SOTTOTITOLI.ToList();
                   foreach(COMUNICAZIONI_SOTTOTITOLI s in l)
                    {
                        sottotitoli.Add(DaoSQLServerDBHelper.MapToSottotitolo(s));
                    }
                }              
                catch (SqlException oex)
                {                   
                    ManagedException mEx = new ManagedException(DalExMessages.SOTTOTITOLO_NON_RECUPERATO,
                        "DAL_SOT_001", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(mEx);
                    throw mEx;                   
                }
            }
            return sottotitoli;
        }

        /// <summary>
        /// Restituisce la collezione di oggetti SottoTitolo
        /// collegati al Titolo passato.
        /// </summary>
        /// <param name="titoloKey">identificativo del Titolo</param>
        /// <returns>una collection di oggetti SottoTitolo</returns>
        public ICollection<SottoTitolo> FindByTitolo(decimal titoloKey)
        {
            List<SottoTitolo> sottotitoli = null;
            using (var dbcontext = new FAXPECContext())
            {              
                         
                try
                {
                    var l = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.REF_ID_TITOLO == titoloKey).ToList();
                    if(l.Count > 0)
                    {
                        sottotitoli = new List<SottoTitolo>();
                        foreach(COMUNICAZIONI_SOTTOTITOLI t in l)
                        {
                           sottotitoli.Add(DaoSQLServerDBHelper.MapToSottotitolo(t));
                        }
                    }                   
                }            
                catch (SqlException oex)
                {                  
                    ManagedException mEx = new ManagedException(DalExMessages.SOTTOTITOLO_NON_RECUPERATO,
                        "DAL_SOT_002", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;                  
                }
            }
            return sottotitoli;
        }

        #endregion

        #region ISottoTitoloDao Membri di


        #endregion

        #region ISottoTitoloDao Membri di
         

        public SottoTitolo GetSottoTitoloByComCode(string comcode)
        {
            SottoTitolo sottotitolo = null;
            using (var dbcontext = new FAXPECContext())
            {            
            
                try
                {
                    var s = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.COM_CODE.ToUpper() == comcode.ToUpper()).First();
                    sottotitolo = DaoSQLServerDBHelper.MapToSottotitolo(s);
                }               
                catch (SqlException oex)
                {                  
                    ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO,
                        "DAL_TIT_011", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;
                }
            }
            return sottotitolo;
        }

        public SottoTitolo GetById(decimal id)
        {
            SottoTitolo sottotitolo = null;
            using (var dbcontext = new FAXPECContext())
            {    
                try
                {
                    var s = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.ID_SOTTOTITOLO == id).First();
                    sottotitolo = DaoSQLServerDBHelper.MapToSottotitolo(s);
                }              
                catch (SqlException oex)
                {                   
                    ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO,
                        "DAL_TIT_010", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;
                }
            }
            return sottotitolo;
        }

        public void Insert(SottoTitolo sottoTitolo)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    COMUNICAZIONI_SOTTOTITOLI s = DaoSQLServerDBHelper.MapToComunicazioniSottotitolo(sottoTitolo,true);
                    dbcontext.COMUNICAZIONI_SOTTOTITOLI.Add(s);
                    var resp = dbcontext.SaveChanges();
                    if (resp == 0)
                    {
                        throw new Exception(DalExMessages.ID_NON_RESTITUITO);
                    }
                }           
                catch (InvalidOperationException ioex)
                {                    
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA,
                        "DAL_RUB_002", string.Empty,
                        string.Empty, ioex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;
                }                
                catch (SqlException oex)
                {
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA,
                        "DAL_RUB_001", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;                   

                }
            }
        }

        public void Update(SottoTitolo sottoTitolo)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {

                    COMUNICAZIONI_SOTTOTITOLI s = DaoSQLServerDBHelper.MapToComunicazioniSottotitolo(sottoTitolo, false);
                    var olds = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.ID_SOTTOTITOLO == sottoTitolo.Id).First();
                    dbcontext.COMUNICAZIONI_SOTTOTITOLI.Remove(olds);
                    dbcontext.COMUNICAZIONI_SOTTOTITOLI.Add(s);
                    int rowAff =dbcontext.SaveChanges();
                    if (rowAff == 0)                 
                    {                      
                        ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                            "DAL_TIT_009", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        log.Error(mEx);
                        throw mEx;
                    }                

                }              
                catch (InvalidOperationException ex)
                {
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA,
                        "DAL_UNIQUE_CODE", string.Empty,
                        string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;                 
                }
            }
        }

        public void Delete(decimal id)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var s = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.ID_SOTTOTITOLO == id).First();
                    dbcontext.COMUNICAZIONI_SOTTOTITOLI.Remove(s);
                    int rows = dbcontext.SaveChanges();
                    if (rows != 1)
                    {                      
                        ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                            "DAL_TIT_009", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        log.Error(mEx);
                        throw mEx;
                    }                 
                }              
                catch (Exception ex)
                {                  
                    if (ex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA,
                                    "DAL_UNIQUE_CODE", string.Empty,
                                    string.Empty, ex);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        log.Error(mEx);
                        throw mEx;
                    }
                    else throw;                 
                }
            }
        }

        public void DeleteLogic(decimal id)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var s = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.ID_SOTTOTITOLO == id).First();
                    s.ACTIVE = 0;
                    int rowAff =dbcontext.SaveChanges();
                    if (rowAff != 1)
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                                               "DAL_SOT_009", string.Empty,
                                               string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        log.Error(mEx);
                        throw mEx;
                    }

                }               
                catch (InvalidOperationException ex)
                {

                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA,
                        "DAL_SOT_034", string.Empty,
                        string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;               
                }
            }
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
        // ~SottoTitoloSQLDb() {
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



        #endregion
    }
}

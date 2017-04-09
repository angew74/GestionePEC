using Com.Delta.Logging;
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
    public class TitoloSQLDb : ITitoloDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TitoloSQLDb));

        #region "ITitoloDao members"

        public Titolo GetById(decimal id)
        {
            Titolo titolo = null;
            using (var dbcontext = new FAXPECContext())
            {                
                // eseguo il command
                try
                {
                    var t = dbcontext.COMUNICAZIONI_TITOLI.Where(x => x.ID_TITOLO == id).First();
                    titolo = AutoMapperConfiguration.MapToTitolo(t);                 
                }
                catch (SqlException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO,
                        "DAL_TIT_010", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO, "DAL_TIT_010", "", "", "", "", "", oex);
                }
            }
            return titolo;
        }

        public ICollection<Titolo> GetAll()
        {
            List<Titolo> titoli = null;
            using (var dbcontext = new FAXPECContext())
            {
                // preparo il command
                try
                {
                    List<COMUNICAZIONI_TITOLI> l = dbcontext.COMUNICAZIONI_TITOLI.ToList();
                    foreach (COMUNICAZIONI_TITOLI t in l)
                    {
                        var titolo = AutoMapperConfiguration.MapToTitolo(t);
                        titoli.Add(titolo);
                    }
                }
                catch (SqlException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO,
                        "DAL_TIT_001", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(mEx);
                    throw mEx;
                }
            }
            return titoli;
        }

        public ICollection<Titolo> GetSottotitoliByIdPadre(decimal titoloKey)
        {

            List<Titolo> collection = null;
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var ls = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.REF_ID_TITOLO == titoloKey).ToList();
                    foreach (var s in ls)
                    {
                        Titolo t = AutoMapperConfiguration.MapSottotitoliToTitoli(s);
                        collection.Add(t);
                    }
                }
                catch (SqlException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO,
                        "DAL_TIT_001", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;
                }
            }
                return collection;
        }


        //Inserisce un nuovo titolo, automaticamente viene inserito anche il sottotiolo di default
        public void Insert(Titolo titolo)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var transaction = dbcontext.Database.BeginTransaction())
                {
                    SottoTitolo s1 = new SottoTitolo(titolo);
                    s1.Id = 0;
                    s1.RefIdTitolo = titolo.Id;
                    COMUNICAZIONI_TITOLI t = AutoMapperConfiguration.MapToComunicazioniTitoli(titolo, true);
                    try
                    {

                        dbcontext.COMUNICAZIONI_TITOLI.Add(t);
                        dbcontext.SaveChanges();
                        //param out
                        Int64 iNewID = default(Int64);
                        Int64.TryParse(t.ID_TITOLO.ToString(), out iNewID);
                        s1.RefIdTitolo = iNewID;
                        COMUNICAZIONI_SOTTOTITOLI s = DaoSQLServerDBHelper.MapToComunicazioniSottotitolo(s1, true);
                        dbcontext.COMUNICAZIONI_SOTTOTITOLI.Add(s);
                        //todo.. MIGLIORARE
                        if (iNewID != default(int))
                        {
                            titolo.Id = iNewID;

                        }
                        else
                            throw new Exception(DalExMessages.ID_NON_RESTITUITO);
                    }
                    /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
                    catch (InvalidOperationException ioex)
                    {
                        //throw new DALException(DalExMessages.RUBRICA_NON_INSERITA, ioex);
                        transaction.Rollback();
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA,
                            "DAL_RUB_002", string.Empty,
                            string.Empty, ioex);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        log.Error(mEx);
                        throw mEx;

                        //throw new ManagedException(DalExMessages.RUBRICA_NON_INSERITA, "DAL_RUB_002", "", "", "", "", "", ioex);

                    }
                    catch (SqlException oex)
                    {
                        //throw new DALException(DalExMessages.RUBRICA_NON_INSERITA, oex);
                        transaction.Rollback();
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA,
                            "DAL_RUB_001", string.Empty,
                            string.Empty, oex);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        log.Error(mEx);
                        throw mEx;

                        //throw new ManagedException(DalExMessages.RUBRICA_NON_INSERITA, "DAL_RUB_001", "", "", "", "", "", oex);

                    }
                }
            }
        }

        public void Update(Titolo titolo)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var transaction = dbcontext.Database.BeginTransaction())
                {                    
                    try
                    {
                        var updated = dbcontext.COMUNICAZIONI_TITOLI.Where(x => x.ID_TITOLO == titolo.Id).First();
                        updated.APP_CODE = titolo.AppCode;
                        updated.NOTE = titolo.Note;
                        updated.TITOLO = titolo.Nome;
                        updated.PROT_CODE = titolo.CodiceProtocollo;

                        int rowAff = dbcontext.SaveChanges();
                        //todo.. MIGLIORARE
                        if (rowAff != 1)
                        //throw new DALException(DalExMessages.NESSUNA_RIGA_MODIFICATA);
                        {
                            //TASK: Allineamento log - Ciro
                            ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                                "DAL_TIT_009", string.Empty,
                                string.Empty, null);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);

                            log.Error(mEx);
                            throw mEx;
                        }
                        //throw new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA, "DAL_TIT_009", "", "", "", "", "", null);

                    }
                    /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
                    catch (InvalidOperationException ex)
                    {

                        //throw new DALException(DalExMessages.RUBRICA_NON_AGGIORNATA, ex);

                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA,
                            "DAL_UNIQUE_CODE", string.Empty,
                            string.Empty, ex);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        log.Error(mEx);
                        throw mEx;
                        //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
                    }
                }
            }
        }

        //1-se i titoli hanno referenze da 'RICHIESTE' la cancellazione deve essere logica
        //2-se i tioli hanno refrenze a sottotiotli prima devono essere cancellati i sottotitoli
        //3- se però tutti i sottotitoli sono cancellati logicamente allora si può procedere con la cancellazione logica
        public void Delete(decimal id)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var transaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        var deleted = dbcontext.COMUNICAZIONI_TITOLI.Where(x => x.ID_TITOLO == id).First();
                        var st = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.REF_ID_TITOLO == id).ToList();
                        int sottoTitoli = st.Count;
                        int sottoTitoliAttivi = 0;
                        decimal lastActive = -1;
                        if (st.Count > 1)
                        {
                            foreach (COMUNICAZIONI_SOTTOTITOLI t in st)
                            {
                                if (t.ACTIVE != 0)
                                {
                                    sottoTitoliAttivi++;
                                    lastActive = t.ID_SOTTOTITOLO;
                                }
                            }
                            if (sottoTitoliAttivi > 1)
                            {
                                transaction.Rollback();
                                ManagedException mEx = new ManagedException("ATTENZIONE!!! Cancellare prima i sottotitoli associati questo titolo",
                                    "DAL_TIT_0015", string.Empty,
                                    string.Empty, null);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);

                                log.Error(mEx);
                                throw mEx;
                            }
                            else if (sottoTitoliAttivi == 1)
                            {
                                deleted.ACTIVE = 0;
                                var oldsottotitolo = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.ID_SOTTOTITOLO == lastActive).First();
                                oldsottotitolo.ACTIVE = 0;
                                int rows = dbcontext.SaveChanges();
                            }
                        }
                        else if (st.Count == 1)
                        {
                            //cancello fisicamente i record del titolo e del sottotitolo di default
                            //TODO:occorre aggiungere la parte relativa al controllo sulle richieste associate
                            //se ci sono la cancellazione torna ad essere logica
                            var oldsottotitolo = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.ID_SOTTOTITOLO == st[0].ID_SOTTOTITOLO).First();
                            dbcontext.COMUNICAZIONI_SOTTOTITOLI.Remove(oldsottotitolo);
                            dbcontext.COMUNICAZIONI_TITOLI.Remove(deleted);
                            int rows = dbcontext.SaveChanges();
                        }
                        if (st.Count == 0)
                        {
                            dbcontext.COMUNICAZIONI_TITOLI.Remove(deleted);
                            int row = dbcontext.SaveChanges();
                            if (row != 1)
                            {
                                transaction.Rollback();
                                //TASK: Allineamento log - Ciro
                                ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                                    "DAL_TIT_009", string.Empty,
                                    string.Empty, null);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);

                                log.Error(mEx);
                                throw mEx;
                            }
                            //throw new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA, "DAL_TIT_009", "", "", "", "", "", null);
                        }
                    }
                    /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
                    catch (Exception ex)
                    {
                        //TASK: Allineamento log - Ciro
                        transaction.Rollback();
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
                        //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
                    }
                }
            }
        }

        public void DeleteLogic(decimal id)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var deleted = dbcontext.COMUNICAZIONI_TITOLI.Where(x => x.ID_TITOLO == id).First();
                    deleted.ACTIVE = 0;
                    int rowAff = dbcontext.SaveChanges();
                    if (rowAff != 1)
                    {
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                            "DAL_TIT_009", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        log.Error(mEx);
                        throw mEx;
                    }
                    //throw new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA, "DAL_TIT_009", "", "", "", "", "", null);
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
                catch (InvalidOperationException ex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA,
                        "DAL_UNIQUE_CODE", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    if (System.Threading.Thread.CurrentContext.ContextID != null)
                        err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
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
        // ~TitoloSQLDb() {
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

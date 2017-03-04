#region "using"
using System;

using log4net;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using System.Configuration;

#endregion

namespace SendMailApp
{
    public class Program
    {
        //const string mutexId1 = "Global\\{e61867d8-8572-4057-a890-9b1007ad4b12}";
        private static readonly string mutexIdKey = ConfigurationManager.AppSettings.Get("MutexName");
        static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        static string APP_CODE;
        private static readonly string LOG_ERR = "PRG_";

        [STAThread]
        static void Main(string[] args)
        {
            ApplicationCodeConfigSection configSection =
                (ApplicationCodeConfigSection)ConfigurationManager.GetSection("ApplicationCode");
            if (configSection != null)
                APP_CODE = configSection.AppCode;

            string mail = null;
            int maxNrMail = 0;
            string operazione = null;
            var p = new NDesk.Options.OptionSet()
            {
                { "m=|mail=", "mail usata", v => mail = v },
                { "n:|max:", "numero massimo di invii", (int v) => maxNrMail = v },
                { "o:|ope:", "operazione", v => operazione = v.ToUpper() }
            };
            try
            {
                p.Parse(args);
            }
            catch (NDesk.Options.OptionException e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Errore nei parametri in ingresso. Mail: " + mail.ToString(),
                        LOG_ERR + "001",
                        "Main(string[] args)",
                        string.Empty,
                        e);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(err);
                }
                //ManagedException mEx = new ManagedException("Errore nei parametri in ingresso",
                //    LOG_ERR + "001", "Main(string[] args)", null, e);
                //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
                //_log.Error(err);
            }
            if (maxNrMail == 0)
            {
                maxNrMail = int.Parse(ConfigurationManager.AppSettings.Get("MaxNumeroMailsPerInvio"));
            }
            if (String.IsNullOrEmpty(mail)) return;

            string mutexId = String.Format("Global\\{0}_{1}", mutexIdKey ?? "{e61867d8-8572-4057-a890-9b1007ad4b12}", mail);
            //controllo univocità del batch
            using (var mutex = new Mutex(false, mutexId))
            {
                var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    MutexRights.FullControl, AccessControlType.Allow);
                var securitySettings = new MutexSecurity();
                securitySettings.AddAccessRule(allowEveryoneRule);
                mutex.SetAccessControl(securitySettings);

                bool hasHandle = false;
                try
                {
                    try
                    {
                        hasHandle = mutex.WaitOne(5000, false);
                        if (hasHandle == false) return;
                    }
                    catch (AbandonedMutexException e)
                    {
                        if (!e.GetType().Equals(typeof(ManagedException)))
                        {
                            //TASK: Allineamento log - Ciro
                            ManagedException mEx = new ManagedException("Oggetto mutex non rilasciato dall'applicazione precedente. Mail: " + mail.ToString(),
                                "PRG_002",
                                "Main(string[] args)",
                                string.Empty,
                                e);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            _log.Error(err);
                        }
                        //ManagedException mEx = new ManagedException("Oggetto mutex non rilasciato dall'applicazione precedente",
                        //    "PRG_001", "", "", e);
                        //ErrorLog err = new ErrorLog(APP_CODE, mEx);
                        //_log.Error(err);
                    }

                    string tipoOp = operazione ?? ConfigurationManager.AppSettings.Get("OperationType").ToUpper();
                    switch (tipoOp)
                    {
                        case "SEND":
                           // SenderMail.Send(mail, maxNrMail);
                            break;
                        case "RECEIVE":
                            ReceiverMail.Receive(mail);
                            break;
                        case "ALL":
                            ReceiverMail.Receive(mail);
                          //  SenderMail.Send(mail, maxNrMail);
                            break;
                        default:
                            throw new ArgumentException("Operazione non gestita");
                    }

                }
                catch (Exception ex)
                {
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException("Errore nell'applicazione di invio. Utente: " + mail.ToString(),
                            "PRG_003",
                            string.Empty,
                            string.Empty,
                            ex.InnerException);
                        string inner = (ex.InnerException!=null)?ex.InnerException.Message:"vuota.";
                        mEx.addEnanchedInfosTag("INNER", inner);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(err);
                    }
                    //ManagedException mEx = new ManagedException("Errore nell'applicazione di invio",
                    //        "PRG_002", "", ex.StackTrace + " " + ex.InnerException, ex);
                    //ErrorLog err = new ErrorLog(APP_CODE, mEx);
                    //_log.Error(err);
                }
                finally
                {
                    if (hasHandle)
                        mutex.ReleaseMutex();
                    LogManager.Shutdown();
                }
            }
        }
    }
}

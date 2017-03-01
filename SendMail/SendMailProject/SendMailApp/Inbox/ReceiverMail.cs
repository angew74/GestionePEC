using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using ActiveUp.Net.Mail.UnisysExt;
using Com.Unisys.Logging;
using Com.Unisys.Logging.Errors;
using SendMail.Business.MailFacades;
using SendMail.Business.Contracts;
using SendMail.Locator;
using ActiveUp.Net.Common.UnisysExt;
using ActiveUp.Net.Mail;
using SendMailApp.extension;
using System.Configuration;
using Com.Unisys.Logging.CrabMail;
using System.Threading;


namespace SendMailApp
{
    public class ReceiverMail
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SenderMail));
        private static String APP_CODE;
        private static string LOG_CODE = "SEND_MAIL_";
        private static int prog = 0;

        public static void Receive(string mail)
        {
            ApplicationCodeConfigSection configSection =
                (ApplicationCodeConfigSection)ConfigurationManager.GetSection("ApplicationCode");
            if (configSection != null)
                APP_CODE = configSection.AppCode;

            IList<MailUser> listUsers = null;
            try
            {
                //listUsers = ServiceLocator.GetServiceFactory().MailAccountService.GetAllManaged();
                List<string> mails = new List<string>();
                mails.Add(mail);
                listUsers = ServiceLocator.GetServiceFactory().MailAccountService.GetUsersByMails(mails);
                listUsers = listUsers.Where(x => x.FlgManaged != 0).ToList();
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Errore nel caricamento degli utenti di posta dettagli: " + ex.Message,
                                "RMA_001",
                                string.Empty,
                                string.Empty,
                                ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(err); 
                }

                //ManagedException mEx = new ManagedException("Errore nel caricamento degli utenti di posta dettagli: " + ex.Message, "RMA_001", "", "", ex);
                //mEx.EnanchedInfos = ex.StackTrace;
                //ErrorLog err = new ErrorLog(APP_CODE, mEx);
                //_log.Error(err);
                //_log.Error(ex.Message);
                //if (ex.InnerException != null)
                //{
                //    _log.Error(ex.InnerException.Message);
                //    _log.Error(ex.Source);
                //}
            }

            IMailServerService mailService = null;
            SendMail.Business.MailFacades.MailLocalService mailMessageService = null;
            foreach (MailUser user in listUsers)
            {
                _log.Info(new CrabMailLogInfo(APP_CODE, "", user.DisplayName,
                    user.EmailAddress, (string)null));
                try
                {
                    mailService = MailServerService.Instance.GetInstance(user);
                    mailMessageService = MailLocalService.Instance;
                }
                catch (Exception e)
                {
                    //TASK: Allineamento log - Ciro
                    if (e.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException("Errore nel caricamento degli utenti di posta dettagli: " + e.Message,
                            "RMA_002",
                            string.Empty,
                            string.Empty,
                            e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(err);
                    }
                    //ManagedException mEx = new ManagedException(String.Format("Account {0} non valido", user.EmailAddress), "RMA_002", "", "", e);
                    //ErrorLog err = new ErrorLog(APP_CODE, mEx);
                    //_log.Error(err);
                    continue;
                }
                user.Validated = true;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Account: " + user.EmailAddress);
                Console.ResetColor();
                int retries = 0;
                do
                {
                    mailService.IncomingConnect();
                    if (mailService.IsIncomingConnected())
                    {
                        break;
                    }
                    ++retries;
                }
                while (retries <= 5);

                if (!mailService.IsIncomingConnected())
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Impossibile connettersi al server remoto",
                        "RMA_022",
                        string.Empty,
                        string.Empty,
                        null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = string.Empty;
                    err.objectID = mail.ToString() + " - " + System.DateTime.Now.Date + "_" + System.DateTime.Now.Ticks;
                    err.userID = user.UserId.ToString();
                    _log.Error(err);
                    //_log.Error(new ErrorLogInfo(APP_CODE, "RMA_002", "Impossibile connettersi al server remoto.",
                    //    user.EmailAddress, null, null, null, null, null, null,
                    //    null, null, null));
                    continue;
                }
                ///pensiamo a qualcosa di più decente che così non si può vedere
                System.Threading.Thread.Sleep(500);
                List<MessageUniqueId> listUIds = mailService.RetrieveUIds();
                if (listUIds == null || listUIds.Count == 0)
                {
                    if (mailService.IsIncomingConnected())
                        Disconnect(mailService);
                    _log.Info(new CrabMailLogInfo(APP_CODE, "", user.DisplayName,
                        user.EmailAddress, "Nessun messaggio da scaricare"));
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Non vi sono messaggi da scaricare");
                    Console.ResetColor();
                    continue;
                }

                listUIds = listUIds.OrderBy(x => x.Ordinal).ToList();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(String.Format("Vi sono {0} messaggi da scaricare", listUIds.Count()));
                Console.ResetColor();
                List<MailStatusServer> stSer = new List<MailStatusServer>();
                stSer.Add(MailStatusServer.PRESENTE);
                stSer.Add(MailStatusServer.DA_NON_CANCELLARE);
                Dictionary<MailStatusServer, List<string>> savedUidsAll = (Dictionary<MailStatusServer, List<string>>)mailMessageService.GetAllUIDsByAccount(user.EmailAddress, stSer);
                List<string> savedUids = null;
                if (savedUidsAll != null && savedUidsAll.Count > 0)
                {
                    savedUids = savedUidsAll.SelectMany(x => x.Value).ToList();
                }
                else { savedUids = new List<string>(); }
                //cancello dal server
                bool toCancel = false;
                string cancelKey = System.Configuration.ConfigurationManager.AppSettings.Get("DeleteFromServer");
                bool.TryParse(cancelKey, out toCancel);
                int idx = 0;
                //se abilitato alla cancellazione
                if (user.FlgManaged == 2 && toCancel)
                {
                    //se ci sono email da cencellare
                    if (savedUidsAll != null && savedUidsAll.ContainsKey(MailStatusServer.PRESENTE) && savedUidsAll[MailStatusServer.PRESENTE].Count != 0)
                    {
                        try
                        {
                            //seleziona gli uid da cancellare                           
                            var mailToCancel = listUIds.Where(x => 
                                savedUidsAll[MailStatusServer.PRESENTE].Contains(x.UId.Replace(',','§')));
                            foreach (var uid in mailToCancel)
                            {
                                //cancella dal server
                                mailService.DeleteMessageFromServer(uid.UId);
                                Disconnect(mailService);
                                Connect(mailService);
                                mailMessageService.UpdateMessageServerStatus(user.EmailAddress, uid.UId, MailStatusServer.CANCELLATA);
                                idx = mailToCancel.IndexOf(uid) + 1;
                                //disconnect ogni 10 mail per committare sul server
                                if ((idx % 10) == 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Cancellati " + idx + " messaggi");
                                    Console.ResetColor();
                                    if (Disconnect(mailService))
                                    {
                                        if (!Connect(mailService))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            _log.Info(new CrabMailLogInfo(APP_CODE, "", user.DisplayName,
                                user.EmailAddress, "Cancellati " + idx + " messaggi"));
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cancellati " + idx + " messaggi");
                            Console.ResetColor();
                            if (Disconnect(mailService))
                            {
                                if (!Connect(mailService))
                                {
                                    continue;
                                }
                            }
                            //aggiorna gli uid dal server di posta
                            listUIds = mailService.RetrieveUIds();
                            if (listUIds == null || listUIds.Count == 0)
                            {
                                if (mailService.IsIncomingConnected())
                                    Disconnect(mailService);
                                _log.Info(new CrabMailLogInfo(APP_CODE, "", user.DisplayName,
                                    user.EmailAddress, "Nessun messaggio da scaricare"));
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Non vi sono messaggi da scaricare");
                                Console.ResetColor();
                                continue;
                            }
                            //esclude quelli da già in banca dati, se vi sono
                            var lUids = listUIds.Where(x => !savedUidsAll[MailStatusServer.PRESENTE].Contains(x.UId));
                            if (lUids.Count() == 0)
                                continue;
                            listUIds = lUids.OrderBy(x => x.Ordinal).ToList();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(String.Format("Vi sono {0} messaggi da scaricare", listUIds.Count()));
                            Console.ResetColor();
                            _log.Info(new CrabMailLogInfo(APP_CODE, "", user.DisplayName,
                                user.EmailAddress, "Vi sono " + listUIds.Count + " messaggi da scaricare"));
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                idx = 0;
                foreach (MessageUniqueId uId in listUIds)
                {
                    if (!savedUids.Contains(uId.UId.Replace(',','§')))
                    {
                    _log.Debug("WORKING UID:" + uId.UId);
                    _log.Debug("WORKING ORDINAL:" + uId.Ordinal);
                    List<MailHeader> ml = ((MailServerService)mailService).Pop3.RetrieveHeaders(uId.Ordinal, 1);
                    if (ml == null || ml.Count == 0)
                    {
                        ml = new List<MailHeader>();
                        ml.Add(new MailHeader { UniqueId = uId.UId });
                    }
                    _log.Debug(string.Format("WORKING HEADER: FROM - {0} ; SUBJECT - {1} ; UID - {2}", ml[0].From, ml[0].Subject, ml[0].UniqueId));
                    //maxSize da configurazione
                    int maxSize = 0;
                    string confMaxMailSize = ConfigurationManager.AppSettings.Get("MaxMailSizeInMB");
                    if (!String.IsNullOrEmpty(confMaxMailSize))
                    {
                        int.TryParse(confMaxMailSize, out maxSize);
                        // limite alzato a 8 mega
                        //     maxSize *= (int)Math.Pow(1024, 2);
                        maxSize *= (int)Math.Pow(1024, 8);
                    }
                    if (maxSize > 0)//se è definito un maxSize positivo
                    {
                        //dimensione del messaggio in byte
                        int size = mailService.GetMessageSize(uId.UId);
                        if (size > maxSize) // 4MB
                        {
                                //TASK: Allineamento log - Ciro
                                ManagedException mEx = new ManagedException(string.Format("Messaggio: {0} - Mail superiore a {1}MB! Controllare sul server", uId.UId, ConfigurationManager.AppSettings.Get("MaxMailSizeInMB")),
                                    "RMA_031",
                                    string.Empty,
                                    string.Empty,
                                    null);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                _log.Error(err);
                                //_log.Error(new ErrorLogInfo(APP_CODE, "RMA_003",
                                //    string.Format("Messaggio: {0} - Mail superiore a {1}MB! Controllare sul server",
                                //    uId.UId,
                                //    ConfigurationManager.AppSettings.Get("MaxMailSizeInMB")),
                                //    user.EmailAddress, null, null, null, null,
                                //    null, null, null, null, null));
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("WORKING HEADER: FROM - {0} ; SUBJECT - {1} ; UID - {2} ; SIZE - {3} ", ml[0].From, ml[0].Subject, ml[0].UniqueId, size);
                            Console.ResetColor();
                            continue;
                        }
                    }

                    ActiveUp.Net.Mail.Message m = mailService.getMessage(uId.UId, false);

                    if (m == null) //messaggio non trovato dall'UID!!!!
                    {
                        byte[] msx;
                        try
                        {
                            //provo a prendere il messaggio per progressivo
                            if (!mailService.IsIncomingConnected()) Connect(mailService);
                            msx = ((MailServerService)mailService).Pop3.Pop3Client.RetrieveMessage(uId.Ordinal);
                            if (mailService.IsIncomingConnected()) mailService.IncomingDisconnect();
                        }
                        catch (Exception ex)
                        {
                            _log.Info(new CrabMailLogInfo(APP_CODE, "", user.DisplayName, user.EmailAddress,
                                "Messaggio " + uId.UId + " non trovato"));
                            msx = null;
                        }
                        if (msx == null || msx.Length == 0) continue;
                        ActiveUp.Net.Mail.Message msg = new Message();
                        try
                        {
                            //provo il parser dal binary content
                            //Parser.BodyParsed += new Parser.OnBodyParsedEvent(Parser_BodyParsed);
                            //Parser.ErrorParsing += new Parser.OnErrorParsingEvent(Parser_ErrorParsing);
                            msg = Parser.ParseMessage(msx);
                            msg.Uid = uId.UId;
                        }
                        catch (Exception ex)
                        {
                            //costruisco il messaggio a mano
                            msg.OriginalData = msx;
                            msg.Uid = uId.UId;
                            msg.From = new Address();
                            msg.From.Email = ml[0].From ?? "UNDEFINED";
                            if (!String.IsNullOrEmpty(ml[0].To))
                            {
                                msg.To = new AddressCollection();
                                string[] to_emails = ml[0].To.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                msg.To.AddRange(to_emails.Select(to => new Address(to.Trim())));
                            }
                            msg.Subject = ml[0].Subject;
                            msg.Date = ml[0].Date;

                            if (ex.GetType() != typeof(ManagedException))
                            {
                                ManagedException mEx = new ManagedException("Messaggio costruito a mano: " + ex.Message,
                                    "ERR_H100",
                                    string.Empty,
                                    string.Empty,
                                    ex);
                                ErrorLogInfo er = new ErrorLogInfo(mEx);
                                _log.Error(er);
                            }
                        }
                        //assegno il messaggio al messaggio in lavorazione
                        m = msg;
                    }

                    try
                    {
                        if (savedUidsAll != null && savedUidsAll.ContainsKey(MailStatusServer.DA_NON_CANCELLARE) && savedUidsAll[MailStatusServer.DA_NON_CANCELLARE].Contains(m.Uid))
                        {
                            mailMessageService.Update(user, m);
                        }
                        else
                        {
                            
                            mailMessageService.Insert(user, m);
                        }

                        if (!String.IsNullOrEmpty(m.HeaderFields["X-Ricevuta"]))
                        {
                            string ric = m.HeaderFields["X-Ricevuta"].ToLower();
                            MailStatus newSt = MailStatus.UNKNOWN;

                            switch (ric)
                            {
                                case "accettazione":
                                    newSt = MailStatus.ACCETTAZIONE;
                                    break;
                                case "non-accettazione":
                                    newSt = MailStatus.NON_ACCETTAZIONE;
                                    break;
                                case "presa-in-carico":
                                    break;
                                case "avvenuta-consegna":
                                    newSt = MailStatus.AVVENUTA_CONSEGNA;
                                    break;
                                case "posta-certificata":
                                    break;
                                case "errore-consegna":
                                    newSt = MailStatus.ERRORE_CONSEGNA;
                                    break;
                                case "preavviso-errore-consegna":
                                    newSt = MailStatus.ERRORE_CONSEGNA;
                                    break;
                                case "rilevazione-virus":
                                    newSt = MailStatus.ERROR;
                                    break;
                            }

                            if (newSt != MailStatus.UNKNOWN)
                            {
                                string idOldString = m.HeaderFields["X-Riferimento-Message-ID"];
                                if (idOldString.StartsWith("<"))
                                    idOldString = idOldString.Substring(1);
                                if (idOldString.EndsWith(">"))
                                    idOldString = idOldString.Substring(0, idOldString.Length - 1);
                                string[] idOldStr = idOldString.Split('.');
                                Int64 idOld = -1;
                                if (idOldStr.Length > 0 && Int64.TryParse(idOldStr[0], out idOld))
                                {
                                    IComunicazioniService comS = ServiceLocator.GetServiceFactory().ComunicazioniService;
                                    SendMail.Model.ComunicazioniMapping.Comunicazioni com = comS.LoadComunicazioneByIdMail(idOld);
                                    if (com != null)
                                    {
                                        SendMail.Model.ComunicazioniMapping.ComFlusso flusso = com.ComFlussi[SendMail.Model.TipoCanale.MAIL].Last();

                                        //se si trova in stato di errore non aggiorno
                                        if (flusso.StatoComunicazioneNew == MailStatus.ERROR ||
                                            flusso.StatoComunicazioneNew == MailStatus.CANCELLED ||
                                            flusso.StatoComunicazioneNew == MailStatus.NON_ACCETTAZIONE ||
                                            flusso.StatoComunicazioneNew == MailStatus.ERRORE_CONSEGNA)
                                        {
                                            idx = listUIds.ToList().IndexOf(uId);
                                            if ((idx != 0) && ((idx % 50) == 0))
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("Inseriti " + idx + " messaggi");
                                                Console.ResetColor();
                                                _log.Info(new CrabMailLogInfo(APP_CODE,
                                                    "", user.DisplayName, user.EmailAddress,
                                                    "Inseriti " + idx + " messaggi"));
                                            }
                                            continue;
                                        }

                                        SendMail.Model.ComunicazioniMapping.ComFlusso newFl = new SendMail.Model.ComunicazioniMapping.ComFlusso();
                                        newFl.Canale = SendMail.Model.TipoCanale.MAIL;
                                        newFl.DataOperazione = m.Date;
                                        newFl.RefIdComunicazione = flusso.RefIdComunicazione;
                                        newFl.StatoComunicazioneNew = newSt;
                                        newFl.StatoComunicazioneOld = flusso.StatoComunicazioneNew;
                                        newFl.UtenteOperazione = "SYSTEM";
                                        com.ComFlussi[SendMail.Model.TipoCanale.MAIL].Add(newFl);
                                        comS.UpdateFlussoComunicazione(SendMail.Model.TipoCanale.MAIL, com);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Message msg = mailMessageService.GetById(idOld);
                                            if (msg != null)
                                            {
                                                List<string> uidMails = new List<string>();
                                                uidMails.Add(msg.Uid);
                                                    mailMessageService.UpdateMailStatus(user.EmailAddress, uidMails, newSt, null, null, "I");
                                            }
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (ManagedException mEx)
                    {
                        if (mEx.CodiceEccezione.Equals("WRN_INS_ML_001"))
                        {
                            mailService.DeleteMessageFromServer(uId.UId);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TASK: Allineamento log - Ciro
                        if (ex.GetType() != typeof(ManagedException))
                        {
                            ManagedException mEx = new ManagedException(String.Format("Errore nel salvataggio della mail uid: {0} from: {1} subject:{2} dell'account {3}", m.Uid, m.From.Email, m.Subject, user.EmailAddress),
                                "RMA_003",
                                string.Empty,
                                string.Empty,
                                ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            _log.Error(err);
                        }
                        //ManagedException mEx = new ManagedException(
                        //    String.Format("Errore nel salvataggio della mail uid: {0} from: {1} subject:{2} dell'account {3}", m.Uid, m.From.Email, m.Subject, user.EmailAddress),
                        //    "RCV_MAIL_003", "", "", ex);
                        //ErrorLog err = new ErrorLog(APP_CODE, mEx);
                        //_log.Error(err);
                    }

                    idx = listUIds.IndexOf(uId) + 1;
                    if ((idx % 50) == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Inseriti " + idx + " messaggi");
                        Console.ResetColor();
                        _log.Info(new CrabMailLogInfo(APP_CODE, "", user.DisplayName,
                            user.EmailAddress, "Inseriti " + idx + " messaggi"));
                    }
                }
            }
                if (mailService.IsIncomingConnected())
                    Disconnect(mailService);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Inseriti " + listUIds.Count + " messaggi");
                Console.ResetColor();
                _log.Info(new CrabMailLogInfo(APP_CODE, "", user.DisplayName,
                    user.EmailAddress, "Inseriti " + listUIds.Count + " messaggi"));
            }
        }

        static void Parser_ErrorParsing(object sender, Exception ex)
        {
            //TASK: Allineamento log - Ciro
            ManagedException mEx = new ManagedException(string.Format("Errore: {0}",ex.Message),
                "RMA_004",
                string.Empty,
                string.Empty,
                ex);
            ErrorLogInfo err = new ErrorLogInfo(mEx);
            _log.Error(err);
            //_log.Error(new ErrorLogInfo(APP_CODE, "RMA_004",
            //    "Messaggio: " + ex.Message + " - Stack: " + ex.StackTrace,
            //    null, null, null, null, null, null, null, null, null, null));
        }

        static void Parser_BodyParsed(object sender, Message message)
        {
            _log.Info(new CrabMailLogInfo(APP_CODE, "", message.To[0].Name,
                message.To[0].Email, message.Subject));
        }

        public static bool Connect(IMailServerService mailService)
        {
            int retries = 0;
            do
            {
                mailService.IncomingConnect();
                if (mailService.IsIncomingConnected())
                {
                    break;
                }
                ++retries;
            }
            while (retries <= 5);

            if (!mailService.IsIncomingConnected())
            {
                return false;
            }
            else return true;

        }

        public static bool Disconnect(IMailServerService mailService)
        {
            mailService.IncomingDisconnect();

            if (mailService.IsIncomingConnected())
            {
                return false;
            }
            else
                return true;
        }
    }
}

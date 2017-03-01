using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Mail.Facades;

using System.Linq;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.Locator;
using Com.Delta.Logging.Errors;
using log4net;



namespace SendMail.Business.MailFacades
{

    public class MailServerFacade : BaseFacade, IMailServerFacade
    {
        #region Singleton

        private static readonly ILog _log = LogManager.GetLogger("MailServerFacade");
        private static IDictionary<string, MailServerFacade> facades = new Dictionary<string, MailServerFacade>();
        private static MailServerFacade getFacade(string name)
        {
            if (facades.ContainsKey(name)) return facades[name];
            else return null;
        }

        private MailServerFacade(MailUser acs)
        {
            this._accSettings = acs;
            if (acs.IncomingProtocol.Equals("POP3") && !string.IsNullOrEmpty(acs.IncomingServer))
                this.popController = new Pop3Controller();
            else if (acs.IncomingProtocol.Equals("IMAP") && !string.IsNullOrEmpty(acs.IncomingServer))
                this.imapController = new ImapController();
            if (!string.IsNullOrEmpty(acs.OutgoingServer))
                this.smtpController = new SmtpController();
            if (!acs.IsManaged)
            {
                this.IncomingConnect();
                if (!this.popController.Connect(_accSettings))
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = null;
                    if (_accSettings != null)
                    {
                        mEx = new ManagedException(String.Format("Mail: non posso connettermi a {0} usando il protocollo {1}", _accSettings.IncomingServer, _accSettings.IncomingProtocol),
                        "ERR_MAIL_0001", 
                        string.Empty,
                        string.Empty, 
                        null);
                    }
                    else
                    {
                        mEx = new ManagedException("Mail: connessione non riuscita _accSettings è null.",
                        "ERR_MAIL_0010", 
                        string.Empty,
                        string.Empty, 
                        null);
                    }

                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    //err.loggingAppCode = "SEND_MAIL";
                    //if (System.Threading.Thread.CurrentContext.ContextID != null)
                    //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                    _log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(String.Format("Mail: non posso connettermi a {0} usando il protocollo {1}", _accSettings.IncomingServer, _accSettings.IncomingProtocol)
                    //                          , "ERR_MAIL_0001", "MailServerFacade.cs", "MailServerFacade", "popController.Connect", "", "", null
                    //                          );
                }
                this.IncomingDisconnect();
            }
        }

        public static MailServerFacade GetInstance(MailUser acs)
        {
            string key = acs.EmailAddress + acs.IncomingProtocol + acs.IncomingServer;
            if (getFacade(key) == null)
                facades.Add(key, new MailServerFacade(acs));
            return getFacade(key);
        }

        public static void DropInstance(MailUser acs)
        {

            string key = acs.EmailAddress + acs.IncomingProtocol + acs.IncomingServer;
            if (getFacade(key) == null) return;
            else
            {
                facades[key].IncomingDisconnect();
                facades.Remove(key);
            }
        }

        #endregion


        #region mail GET/SEND

        public MailUser AccountInfoCurrentGet()
        {
            if (_accSettings != null)
                return this._accSettings;
            else
                return null;
        }


        ////recupera gli header dei messagi per popolare la inbox
        //public List<MailHeader> Retrieves(bool refresh)
        //{
        //    if (refresh || _ListHeaders == null)
        //    {
        //        IncomingConnect();
        //        switch (_accSettings.IncomingProtocol)
        //        {
        //            case "POP3":
        //             //   this._ListHeaders = fillFromRawHeaders(this.popController.RetrieveHeaders());
        //                this._ListHeaders = fillFromRawHeaders(this.popController.RetrieveHeaders());
        //                this._LastRefresh = System.DateTime.Now;
        //                return this._ListHeaders;
        //            case "IMAP":
        //                this._ListHeaders = fillFromRawHeaders(this.imapController.RetrieveHeaders(MAILBOX));
        //                this._LastRefresh = System.DateTime.Now;
        //                return this._ListHeaders;
        //            default:
        //                return null;
        //        }
        //    }
        //    else return this._ListHeaders;
        //}

        public Message getMessageByOrdinal(int piOrdinal, bool doARefreshBefore)
        {
            return (getMessageByOrdinal(piOrdinal.ToString(), doARefreshBefore));
        }

        public Message getMessageByOrdinal(String psOrdinal, bool doARefreshBefore)
        {
            Message msgRetVal = null;
            int iOrdinal;


            if (!CanIReceive(true))
                return (msgRetVal);

            if (doARefreshBefore)
                if (MailHeaderLoad() < 0)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                        "ERR_MAIL_0002", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    //err.loggingAppCode = "SEND_MAIL";
                    //if (System.Threading.Thread.CurrentContext.ContextID != null)
                    //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                    _log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException("Non posso leggere la casella postale"
                    //                          , "ERR_MAIL_0002", "MailServerFacade.cs", "getMessageByOrdinal", "MailHeaderLoad", "", "", null
                    //                          );
                }

            if (!int.TryParse(psOrdinal, out iOrdinal))
                return (msgRetVal);

            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    return this.popController.getMessageByOrdinal(iOrdinal);
                case IncomingProtocols.IMAP:
                    return this.imapController.getMessageByOrdinal(iOrdinal);
                default:
                    return null;
            }
        }


        /// <summary>
        /// Returns the element the piIndex parameter points to.
        /// It transalte the passed index into an Ordinal value to pass to the Server.
        /// </summary>
        /// <param name="piIndex">Index into the aMailHeader table</param>
        /// <param name="doARefreshBefore">Before access the aMailHeader table, it must be reloaded.</param>
        /// <returns>A Message object</returns>
        public Message getMessageByIndex(int piIndex, bool doARefreshBefore)
        {
            Message msgRetVal = null;
            int iOrdinal;


            if (!CanIReceive(true))
                return (msgRetVal);

            if (doARefreshBefore)
                if (MailHeaderLoad() < 0)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                        "ERR_MAIL_0003", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    //err.loggingAppCode = "SEND_MAIL";
                    //if (System.Threading.Thread.CurrentContext.ContextID != null)
                    //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                    _log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException("Non posso leggere la casella postale"
                    //                          , "ERR_MAIL_0003", "MailServerFacade.cs", "getMessageByIndex", "MailHeaderLoad", "", "", null
                    //                          );
                }

            if ((piIndex < 1) || (piIndex >= aMailHeader.Length))
                return (msgRetVal);

            if (!int.TryParse(aMailHeader[piIndex].Index, out iOrdinal))
                return (msgRetVal);

            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    return this.popController.getMessageByOrdinal(iOrdinal);
                case IncomingProtocols.IMAP:
                    return this.imapController.getMessageByOrdinal(iOrdinal);
                default:
                    return null;
            }
        } // Message getMessageByIndex(...


        /// <summary>
        /// Retrieve a message by UId. modificata gestione folders
        /// </summary>
        /// <param name="pUId"></param>
        /// <param name="doARefreshBefore"></param>
        /// <returns></returns>
        public Message getMessage(string pUId, bool doARefreshBefore)
        {
            Message msgRetVal = null;
            int iOrdinal;

            if (this.AccSettings.IsManaged)
            {
                string mailFolder = string.Empty;
                switch (this.MailBoxName)
                {
                    case "inbox":
                        if ((this.AccSettings.FlgManaged == null) ||
                            (this.AccSettings.FlgManaged < 1))
                            return null;
                        mailFolder = "1";
                        break;
                    case "outbox":
                        mailFolder = "2";
                        break;
                }
                return
                    MailLocalService.Instance.GetById(pUId, this.AccSettings.EmailAddress, mailFolder);
            }
            else
            {
                if (!CanIReceive(true))
                    return (msgRetVal);

                if (doARefreshBefore)
                    if (MailHeaderLoad() < 0)
                    {
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                            "ERR_MAIL_0004", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(mEx);
                        throw mEx;
                        //throw new ManagedException("Non posso leggere la casella postale"
                        //                          , "ERR_MAIL_0004", "MailServerFacade.cs", "getMessage", "MailHeaderLoad", "", "", null
                        //                          );
                    }

                if ((iOrdinal = MailHeader_ArrayList_OrdinalGetByUid(pUId)) < 0)
                    return (msgRetVal);

                switch (InProtoGet())
                {
                    case IncomingProtocols.POP3:
                        msgRetVal = this.popController.getMessageByOrdinal(iOrdinal);
                        break;
                    case IncomingProtocols.IMAP:
                        msgRetVal = this.imapController.getMessageByOrdinal(iOrdinal);
                        break;
                    default:
                        return msgRetVal;
                }
            }

            // Just in case of lower error(s) 
            if (msgRetVal != null)
                if (msgRetVal.Uid != pUId)
                {
                    // Desperate action!
                    // Recursively I'm calling myself ... (be careful)
                    return (getMessage(pUId, true));
                }
            return (msgRetVal);
        } // Message getMessage(...



        public void ReConnect()
        {
            this.IncomingDisconnect();
            this.IncomingConnect();
        }

        public void IncomingConnect()
        {
            if (_accSettings != null)
            {
                if (InProtoGet() == IncomingProtocols.POP3)
                {
                    if (popController.Pop3Client != null && popController.Pop3Client.IsConnected)
                        this.IncomingDisconnect();
                    this.popController.Connect(_accSettings);
                    if (CanIReceive(false)) // CANNOT BE true: IsMailServerReady calls this routine! The risk is a StackOverflow exception!
                    {
                        if (MailHeaderLoad() < 0)
                        {
                            //TASK: Allineamento log - Ciro
                            ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                                "ERR_MAIL_0005", string.Empty,
                                string.Empty, null);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            //err.loggingAppCode = "SEND_MAIL";
                            //if (System.Threading.Thread.CurrentContext.ContextID != null)
                            //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                            _log.Error(mEx);
                            throw mEx;
                        }
                            //throw new ManagedException("Non posso leggere la casella postale"
                            //                          , "ERR_MAIL_0005", "MailServerFacade.cs", "IncomingConnect", "MailHeaderLoad", "", "", null
                            //                          );
                    }
                }
            }
            else
            {
                if (imapController.Imap4Client == null)
                {
                    if (!imapController.Imap4Client.IsConnected)
                        this.IncomingDisconnect();
                    this.imapController.Connect(_accSettings);
                }
                if (CanIReceive(false)) // CANNOT BE true: IsMailServerReady calls this routine! The risk is a StackOverflow exception!
                {
                    if (MailHeaderLoad() < 0)
                    {
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                            "ERR_MAIL_0006", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        //err.loggingAppCode = "SEND_MAIL";
                        //if (System.Threading.Thread.CurrentContext.ContextID != null)
                        //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                        _log.Error(mEx);
                        throw mEx;
                    }
                        //throw new ManagedException("Non posso leggere la casella postale"
                        //                          , "ERR_MAIL_0006", "MailServerFacade.cs", "IncomingConnect", "MailHeaderLoad", "", "", null
                        //                          );
                }
            }
        }


        public bool IsIncomingConnected()
        {


            if (_accSettings == null)
                return (false);

            if (InProtoGet() == IncomingProtocols.POP3 && (this.popController.Pop3Client != null && this.popController.Pop3Client.IsConnected))
                return (true);
            else
                return (this.imapController.Imap4Client != null && this.imapController.Imap4Client.IsConnected);
        }


        /// <summary>
        ///Method to disconnect the incoming mail, Pop or Imap.
        /// </summary>
        public void IncomingDisconnect()
        {
            //AccountSettings.AccountInfo accountInfo = this.AccountInfoCurrentGet();


            //if (  accountInfo != null)
            //{
            //    if (accountInfo.IncomingServerProto == Constants.POP3 && (this.popController.Pop3Client != null && this.popController.Pop3Client.IsConnected))
            //    {
            //        this.popController.Disconnect();
            //    }
            //    else if(this.imapController.Imap4Client != null && this.imapController.Imap4Client.IsConnected)
            //    {
            //        this.imapController.Disconnect();
            //    }
            //}


            if (IsIncomingConnected())
                switch (InProtoGet())
                {
                    case IncomingProtocols.POP3:
                        this.popController.Disconnect();
                        break;
                    case IncomingProtocols.IMAP:
                        this.imapController.Disconnect();
                        break;
                    default:
                        break;
                }
        }

        #endregion


        public List<MailHeader> fillFromRawHeaders(IList<Header> rawData)
        {
            List<MailHeader> ltemp = new List<MailHeader>();
            foreach (Header header in rawData)
            {
                MailHeader mh = new MailHeader();
                mh.FillHeader(header.Subject, header.From.Name, header.From.Email, header.Date, header.Uid);
                mh.Index = header.LocalIndex.ToString();
                ltemp.Add(mh);
                //TODO:controllare cosa è serverIndex e se è meglio;
            }
            return ltemp;
        }

        #region --------------------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //


        public bool CanIReceive(bool pbReconnect)
        {

            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    if (!popController.IsMailServerReady())
                    {
                        if (pbReconnect)
                        {
                            ReConnect();
                            if (!popController.IsMailServerReady())
                                return (false);
                        }
                        else
                            return (false);
                    }
                    break;
                case IncomingProtocols.IMAP:
                    if (!imapController.CanIReceive())
                    {
                        if (pbReconnect)
                        {
                            ReConnect();
                            if (!imapController.CanIReceive())
                                return (false);
                        }
                        else
                            return (false);
                    }
                    break;
                default:
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Protocol not implemented yet: " + InProtoGet().ToString(),
                        "ERR_MAIL_0099", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    //err.loggingAppCode = "SEND_MAIL";
                    //if (System.Threading.Thread.CurrentContext.ContextID != null)
                    //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                    _log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException("Protocol not yet implemented: " + InProtoGet().ToString(), String.Empty, String.Empty, String.Empty, null);
                    break;
            }

            return (true);
        } // bool IsMailServerReady(...


        /// <summary>
        /// Account Types.
        /// </summary>
        public enum IncomingProtocols
        {
            POP3, IMAP
        }


        protected string _MailBoxName = "inbox";
        public string MailBoxName
        {
            get { return (_MailBoxName); }
            set { _MailBoxName = value; }
        }

        /// <summary>
        /// Check the existence of the touple ( Ordinal, Uid )
        /// </summary>
        /// <param name="psOrdinal"></param>
        /// <param name="psUid"></param>
        /// <returns></returns>
        public bool Ordinal_Uid_Check(String psOrdinal, String psUid)
        {
            int iOrdinal;


            if (int.TryParse(psOrdinal, out iOrdinal) == false)
                return (false);

            return (Ordinal_Uid_Check(iOrdinal, psUid));
        } // bool Ordinal_Uid_Check(...


        /// <summary>
        /// Check if the Message at the Ordinal position has the specified UniqueId.
        /// </summary>
        /// <param name="messageOrdinal">The index of the message</param>
        /// <param name="messageUid">The UID / ServerId / MessageId</param>
        /// <returns>true if the messageOrdinal-th has UID passed</returns>
        public bool Ordinal_Uid_Check(int messageOrdinal, String messageUid)
        {
            String retrievedUid = "";


            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    retrievedUid = this.popController.Pop3Client.GetUniqueId(messageOrdinal);
                    break;
                case IncomingProtocols.IMAP:
                    Mailbox inbox = this.imapController.Imap4Client.SelectMailbox(MailBoxName);
                    Fetch fetch = inbox.Fetch;
                    retrievedUid = fetch.Uid(messageOrdinal).ToString();
                    break;
                default:
                    retrievedUid = "";
                    break;
            }
            return (retrievedUid == messageUid);
        }  // bool Ordinal_Uid_Check(...


        /// <summary>
        /// Returns the server protocol used ( i.e.: configured ) to read mails.
        /// </summary>
        /// <returns>One out of the IncomingProtocols values</returns>
        public IncomingProtocols InProtoGet()
        {
            const IncomingProtocols KINPROTO_DEFAULT = IncomingProtocols.POP3;
            IncomingProtocols uatRetVal = KINPROTO_DEFAULT;


            if (_accSettings == null)
                return (uatRetVal);

            if (_accSettings.IncomingProtocol == IncomingProtocols.POP3.ToString())
                return (IncomingProtocols.POP3);
            if (_accSettings.IncomingProtocol == IncomingProtocols.IMAP.ToString())
                return (IncomingProtocols.IMAP);

            return (KINPROTO_DEFAULT);         //LR// It would be nice return 'unknown'
        } // IncomingProtocols InProtoGet()


        /// <summary>
        /// First phase: the complete list of the UIds is built.
        /// </summary>
        /// <returns></returns>
        public List<MessageUniqueId> RetrieveUIds()
        {
            List<Pop3Client.PopServerUniqueId> lstUId = null;
            List<MessageUniqueId> lstMUId = null;
            int iMailHeader;


            // Create the object
            if (triggerChanges == null)
                triggerChanges = new PostOfficeChangesTrigger(this);

            // Restart the container (BUT without requesting reloading (recursive comes here again!)
            if (MailHeaderSyncCheck(true, false) < 0)
                // Problems while connecting ...
                return (null);

            // Get the full UIds list from PostOffice
            do
            {
                switch (InProtoGet())
                {
                    case IncomingProtocols.POP3:
                        // Ask server
                        lstUId = this.popController.RetrieveUIds();
                        // Convert from one type to (an)other(s) ...
                        do
                        {
                            lstMUId = new List<MessageUniqueId>();
                            iMailHeader = 0;
                            foreach (Pop3Client.PopServerUniqueId PSUId in lstUId)
                            {
                                // ------------------------------------------------------------ //
                                iMailHeader++;
                                if (iMailHeader != PSUId.Index)
                                {
                                    // Unsynchronized lists (local vs. server)
                                    iMailHeader = iMailHeader;
                                }
                                MailHeader mh = new MailHeader();
                                mh.Index = PSUId.Index.ToString();
                                mh.UniqueId = PSUId.UniqueId;
                                mh.Date = DateTime.MinValue;    // This value flags that the rest is not loaded yet.
                                aMailHeader[iMailHeader] = mh;
                                // ------------------------------------------------------------ //
                                MessageUniqueId muid = new MessageUniqueId();
                                muid.Ordinal = PSUId.Index;
                                muid.UId = PSUId.UniqueId;
                                lstMUId.Add(muid);
                                // ------------------------------------------------------------ //
                            }
                            // The following line means: if it has changed in the meanwhile, please reload it!
                        } while (triggerChanges.StatisticsUpdate());
                        break;
                    case IncomingProtocols.IMAP:
                        // doesn't yet exist! lstUId                          = this.imapController.RetrieveUIds( Constants.MAILBOX );
                        // Convert from one type to another ...
                        //lstMUId                         = new List<MessageUniqueId>();
                        //foreach( Pop3Client.PopServerUniqueId Puid in lstUId )
                        //    {
                        //    MessageUniqueId    muid         = new MessageUniqueId();
                        //    muid.Ordinal                    = Puid.Index;
                        //    muid.UId                        = Puid.UniqueId;
                        //    lstMUId.Add( muid );
                        //    }

                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException("Imap not supported at all!",
                            "ERR_MAIL_0098", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        //err.loggingAppCode = "SEND_MAIL";
                        //if (System.Threading.Thread.CurrentContext.ContextID != null)
                        //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                        _log.Error(mEx);
                        throw mEx;

                        //throw new ManagedException("Imap not supported at all!", String.Empty, String.Empty, String.Empty, null);
                        break;
                    default:
                        return null;
                }
            } while ((popController.Pop3Client.IsConnected) && (triggerChanges.StatisticsUpdate() == true));

            return (lstMUId);
        } // List<MailUniqueId> RetrieveUIds()


        // No more used. Old version.
        //    public  List<MessageUniqueId>           listUIds                = null;

        // This will hold UIds List and the MailHeader(s).
        // This is the real work copy.
        public MailHeader[] aMailHeader = null;
        // After have fully loaded the aMailHeader, this will hold an exact copy.
        // In case of reloading, this array if checked BEFORE request data to PO.
        // This will hopefully speed up the overall reload process.
        public MailHeader[] aMailHeader_CurrCopy = null;


        public class PostOfficeChangesTrigger
        {
            private MailServerFacade facade;

            private int count;
            private int size;
            private DateTime dtLast;
            private bool bChanged;


            public PostOfficeChangesTrigger(MailServerFacade pfacade)
            {
                count = -1;
                size = -1;
                facade = pfacade;
                dtLast = DateTime.MinValue;
                bChanged = true;                // default to "Must read"
            }


            public void Start()
            {

                StatisticsUpdate();
                bChanged = false;               // It starts now.
                // So, after have got the necessary info, reset the flag,
                //   as 'no changes have been made'.
            }

            public bool StatisticsUpdate()
            {
                // Assume the worst case
                bChanged = true;
                // May I get the requested update?
                if ((facade == null)
                    || (facade.popController == null)
                    || (facade.popController.Pop3Client == null)
                   )
                    return (bChanged);
                if (facade.CanIReceive(true) == false)
                    return (bChanged);
                // get updated info
                facade.popController.Pop3Client.UpdateStats();
                // set the internal status
                bChanged = (count != facade.popController.MessageCountGet());
                bChanged |= (size != facade.popController.MessageSizeGet());
                // Update internal info
                count = facade.popController.MessageCountGet();
                size = facade.popController.MessageSizeGet();
                dtLast = DateTime.Now;

                return (bChanged);
            } // bool StatisticsUpdate()


            private TimeSpan tsTrigger = new TimeSpan(0, 0, 0, 0, 500);
            public bool IsSynchronized()
            {
                if (((TimeSpan)(DateTime.Now - dtLast)) < tsTrigger)
                    return (!bChanged);
                else
                    return (!StatisticsUpdate());
            } // bool IsSynchronized()


            // Not needed. Use Start() and IsSynchronized() instead...
            public bool HasChanged
            {
                get { return (bChanged); }
                set
                {
                    bChanged = value;
                    dtLast = DateTime.MinValue; // Forces, next time, to resynchronize!
                }
            }

        }

        // Try to get synchronized to the server 
        PostOfficeChangesTrigger triggerChanges;


        /// <summary>
        /// Check if either a MailHeader slot has already been loaded or not.
        /// WARNING: static checks! No sync against PostOffice is here done!
        /// </summary>
        /// <param name="pmh">The item to check</param>
        /// <returns>It answers to the question. So 'true' means NOT loaded and conversely 'false' is returned if the item is already in.</returns>
        public bool IsMailHeaderNotLoaded(MailHeader pmh)
        {
            bool bret = true; // worst

            if (pmh == null)
                return (bret);
            if (pmh.Date == DateTime.MinValue)
                return (bret);
            if (String.IsNullOrEmpty(pmh.From))
                return (bret);

            // PEC does leave it to null!
            //if ( String.IsNullOrEmpty( pmh.To ) )
            //    return (bret);

            // It is loaded.
            bret = false;
            return (bret);
        } // bool IsMailHeaderNotLoaded(...


        // Just to be sure ...
        // May be semplified ... done.
        public bool MailHeaderCopy_IsEqual(MailHeader pmh1, MailHeader pmh2)
        {

            if (pmh1.UniqueId != pmh2.UniqueId)
                return (false);
            //if ( pmh1.To != pmh2.To )
            //    return( false );
            //if ( pmh1.From != pmh2.From )
            //    return( false );
            //if ( pmh1.Date != pmh2.Date )
            //    return( false );

            return (true);
        } // bool MailHeaderCopy_IsEqual(...


        public int MailHeaderCopy_IndexOf(MailHeader pmh)
        {
            int iScan;


            for (iScan = 1; iScan < aMailHeader_CurrCopy.Length; iScan++)
                if (MailHeaderCopy_IsEqual(aMailHeader_CurrCopy[iScan], pmh))
                    break;

            if (iScan < aMailHeader_CurrCopy.Length)
                return (iScan);
            else
                return (-1);
        } // MailHeader MailHeaderCopy_IndexOf(...


        public void MailHeaderCopy_Recover()
        {
            int iScan;
            int iItem;


            if (aMailHeader_CurrCopy != null)
                // All items already still present and already previously loaded are now copied into the new array ...
                for (iScan = 1; iScan < aMailHeader.Length; iScan++)
                {
                    iItem = MailHeaderCopy_IndexOf(aMailHeader[iScan]);
                    if (iItem >= 0)
                        aMailHeader[iScan] = aMailHeader_CurrCopy[iItem];
                }

            // Make the copy exactly the same of the so arranged work one.
            if (aMailHeader != null)
                aMailHeader_CurrCopy = (MailHeader[])aMailHeader.Clone();
            else
                aMailHeader_CurrCopy = null;

        } // void MailHeaderCopy_Recover()


        /// <summary>
        /// Reload the UIds full list from the PostOffice.
        /// Recover the MailHeader items already loaded from the copy table.
        /// </summary>
        /// <returns>The actual number of MailHeader loaded, -1 if an error occured.</returns>
        public int MailHeaderLoad()
        {
            RetrieveUIds();
            MailHeaderCopy_Recover();

            if (aMailHeader == null)
                return (-1);
            else
                return (aMailHeader.Length);
        } // int MailHeaderLoad()


        /// <summary>
        /// Manage the local data structure holding MailHeader(s).
        /// </summary>
        /// <param name="bRestart">Re-creates the MailHeader data structure and leave it empty.</param>
        /// <param name="bReloadUIds">Forces the reload of all the values.</param>
        /// <returns>The count of slots-created/items-loaded. A value less than 0 means problems (not connected, ...).</returns>
        private int MailHeaderSyncCheck(bool bRestart, bool bReloadUIds)
        {
            int iCount;

            if (bRestart)
                aMailHeader = null;
            if (popController == null)
                return (-1);
            if (bReloadUIds)
            {
                // Reload all UIds
                iCount = MailHeaderLoad();
            }
            else
            {
                // Check the current array is smaller than it needs to be
                if (!CanIReceive(true))
                    return (-1);
                popController.Pop3Client.UpdateStats();
                //+1 because herein it is zero-based.
                // The MailHeaders, into the PO, start from 1.
                iCount = popController.MessageCountGet() + 1;
                if ((aMailHeader == null) || (aMailHeader.Length < iCount))
                    Array.Resize(ref aMailHeader, iCount);
            }
            return (iCount);
        } // int MailHeaderSyncCheck()

        private int iBase = 1;

        private bool FetchScanCondition(int piDir, int piIndex, int piLimit)
        {

            if (piDir > 0)
                return (piIndex <= piLimit);
            else
                return (piIndex >= piLimit);
        }


        /// <summary>
        /// Next time the MailHeader_ArrayList_Fetch is called, a complete restart ( UIds reload ) is performed.
        /// </summary>
        public void MailHeader_ReloadForce()
        {

            triggerChanges = null;
        } // void MailHeader_ReloadForce()


        private Boolean ReloadForce = true;


        /// <summary>
        /// Load a MailHeader segment
        /// </summary>
        /// <param name="piStart"></param>
        /// <param name="piCount"></param>
        /// <returns></returns>
        private MailHeader[] MailHeader_Fetch(int pidxStart, int piCount)
        {
            int idxStart;
            int idxStop;
            List<MailHeader> tmpListMailHeader = new List<MailHeader>();
            int i;
            int iMax;
            int iMin;
            int iDir;


            if (triggerChanges == null)
                triggerChanges = new PostOfficeChangesTrigger(this);

            if (ReloadForce || !triggerChanges.IsSynchronized())
                // Reloads all the UIds (but NOT MailHeaders)
                if (MailHeaderSyncCheck(true, true) < 0)
                    // Problems while connecting ...
                    return (null);

            iMax = aMailHeader.Length - 1;
            iMin = 1;
            if (false)
            {
                idxStart = pidxStart + iMin;
                idxStop = idxStart + piCount - 1;
                iDir = 1;
            }
            else
            {
                idxStart = aMailHeader.Length - pidxStart;
                idxStop = idxStart - piCount + 1;
                iDir = -1;
            }

            // Start is "Out of bound"? Zero included because the first item has index=1.
            if ((idxStart <= 0) || (idxStart >= aMailHeader.Length))
                return (aMailHeader);     // Unchanged!
            // Stop adjustment ...
            if ((idxStop <= 0) || (idxStop >= aMailHeader.Length))
                idxStop = (iDir > 0) ? iMax : iMin;

            // First I do check that requested elements has been already loaded ...
            for (i = idxStart; FetchScanCondition(iDir, i, idxStop); i += iDir)
            {
                // Is it loaded?
                if (IsMailHeaderNotLoaded(aMailHeader[i]))
                {
                    // No. I must load it now.
                    switch (InProtoGet())
                    {
                        case IncomingProtocols.POP3:
                            tmpListMailHeader = popController.RetrieveHeaders(i, 1);
                            break;
                        case IncomingProtocols.IMAP:
                            tmpListMailHeader = imapController.RetrieveHeaders(_MailBoxName, i, 1);
                            break;
                        default:
                            return (aMailHeader);  // Empty list ...
                            break;
                    }
                    // Starting from current position on ...
                    int cntInsert = 0;
                    foreach (MailHeader mh in tmpListMailHeader)
                    {
                        if (mh.UniqueId == "*")
                        { // Remove a no-more-valid item
                            int j = i + cntInsert;

                            if (mh.Index == aMailHeader[j].Index)   // It is the one to drop!
                            {
                                // ------------------------------------------------------------ //
                                // Be careful: now I remove the item ...
                                // ------------------------------------------------------------ //
                                for (; j < aMailHeader.Length - 1; j++)
                                    aMailHeader[j] = aMailHeader[j + 1];
                                // --- Size and Indexes bias ---------------------------------- //
                                iMax = aMailHeader.Length - 1;
                                idxStop += iDir;
                                if (i == idxStart)
                                    idxStart += iDir;

                                if (idxStart <= 0)
                                    idxStart = 1;
                                if (idxStart >= aMailHeader.Length - 1)
                                    idxStart = aMailHeader.Length - 1;

                                if (idxStop <= 0)
                                    idxStop = 1;
                                if (idxStop >= aMailHeader.Length - 1)
                                    idxStop = aMailHeader.Length - 1;

                                // ------------------------------------------------------------ //
                                Array.Resize(ref aMailHeader, aMailHeader.Length - 1);
                            }
                        }
                        else
                        {
                            if (aMailHeader[i + cntInsert].UniqueId != mh.UniqueId)
                            {
                                // Unsynchronized lists!
                                if (MailHeaderLoad() < 0)
                                {
                                    //TASK: Allineamento log - Ciro
                                    ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                                        "ERR_MAIL_0007", string.Empty,
                                        string.Empty, null);
                                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                                    //err.loggingAppCode = "SEND_MAIL";
                                    //if (System.Threading.Thread.CurrentContext.ContextID != null)
                                    //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                                    _log.Error(mEx);
                                    throw mEx;
                                }
                                    //throw new ManagedException("Non posso leggere la casella postale"
                                    //                          , "ERR_MAIL_0007", "MailServerFacade.cs", "MailHeader_Fetch", "MailHeaderLoad", "", "", null
                                    //                          );
                                return (MailHeader_Fetch(pidxStart, piCount));
                            }
                            else
                            {
                                aMailHeader[i + cntInsert] = mh;
                                cntInsert += iDir;
                            }
                        }
                    }
                }
            } // for ( i = idxStart ; ...

            ReloadForce = false;
            return (aMailHeader);
        } // MailHeader[] MailHeader_Fetch(...


        public bool MailHeader_ItemRemove(int piOrdinal)
        {
            bool bRetVal = false;

            if (piOrdinal < 1)
                return (bRetVal);
            try
            {
                switch (InProtoGet())
                {
                    case IncomingProtocols.POP3:
                        popController.Pop3Client.DeleteMessage(piOrdinal);
                        break;
                    case IncomingProtocols.IMAP:
                        break;
                    default:
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException("Protocol not yet implemented: " + InProtoGet().ToString(),
                            "ERR_MAIL_0097", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        //err.loggingAppCode = "SEND_MAIL";
                        //if (System.Threading.Thread.CurrentContext.ContextID != null)
                        //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                        _log.Error(mEx);
                        throw mEx;
                        //throw new ManagedException("Protocol not yet implemented: " + InProtoGet().ToString(), String.Empty, String.Empty, String.Empty, null);
                        break;
                }
                bRetVal = true;
            }
            catch (Exception ex)
            {
                bRetVal = false;

                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message, 
                        "ERR_SRVF01", 
                        string.Empty, 
                        string.Empty,
                        ex.InnerException);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    _log.Error(er);
                    throw mEx;
                }
                else throw;
            }
            MailHeader_ReloadForce();
            return (bRetVal);
        } // bool MailHeader_ItemRemove

        public bool MailHeader_ItemRemove(String pUId)
        {

            return (MailHeader_ItemRemove(MailHeader_ArrayList_OrdinalGetByUid(pUId)));
        }

        /// <summary>
        /// Called by UI.
        /// Reload parameter tells if the whole UIds must be reloaded.
        /// It checks all items we are going to display (the passed range).
        /// All MailHeader's info we gonna use must be already loaded. Otherwise it reloads 'em.
        /// </summary>
        /// <param name="Reload">Forces the reload of all UIds</param>
        /// <param name="pidxStart">First element's index to show</param>
        /// <param name="piCount">How many items have to be displayed.</param>
        /// <returns>A full list of all the element cached.</returns>
        public List<MailHeader> MailHeader_ArrayList_Fetch(Boolean Reload, int pidxStart, int piCount)
        {

            ReloadForce = Reload;
            return (MailHeader_ArrayList_Fetch(pidxStart, piCount));
        } // List<MailHeader> MailHeader_ArrayList_Fetch(...


        /// <summary>
        /// Called by UI.
        /// It checks all items we are going to display (the passed range).
        /// All MailHeader's info we gonna use must be loaded.
        /// </summary>
        /// <param name="pidxStart">First element's index to show</param>
        /// <param name="piCount">How many items have to be displayed.</param>
        /// <returns>A full list of all the element cached.</returns>
        public List<MailHeader> MailHeader_ArrayList_Fetch(int pidxStart, int piCount)
        {
            if (AccSettings.IsManaged)
            {
                return MailHeader_ArrayList_FetchFromDB(pidxStart, piCount);
            }

            // aMailHeader is an exact copy of the PostOffice list PLUS the first item (null): 
            // this will help mantaining the same index the PostOffice use.

            MailHeader_Fetch(pidxStart, piCount);
            return (ListCurrentGet());
        } // List<MailHeader> MailHeader_ArrayList_Fetch(...

        /// <summary>
        /// Convert the MailHeader table to a list that can be used for display purposes.
        /// </summary>
        /// <returns>A MailHeader list: either empty or full</returns>
        public List<MailHeader> ListCurrentGet()
        {
            List<MailHeader> l = new List<MailHeader>();


            if (aMailHeader != null)
            {
                // Convert to a list
                l.AddRange(aMailHeader);
                // Remove the first item (it is null): THIS IS MANDATORY! UI returns an error otherwise!
                l.RemoveAt(0);
                // Reverse the order: from newer (greater indexes, latest returned by the PO) to older (smaller indexes)
                l.Reverse();
            }
            // Either empty either full
            return (l);
        } // List<MailHeader> ListCurrentGet()


        /// <summary>
        /// Search for a UId into the local array aMailHeader ...
        /// </summary>
        /// <param name="pUId"></param>
        /// <returns></returns>
        public int MailHeader_ArrayList_OrdinalGetByUid(String pUId)
        {
            int iScan;


            for (iScan = 1; iScan < aMailHeader.Length; iScan++)
                if (pUId == aMailHeader[iScan].UniqueId)
                    break;

            if (iScan < aMailHeader.Length)
            {
                int iRetVal;

                if (!int.TryParse(aMailHeader[iScan].Index, out iRetVal))
                    return (-1);
                return (iRetVal);
            }
            else
                return (-1);

        } // int MailHeader_ArrayList_OrdinalGetByUid(...


        /// <summary>
        /// Search for an Index thru the local array aMailHeader...
        /// </summary>
        /// <param name="psIndex">The index to look for.</param>
        /// <returns></returns>
        public int MailHeader_ArrayList_OrdinalGetByIndex(String psIndex)
        {
            int iScan;


            for (iScan = 1; iScan < aMailHeader.Length; iScan++)
                if (psIndex == aMailHeader[iScan].Index)
                    break;

            if (iScan >= aMailHeader.Length)
                psIndex = "-1";

            int.TryParse(psIndex, out iScan);
            return (iScan);
        } // int MailHeader_ArrayList_OrdinalGetByUid(...


        /// <summary>
        /// Method to get all Headers message.
        /// </summary>
        /// <returns>A List of MailHeader</returns>
        //public  List<Header>                    Retrieves()
        //    {


        //    switch( InProtoGet() )
        //        {            
        //        case IncomingProtocols.POP3:
        //            return this.popController.RetrieveHeaders();
        //        case IncomingProtocols.IMAP:
        //            return this.imapController.RetrieveHeaders( _MailBoxName );
        //        default:
        //            return null;
        //        }
        //    }


        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        /*
            /// <summary>
            /// Method to gets all Messages.
            /// </summary>
            /// <returns>A List of Message</returns>
            public List<Message>                    getListMessages()
                {


                switch( InProtoGet() )
                    {
                    case IncomingProtocols.POP3:
                        return this.popController.ListMessageInbox;
                    case IncomingProtocols.IMAP:
                        return this.imapController.ListMessageInbox;
                    default:
                        return null;
                    }
                }

            /// <summary>
            /// Method for sets an Account Information.
            /// </summary>
            /// <param name="acc">The Account information</param>
            public  void                            setAccountInfo( AccountSettings.AccountInfo acc )
                {

                if ( this._accSettings == null )
                    this.AccSettings = new AccountSettings();

                this._accSettings.Acc_Info = acc;
                }


            /// <summary>
            ///Method to reconnect to the incoming mail.
            /// </summary>
            public  void                            ReConnect()
                {
                this.IncomingDisconnect();
                this.IncomingConnect();
                }
        */
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //




        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        #endregion

        #region Send

        private Message AMessage = new Message();
        public Message MailMessage
        {
            get { return (AMessage); }
            set { AMessage = value; }
        }


        public Message sendMail_New()
        {

            AMessage = new Message();
            AMessage.From.Email = AccountInfoCurrentGet().EmailAddress;
            return (AMessage);
        }


        #region SendMail - Addresses

        //regular expression for email
        //LR// Regular expression to validate a mail address.
        private string AddressValidate_Pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";


        /// <summary>
        /// Validate the TO/CC/BCC email field
        /// </summary>
        /// <param name="mails">The mails string, separated by either ',' or ';'</param>
        /// <param name="colAddresses">The destination AddressCollection object of a mail</param>
        /// <returns>true, if successful</returns>

        private bool validateAddresses(string mails, ref AddressCollection colAddresses)
        {
            bool ret = true;
            mails = mails.Trim();
            string[] ccArray = mails.Split(new char[] { ',', ';' });

            for (int i = 0; i < ccArray.Length; i++)
                if (ccArray[i] != String.Empty)
                    if (Regex.IsMatch(ccArray[i], AddressValidate_Pattern))
                        colAddresses.Add(ccArray[i]);
                    else
                    {
                        ret = false;
                        break;
                    }
            return ret;
        } // bool validateAddresses(...


        public bool sendMailTO(ref Message pMsg, String psTO)
        {
            AddressCollection ac = new AddressCollection();
            bool bret;

            ac = pMsg.To;
            bret = validateAddresses(psTO, ref ac);
            pMsg.To = ac;
            return bret;
        }


        public bool sendMailCC(ref Message pMsg, String psCC)
        {
            AddressCollection ac = new AddressCollection();
            bool bret;

            ac = pMsg.Cc;
            bret = validateAddresses(psCC, ref ac);
            pMsg.Cc = ac;
            return bret;
        }


        public bool sendMailBCC(ref Message pMsg, String psBCC)
        {
            AddressCollection ac = new AddressCollection();
            bool bret;

            ac = pMsg.Bcc;
            bret = validateAddresses(psBCC, ref ac);
            pMsg.Bcc = ac;
            return bret;
        }


        public bool sendMailAddresses(ref Message pMsg, String psTO, String psCC, String psBCC)
        {
            return (sendMailTO(ref pMsg, psTO) && sendMailCC(ref pMsg, psCC) && sendMailBCC(ref pMsg, psBCC));
        }

        public bool sendMailRicevutaDiRitorno(ref Message pMsg)
        {

            pMsg.ConfirmRead = new Address(AccountInfoCurrentGet().EmailAddress);
            return (true);
        }

        public bool sendMail_IsRicevutaDiRitorno(Message pMsg, String psMailAddress)
        {
            return (pMsg.ConfirmRead == new Address(psMailAddress));
        }

        public bool sendMailAttachment(ref Message pMsg, String psPathName)
        {
            try
            {
                pMsg.Attachments.Add(psPathName, true);
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("PathName=[" + psPathName + "]. Dettaglio: " + ex.Message,
                                "ERR_MAIL_0009", string.Empty,
                                string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    //if (System.Threading.Thread.CurrentContext.ContextID != null)
                    //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                    _log.Error(mEx);
                    throw mEx;
                }
                else throw;
                //throw new ManagedException("PathName=[" + psPathName + "]"
                //                          , "ERR_MAIL_0009", "MailServerFacade.cs", "pMsg.sendMailAttachment.Add"
                //                          , String.Empty, String.Empty, String.Empty, ex
                //                          );
            }
            return (pMsg.Attachments.Contains(psPathName));
        }

        #endregion



        public void sendMailInfo(String psTo, String psSubject)
        {
            AMessage.To.Add(new Address(psTo));
            AMessage.Subject = psSubject;
        }

        public void sendMail(Message message)
        {
            try
            {
                smtpController.SendMail(message, _accSettings);
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Non posso leggere la casella postale. Dettaglio " + ex.Message,
                                "ERR_MAIL_0008", string.Empty,
                                string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    //err.loggingAppCode = "SEND_MAIL";
                    //if (System.Threading.Thread.CurrentContext.ContextID != null)
                    //    err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                    _log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException("Non posso leggere la casella postale"
                    //                          , "ERR_MAIL_0008", "MailServerFacade.cs", "sendMail", "smtpController.sendMail", "", "", ex
                    //                          ); 
                }
                else throw;
            }
        }

        #endregion

        #region Managed Accounts

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Reload"></param>
        /// <param name="pidxStart"></param>
        /// <param name="piCount"></param>
        /// <returns></returns>
        public ResultList<MailHeader> MailHeader_ResultList_Fetch(Boolean Reload, int pidxStart, int piCount)
        {
            ReloadForce = Reload;
            return MailHeader_ResultList_Fetch(pidxStart, piCount);
        }

        /// <summary>
        /// modificata gestione folder
        /// </summary>
        /// <param name="pidxStart"></param>
        /// <param name="piCount"></param>
        /// <returns></returns>
        public ResultList<MailHeader> MailHeader_ResultList_Fetch(int pidxStart, int piCount, string folder)
        {
            ResultList<MailHeader> res = null;
            if (this.AccSettings.IsManaged)
            {
                try
                {
                    ResultList<MailHeaderExtended> resExt = MailLocalService.Instance.GetAllMailsReceivedByAccount(
                        this.AccSettings.EmailAddress, folder, pidxStart, piCount);
                    if (resExt != null && resExt.List != null)
                        res = new ResultList<MailHeader>() { Da = resExt.Da, Per = resExt.Per, Totale = resExt.Totale, List = resExt.List.Cast<MailHeader>().ToList() };
                }
                catch (Exception ex)
                {
                    res = null;
                    //Allineamento log - Ciro
                    if (ex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(ex.Message,
                            "ERR_SRVF02",
                            string.Empty,
                            string.Empty,
                            ex.InnerException);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        _log.Error(er);
                        throw mEx;
                    }
                    else throw;
                }
            }
            else
            {
                List<MailHeader> lis = MailHeader_ArrayList_Fetch(pidxStart, piCount);
                res = new ResultList<MailHeader>();
                res.Da = pidxStart;
                res.Per = piCount;
                res.Totale = aMailHeader.Length;
                res.List = lis.Skip(pidxStart).Take(piCount).ToList();
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailFolder"></param>
        /// <param name="Reload"></param>
        /// <param name="pidxStart"></param>
        /// <param name="piCount"></param>
        /// <returns></returns>
        public ResultList<MailHeader> MailHeader_ResultList_Fetch(string mailFolder, string parentFolder, Boolean Reload, int pidxStart, int piCount)
        {
            ReloadForce = Reload;
            return MailHeader_ResultList_Fetch(mailFolder, parentFolder, pidxStart, piCount);
        }

        /// <summary>
        /// modificata gestione folders
        /// </summary>
        /// <param name="mailFolder"></param>
        /// <param name="pidxStart"></param>
        /// <param name="piCount"></param>
        /// <returns></returns>
        public ResultList<MailHeader> MailHeader_ResultList_Fetch(string mailFolder, string parentFolder, int pidxStart, int piCount)
        {
            ResultList<MailHeader> res = null;
            if (this.AccSettings.IsManaged)
            {
                try
                {
                    ResultList<MailHeaderExtended> resExt = null;

                    switch (parentFolder)
                    {
                        case "I":
                            resExt =
                            MailLocalService.Instance.GetAllMailsReceivedByAccount(
                            this.AccSettings.EmailAddress, mailFolder, pidxStart, piCount);
                            break;
                        case "O":
                        case "CO":
                            resExt =
                                MailLocalService.Instance.GetAllMailsSentByAccount(
                                this.AccSettings.EmailAddress, mailFolder, pidxStart, piCount);
                            break;
                        case "A":
                        case "AO":
                            resExt = MailLocalService.Instance.GetAllMailArchivedByAccount(
                             this.AccSettings.EmailAddress, mailFolder, parentFolder, pidxStart, piCount);
                            break;
                        case "C":
                            resExt = MailLocalService.Instance.GetAllMailsCanceledByAccount(
                                         this.AccSettings.EmailAddress, mailFolder, pidxStart, piCount);
                            break;
                        default:
                            break;
                    }

                    if (resExt == null) resExt = new ResultList<MailHeaderExtended>();

                    res = new ResultList<MailHeader>() { Da = resExt.Da, Per = resExt.Per, Totale = resExt.Totale, List = new List<MailHeader>() };
                    if (resExt.List != null)
                        res.List = resExt.List.Cast<MailHeader>().ToList();
                }
                catch (Exception ex)
                {
                    res = null;

                    //Allineamento log - Ciro
                    if (ex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(ex.Message,
                            "ERR_SRVF03",
                            string.Empty,
                            string.Empty,
                            ex.InnerException);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        _log.Error(er);
                        throw mEx;
                    }
                    else throw;
                }
            }
            else
            {
                List<MailHeader> lis = MailHeader_ArrayList_Fetch(pidxStart, piCount);
                res = new ResultList<MailHeader>();
                res.Da = pidxStart;
                res.Per = piCount;
                res.Totale = aMailHeader.Length;
                res.List = lis.Skip(pidxStart).Take(piCount).ToList();
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pidxStart"></param>
        /// <param name="piCount"></param>
        /// <returns></returns>
        public ResultList<MailHeader> MailHeader_ResultList_Fetch(int pidxStart, int piCount)
        {
            ResultList<MailHeader> res = null;
            if (this.AccSettings.IsManaged)
            {
                try
                {
                    ResultList<MailHeaderExtended> resExt = MailLocalService.Instance.GetAllMailsReceivedByAccount(
                        this.AccSettings.EmailAddress, "1", pidxStart, piCount);
                    if (resExt != null && resExt.List != null)
                        res = new ResultList<MailHeader>() { Da = resExt.Da, Per = resExt.Per, Totale = resExt.Totale, List = resExt.List.Cast<MailHeader>().ToList() };
                }
                catch (Exception ex)
                {
                    res = null;

                    //Allineamento log - Ciro
                    if (ex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(ex.Message,
                            "ERR_SRVF04",
                            string.Empty,
                            string.Empty,
                            ex.InnerException);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        _log.Error(er);
                        throw mEx;
                    }
                    else throw;
                }
            }
            else
            {
                List<MailHeader> lis = MailHeader_ArrayList_Fetch(pidxStart, piCount);
                res = new ResultList<MailHeader>();
                res.Da = pidxStart;
                res.Per = piCount;
                res.Totale = aMailHeader.Length;
                res.List = lis.Skip(pidxStart).Take(piCount).ToList();
            }
            return res;
        }


        public int MailMove(List<string> idMails, MailStatus newStatus, string actionid, String utente, string parentFolder)
        {
            int result = 0;
            if (this.AccSettings.IsManaged)
            {
                result = MailLocalService.Instance.UpdateMailStatus(this.AccSettings.EmailAddress, idMails, newStatus, actionid, utente, parentFolder);
            }
            return result;
        }

        public int MailCancella(List<string> idMails, string utente, string action, string parentFolder)
        {
            int result = 0;
            if (this.AccSettings.IsManaged)
            {
                switch (parentFolder)
                {
                    case "O":
                        result = MailLocalService.Instance.UpdateMailSentStatus(this.AccSettings.EmailAddress, idMails, MailStatus.CANCELLATA, utente, action, parentFolder);
                        break;
                    case "I":
                        result = MailLocalService.Instance.UpdateMailStatus(this.AccSettings.EmailAddress, idMails, MailStatus.CANCELLATA, utente, action, parentFolder);
                        break;
                }
            }
            return result;
        }

        public int MailArchivia(List<string> idMails, string utente, string action, string parentFolder)
        {
            int result = 0;          
            if (this.AccSettings.IsManaged)
            {
                switch (parentFolder)
                {
                    case "O":
                        result = MailLocalService.Instance.UpdateMailSentStatus(this.AccSettings.EmailAddress, idMails, MailStatus.ARCHIVIATA, utente, action, parentFolder);
                        break;
                    case "I":
                        result = MailLocalService.Instance.UpdateMailStatus(this.AccSettings.EmailAddress, idMails, MailStatus.ARCHIVIATA, utente, action, parentFolder);
                        break;
                }
            }
            return result;
        }



        public int MailRipristina(List<string> idMail, string parentFolder, string utente)
        {
            int result = 0;
            if (this.AccSettings.IsManaged)
            {
                switch (parentFolder)
                {
                    case "O":
                    case "AO":
                        result = MailLocalService.Instance.RipristinaMailOutBox(this.AccSettings.EmailAddress, idMail, parentFolder, utente);
                        break;
                    default:
                        result = MailLocalService.Instance.RipristinaMail(this.AccSettings.EmailAddress, idMail, parentFolder, utente);
                        break;
                }
            }
            return result;
        }

        private List<MailHeader> MailHeader_ArrayList_FetchFromDB(int pidxStart, int piCount)
        {
            List<MailHeader> res = null;
            ResultList<MailHeader> resExt = null;
            try
            {
                resExt = MailHeader_ResultList_Fetch(pidxStart, piCount);
            }
            catch
            {
                resExt = null;
            }

            if (resExt != null && resExt.List != null)
            {
                res = resExt.List.Cast<MailHeader>().ToList();
            }
            return res;
        }

        public int AssignMessageRating(string idMail, int rating)
        {
            return MailLocalService.Instance.UpdateMessageRating(idMail, this.AccSettings.EmailAddress, rating);
        }

        #endregion
    }

}

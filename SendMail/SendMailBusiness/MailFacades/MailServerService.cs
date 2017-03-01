using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Mail;
using Com.Delta.Logging;
using SendMail.Business.Base;
using SendMail.Business.Contracts;
using Com.Delta.Logging.Errors;
using log4net;

namespace SendMail.Business.MailFacades
{
    public sealed class MailServerService : BaseSingletonService<MailServerService>, IMailServerService
    {

        private static readonly ILog _log = LogManager.GetLogger("MailFacades");
        
        private Pop3Controller popController = null;
        private ImapController imapController = null;
        private SmtpController smtpController = null;

        private MailUser _AccSettings = null;
        private DateTime _LastRefresh = DateTime.MinValue;

        /// <summary>
        /// Account Types.
        /// </summary>
        public enum IncomingProtocols
        {
            POP3, IMAP
        }

        #region IMailServerService Membri di

        private String _MailBoxName = "inbox";
        public String MailBoxName
        {
            get { return (_MailBoxName); }
            set { _MailBoxName = value; }
        }

        public DateTime LastRefresh
        {
            get { return _LastRefresh; }
        }

        public ActiveUp.Net.Mail.DeltaExt.MailUser AccSettings
        {
            get { return _AccSettings; }
        }

        public Pop3Controller Pop3
        {
            get
            {
                if (this.popController == null)
                {
                    popController = new Pop3Controller();
                }
                return popController;
            }
        }

        public ImapController Imap
        {
            get
            {
                if (this.imapController == null)
                {
                    imapController = new ImapController();
                }
                return imapController;
            }
        }

        public SmtpController Smtp
        {
            get
            {
                if (this.smtpController == null)
                {
                    smtpController = new SmtpController();
                }
                return smtpController;
            }
        }

        #region Singleton

        public delegate Boolean Connect(MailUser acs);

        public MailServerService GetInstance(MailUser acs)
        {
            this._AccSettings = acs;
            this.IncomingConnect();

            Connect conn = null;
            switch (acs.IncomingProtocol)
            {
                case "POP3":
                    if (!String.IsNullOrEmpty(acs.IncomingServer))
                    {
                        conn = this.Pop3.Connect;
                    }
                    break;
                case "IMAP":
                    if (!String.IsNullOrEmpty(acs.IncomingServer))
                    {
                        conn = this.Imap.Connect;
                    }
                    break;
            }

            if (!conn(_AccSettings))
            {
                //TASK: Allineamento log - Ciro
                ManagedException mEx = new ManagedException(
                    String.Format("Mail: non posso connettermi a {0} usando il protocollo {1}", _AccSettings.IncomingServer, _AccSettings.IncomingProtocol),
                        "ERR_MAIL_0101", 
                        string.Empty,
                        string.Empty, 
                        null);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                _log.Error(mEx);
                throw mEx;
                //throw new ManagedException(String.Format("Mail: non posso connettermi a {0} usando il protocollo {1}", _AccSettings.IncomingServer, _AccSettings.IncomingProtocol)
                //                          , "ERR_MAIL_0001", "MailServerFacade.cs", "MailServerFacade", "popController.Connect", "", "", null
                //                          );
            }
            this.IncomingDisconnect();
            return this;
        }

        public void DropInstance(MailUser acs)
        {
            this._AccSettings = null;
            this.popController = null;
            this.imapController = null;
            this.smtpController = null;
        }

        #endregion

        public ActiveUp.Net.Mail.DeltaExt.MailUser AccountInfoCurrentGet()
        {
            if (this.AccSettings != null)
                return this.AccSettings;
            else
                return null;
        }

        public bool CanIReceive(bool pbReconnect)
        {

            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    if (!this.Pop3.IsMailServerReady())
                    {
                        if (pbReconnect)
                        {
                            ReConnect();
                            if (!this.Pop3.IsMailServerReady())
                                return (false);
                        }
                        else
                            return (false);
                    }
                    break;
                case IncomingProtocols.IMAP:
                    if (!this.Imap.CanIReceive())
                    {
                        if (pbReconnect)
                        {
                            ReConnect();
                            if (!this.Imap.CanIReceive())
                                return (false);
                        }
                        else
                            return (false);
                    }
                    break;
                default:
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Protocol not yet implemented: " + InProtoGet().ToString(),
                        "ERR_MAIL_0111", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException("Protocol not yet implemented: " + InProtoGet().ToString(), String.Empty, String.Empty, String.Empty, null);
                    //break;
            }

            return (true);
        } // bool IsMailServerReady(...

        /// <summary>
        /// Returns the server protocol used ( i.e.: configured ) to read mails.
        /// </summary>
        /// <returns>One out of the IncomingProtocols values</returns>
        public IncomingProtocols InProtoGet()
        {
            const IncomingProtocols KINPROTO_DEFAULT = IncomingProtocols.POP3;
            IncomingProtocols uatRetVal = KINPROTO_DEFAULT;


            if (_AccSettings == null)
                return (uatRetVal);

            if (_AccSettings.IncomingProtocol == IncomingProtocols.POP3.ToString())
                return (IncomingProtocols.POP3);
            if (_AccSettings.IncomingProtocol == IncomingProtocols.IMAP.ToString())
                return (IncomingProtocols.IMAP);

            return (KINPROTO_DEFAULT);         //LR// It would be nice return 'unknown'
        } // IncomingProtocols InProtoGet()

        /// <summary>
        /// Retrieve a message by UId.
        /// </summary>
        /// <param name="pUId"></param>
        /// <param name="doARefreshBefore"></param>
        /// <returns></returns>
        public Message getMessage(string pUId, bool doARefreshBefore)
        {
            Message msgRetVal = null;
            int iOrdinal;

            if (!CanIReceive(true)) { return msgRetVal; }

            if (doARefreshBefore)
                if (MailHeaderLoad() < 0)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                        "ERR_MAIL_0104", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(mEx);
                    throw mEx;
                }
                    //throw new ManagedException("Non posso leggere la casella postale"
                    //                          , "ERR_MAIL_0004", "MailServerFacade.cs", "getMessage", "MailHeaderLoad", "", "", null
                    //                          );
            if ((iOrdinal = MailHeader_ArrayList_OrdinalGetByUid(pUId)) < 0)
                return (msgRetVal);

            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    msgRetVal = this.Pop3.getMessageByOrdinal(iOrdinal);
                    break;
                case IncomingProtocols.IMAP:
                    msgRetVal = this.Imap.getMessageByOrdinal(iOrdinal);
                    break;
                default:
                    return msgRetVal;
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

        public Message getMessageByOrdinal(string psOrdinal, bool doARefreshBefore)
        {
            Message msgRetVal = null;
            int iOrdinal;

            if (!CanIReceive(true))
                return msgRetVal;

            if (doARefreshBefore)
                if (MailHeaderLoad() < 0)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                        "ERR_MAIL_0102", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(mEx);
                    throw mEx;
                }
                    //throw new ManagedException("Non posso leggere la casella postale"
                    //                          , "ERR_MAIL_0002", "MailServerFacade.cs", "getMessageByOrdinal", "MailHeaderLoad", "", "", null
                    //                          );

            if (!int.TryParse(psOrdinal, out iOrdinal))
                return (msgRetVal);

            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    return this.Pop3.getMessageByOrdinal(iOrdinal);
                case IncomingProtocols.IMAP:
                    return this.Imap.getMessageByOrdinal(iOrdinal);
                default:
                    return null;
            }
        }

        public Message getMessageByOrdinal(int piOrdinal, bool doARefreshBefore)
        {
            return getMessage(piOrdinal.ToString(), doARefreshBefore);
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
                        "ERR_MAIL_0103", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(mEx);
                    throw mEx;
                }
                    //throw new ManagedException("Non posso leggere la casella postale"
                    //                          , "ERR_MAIL_0003", "MailServerFacade.cs", "getMessageByIndex", "MailHeaderLoad", "", "", null
                    //                          );

            if ((piIndex < 1) || (piIndex >= aMailHeader.Length))
                return (msgRetVal);

            if (!int.TryParse(aMailHeader[piIndex].Index, out iOrdinal))
                return (msgRetVal);

            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    return this.Pop3.getMessageByOrdinal(iOrdinal);
                case IncomingProtocols.IMAP:
                    return this.Imap.getMessageByOrdinal(iOrdinal);
                default:
                    return null;
            }
        }

        public void IncomingConnect()
        {
            if (_AccSettings != null)
            {
                if (InProtoGet() == IncomingProtocols.POP3)
                {
                    if (this.Pop3.Pop3Client != null && this.Pop3.Pop3Client.IsConnected)
                        this.IncomingDisconnect();
                     this.Pop3.Connect(_AccSettings);
                    if (CanIReceive(false)) // CANNOT BE true: IsMailServerReady calls this routine! The risk is a StackOverflow exception!
                    {
                        if (MailHeaderLoad() < 0)
                        { 
                            //TASK: Allineamento log - Ciro
                            ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                                "ERR_MAIL_0105", string.Empty,
                                string.Empty, null);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
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
                if (this.Imap.Imap4Client == null)
                {
                    if (!this.Imap.Imap4Client.IsConnected)
                        this.IncomingDisconnect();
                    this.Imap.Connect(_AccSettings);
                }
                if (CanIReceive(false)) // CANNOT BE true: IsMailServerReady calls this routine! The risk is a StackOverflow exception!
                {
                    if (MailHeaderLoad() < 0)
                    {
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException("Non posso leggere la casella postale",
                            "ERR_MAIL_0106", 
                            string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
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
            if (_AccSettings == null)
                return (false);

            if (InProtoGet() == IncomingProtocols.POP3 && (this.Pop3.Pop3Client != null && this.Pop3.Pop3Client.IsConnected))
                return (true);
            else
                return (this.Imap.Imap4Client != null && this.Imap.Imap4Client.IsConnected);
        }

        public void IncomingDisconnect()
        {
            if (IsIncomingConnected())
                switch (InProtoGet())
                {
                    case IncomingProtocols.POP3:
                        this.Pop3.Disconnect();
                        break;
                    case IncomingProtocols.IMAP:
                        this.Imap.Disconnect();
                        break;
                    default:
                        break;
                }
        }

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
        }

        private Boolean ReloadForce = true;

        public void MailHeader_ReloadForce()
        {
            triggerChanges = null;
        }

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
                            tmpListMailHeader = this.Pop3.RetrieveHeaders(i, 1);
                            break;
                        case IncomingProtocols.IMAP:
                            tmpListMailHeader = this.Imap.RetrieveHeaders(_MailBoxName, i, 1);
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
                                        "ERR_MAIL_0107", string.Empty,
                                        string.Empty, null);
                                    ErrorLogInfo err = new ErrorLogInfo(mEx);
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
            // aMailHeader is an exact copy of the PostOffice list PLUS the first item (null): 
            // this will help mantaining the same index the PostOffice use.

            MailHeader_Fetch(pidxStart, piCount);
            return (ListCurrentGet());
        }

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
                        this.Pop3.Pop3Client.DeleteMessage(piOrdinal);
                        break;
                    case IncomingProtocols.IMAP:
                        break;
                    default:
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException("Protocol not yet implemented: " + InProtoGet().ToString(),
                            "ERR_MAIL_0111", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(mEx);
                        throw mEx;
                        //throw new ManagedException("Protocol not yet implemented: " + InProtoGet().ToString(), String.Empty, String.Empty, String.Empty, null);
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
                        "ERR_SRVF05",
                        string.Empty,
                        string.Empty,
                        ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    _log.Error(er);
                    throw mEx;
                }
                else throw;
            }
            MailHeader_ReloadForce();
            return (bRetVal);
        }

        public bool MailHeader_ItemRemove(string pUId)
        {
            return (MailHeader_ItemRemove(MailHeader_ArrayList_OrdinalGetByUid(pUId)));
        }

        public void ReConnect()
        {
            this.IncomingDisconnect();
            this.IncomingConnect();
        }

        public void sendMail(Message message)
        {
            try
            {
                this.Smtp.SendMail(message, _AccSettings);
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Non posso leggere la casella postale. Dettaglio: " + ex.Message,
                                "ERR_MAIL_0008", string.Empty,
                                string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(mEx);
                    throw mEx;
                }
                else throw;
                //throw new ManagedException("Non posso leggere la casella postale"
                //                          , "ERR_MAIL_0008", "MailServerFacade.cs", "sendMail", "smtpController.sendMail", "", "", ex
                //                          );
            }
        }

        #endregion

        // This will hold UIds List and the MailHeader(s).
        // This is the real work copy.
        public MailHeader[] aMailHeader = null;

        // After have fully loaded the aMailHeader, this will hold an exact copy.
        // In case of reloading, this array if checked BEFORE request data to PO.
        // This will hopefully speed up the overall reload process.
        public MailHeader[] aMailHeader_CurrCopy = null;

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
                        lstUId = this.Pop3.RetrieveUIds();
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
                            "ERR_MAIL_0112", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(mEx);
                        throw mEx;
                        //throw new ManagedException("Imap not supported at all!", String.Empty, String.Empty, String.Empty, null);
                        break;
                    default:
                        return null;
                }
            } while ((this.Pop3.Pop3Client.IsConnected) && (triggerChanges.StatisticsUpdate() == true));

            return (lstMUId);
        } // List<MailUniqueId> RetrieveUIds()

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

        private bool FetchScanCondition(int piDir, int piIndex, int piLimit)
        {

            if (piDir > 0)
                return (piIndex <= piLimit);
            else
                return (piIndex >= piLimit);
        }

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

        public void DeleteMessageFromServer(string uid)
        {
            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    // Ask server
                    //this.Pop3.DeleteMessage(uid);
                    this.Pop3.DeleteMessageByUID(uid);
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
                        "ERR_MAIL_0113", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    _log.Error(mEx);
                    throw mEx;

                    //throw new ManagedException("Imap not supported at all!", String.Empty, String.Empty, String.Empty, null);
                    break;
                default:
                    return;
            }
        }

        public int GetMessageSize(string uid)
        {
            switch (InProtoGet())
            {
                case IncomingProtocols.POP3:
                    int idx = this.Pop3.Pop3Client.GetMessageIndex(uid);
                    if (idx > 0)
                    {
                        return this.Pop3.Pop3Client.GetMessageSize(idx);
                    }
                    else return -1;
                    break;
                case IncomingProtocols.IMAP:
                    //throw new ManagedException("Imap not supported at all!", String.Empty, String.Empty, String.Empty, null);
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Imap not supported at all!",
                        "ERR_MAIL_0114", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
               
                    _log.Error(mEx);
                    throw mEx;
                    break;
                default:
                    return 0;
            }
        }

        public class PostOfficeChangesTrigger
        {
            private MailServerService service;

            private int count;
            private int size;
            private DateTime dtLast;
            private bool bChanged;


            public PostOfficeChangesTrigger(MailServerService pServ)
            {
                count = -1;
                size = -1;
                service = pServ;
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
                if ((service.popController == null)
                    || (service.popController.Pop3Client == null)
                   )
                    return (bChanged);
                if (service.CanIReceive(true) == false)
                    return (bChanged);
                // get updated info
                service.popController.Pop3Client.UpdateStats();
                // set the internal status
                bChanged = (count != service.popController.MessageCountGet());
                bChanged |= (size != service.popController.MessageSizeGet());
                // Update internal info
                count = service.popController.MessageCountGet();
                size = service.popController.MessageSizeGet();
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
    }
}

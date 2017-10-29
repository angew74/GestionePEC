using System;
using System.Collections.Generic;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using log4net;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using System.Configuration;

/// <summary>
/// This controller is used for retrieve, delete and manipulate
/// mail messages for POP3 protocol
/// </summary>

public class Pop3Controller
{
    const Boolean KbLogEnabled = false;
    private static readonly ILog log = LogManager.GetLogger("Pop3Controller");

    private static string APP_CODE;

    #region Properties


    /// <summary>
    /// Represents the POP client
    /// </summary>
    private Pop3Client _pop3Client;
    public Pop3Client Pop3Client
    {
        get { return _pop3Client; }
        set { _pop3Client = value; }
    }
    /// <summary>
    /// Last Error Message
    /// </summary>
    private String Error_Message;
    public String ErrorMessage
    {
        get { return Error_Message; }
        set
        {
            Error_Message = value;
            ErrorTrace(Error_Message);
        }
    }

    #region Log management

    /// <summary>
    /// Enable / disable command tracing
    /// </summary>
    private Boolean bLogEnable = false;
    public Boolean LogEnable
    {
        get { return (bLogEnable); }
        set { bLogEnable = value; }
    }

    public Boolean IsLogEnabled
    {
        get { return (bLogEnable && (sLogPathName.Length > 0)); }
    }

    /// <summary>
    /// File where to write the command executing
    /// </summary>
    private String sLogPathName = "";
    public String LogPathName
    {
        get { return (sLogPathName); }
        set { sLogPathName = value; }
    }

    private void DoTrace(String psPrefix, String psMessage)
    {

        if (!IsLogEnabled) return;
        if (psMessage.Length < 1) return;
        try
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sLogPathName, true))
            {
                sw.WriteLine(psPrefix + psMessage);
                sw.Close();
            }
        }
        catch (Exception e)
        { // Do nothing if trace/log is in error.
        }
    }

    private void CommandTrace(String psMsg)
    {

        DoTrace("cmd-", psMsg);
    }

    private void ErrorTrace(String psMsg)
    {

        DoTrace("err-", psMsg);
    }

    private void ResponseTrace(String psMsg)
    {

        DoTrace("res-", psMsg);
    }

    private MailUser AccountInfo { get; set; }

    #endregion

    #endregion

    #region Contructors

    static void Init()
    {
        ApplicationCodeConfigSection configSection = (ApplicationCodeConfigSection)ConfigurationManager.GetSection("ApplicationCode");
        if (configSection == null) return;
        APP_CODE = configSection.AppCode;
    }

    private void Init(Boolean pbLogEnable)
    {

        bLogEnable = pbLogEnable;
        if (bLogEnable)
        {
            if (sLogPathName.Length < 1)
                bLogEnable = false;
            if (_pop3Client != null)
            {
                _pop3Client.TcpWriting += new TcpWritingEventHandler(On_pop3Client_TcpWriting);
                _pop3Client.TcpRead += new TcpReadEventHandler(On_pop3Client_TcpRead);
            }
        }
    }


    public void On_pop3Client_TcpWriting(object sender, TcpWritingEventArgs ea)
    {

        CommandTrace(ea.Command);
    }

    public void On_pop3Client_TcpRead(object sender, TcpReadEventArgs ea)
    {

        ResponseTrace(ea.Response);
    }

    public Pop3Controller()
    {
        Init();
        //this.ListMessageInbox = new List<Message>();
        //this.ListHeaderInbox = new List<MailHeader>();
    }

    public Pop3Controller(MailUser accountInfo)
    {
        AccountInfo = accountInfo;
        Init();
        //this.ListMessageInbox = new List<Message>();
        //this.ListHeaderInbox = new List<MailHeader>();
        //this.IncomingConnect(accountInfo);
    }

    #endregion

    #region Methods

    #region Connection handling
    /// <summary>
    /// Effettua la connessione al server di posta elettronica
    /// </summary>
    /// <param name="accountInfo">The information account</param>
    public Boolean Connect(MailUser accountInfo)
    {
        Boolean bRetVal = false;
        AccountInfo = accountInfo;

        ErrorMessage = "";
        if (this._pop3Client == null || !this._pop3Client.IsConnected)
        {
            if (accountInfo != null) // && accountInfo.IncomingProtocol.Equals("POP3") )
            {
                this._pop3Client = new Pop3Client();
                this._pop3Client.Disconnected += new DisconnectedEventHandler(On_pop3ClientDisconnected);
                this._pop3Client.Connected += new ConnectedEventHandler(On_pop3ClientConnected);
                this._pop3Client.Authenticating += new AuthenticatingEventHandler(On_pop3Client_Authenticating);
                this._pop3Client.Authenticated += new AuthenticatedEventHandler(On_pop3Client_Authenticated);
                Init(KbLogEnabled);
                int port = accountInfo.PortIncomingServer;
                bool ssl = accountInfo.IsIncomeSecureConnection;
                string serverName = accountInfo.IncomingServer;
                string user = accountInfo.LoginId;
                string password = accountInfo.Password;
                bool useInPort = accountInfo.PortIncomingChecked;
                try
                {
                    if (ssl)
                    {
                        if (useInPort)
                        {
                            this._pop3Client.ConnectSsl(serverName, port, user, password);
                        }
                        else
                        {
                            this._pop3Client.ConnectSsl(serverName, user, password);
                        }
                    }
                    else
                    {
                        if (useInPort)
                        {
                            this._pop3Client.Connect(serverName, port, user, password);
                        }
                        else
                        {
                            this._pop3Client.Connect(serverName, user, password);
                        }
                    }

                    bRetVal = true;
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(
                        "Errore nel metodo 'Connect: '" + ex.Message,
                        "POP3_ERR_001",
                        string.Empty,
                        "Casella mail: " + ((AccountInfo.LoginId != null) ? AccountInfo.LoginId : " vuoto. ") +
                        "Server Name: " + ((serverName.ToString() != null) ? serverName.ToString() : " vuoto. "), //EnanchedInfo
                        ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        log.Error(err);
                    }
                    ErrorMessage = ex.Message;
                    StatusConnected = false;
                    bRetVal = false;
                }
            }
        }
        bRetVal = this._pop3Client.IsConnected;
        return (bRetVal);
    }

    public void On_pop3Client_Authenticated(object sender, AuthenticatedEventArgs e)
    {
        log.Debug(String.Format("Autenticato. Host: {0} - User: {1} - Password: {2}", e.Host, e.Username, e.Password));
    }

    public void On_pop3Client_Authenticating(object sender, AuthenticatingEventArgs e)
    {
        log.Debug(String.Format("In autenticazione. Host: {0} - User: {1} - Password: {2}", e.Host, e.Username, e.Password));
    }

    // Boolean Connect(...
    #region StatusConnected - Handlers

    private bool StatusConnected = false;

    public void On_pop3ClientDisconnected(Object sender, EventArgs ea)
    {

        StatusConnected = false;
    } // void On_pop3ClientDisconnected(...

    public void On_pop3ClientConnected(Object sender, EventArgs ea)
    {

        StatusConnected = true;
        log.Debug("Pop3 connesso: " + ((ConnectedEventArgs)ea).ServerResponse);
    } // void On_pop3ClientConnected(...
    #endregion


    /// <summary>
    /// Disconnette l'utente dal server di posta elettronica
    /// </summary>
    public void Disconnect()
    {
        ErrorMessage = "";
        try
        {
            if (StatusConnected || (this._pop3Client != null && this._pop3Client.IsConnected))
                this._pop3Client.Disconnect();
        }
        catch (Exception ex)
        {
            //Allienamento log - Ciro
            if (ex.GetType() != typeof(ManagedException))
            {
                ManagedException mEx = new ManagedException(
                "Disconnessione dal server non riuscita",
                "POP3_ERR_002",
                string.Empty,
                "Casella mail: " + ((AccountInfo.LoginId != null) ? AccountInfo.LoginId : " vuoto. "), //EnanchedInfo
                ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                log.Error(err);
            }
            //ManagedException mEx = new ManagedException(
            //    "Disconnessione dal server non riuscita",
            //    "POP3_ERR_002",
            //    "Pop3Controller",
            //    "Disconnect()",
            //    "Disconnessione dal server POP3",
            //    (AccountInfo == null) ? "" : AccountInfo.LoginId,
            //    null,
            //    ex);
            //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
            //log.Error(err);
            ErrorMessage = ex.Message;
        }
    }

    #endregion

    #region "All messages" info read

    /// <summary>
    /// Recupera il numero di messagi dal server di posta
    /// </summary>
    /// <returns>The message count</returns>
    public int getMessageCount()
    {
        return this._pop3Client.MessageCount;
    }

    /// <summary>
    /// Returns the message count.
    /// </summary>
    /// <returns>The message count number.</returns>
    public int MessageCountGet()
    {
        return (this._pop3Client.MessageCount);
    }

    /// <summary>
    /// Returns the total message size .
    /// </summary>
    /// <returns>The message count number.</returns>
    public int MessageSizeGet()
    {
        return (this._pop3Client.TotalSize);
    }

    #endregion

    /// <summary>
    /// It does check if the client is still connected.
    /// The attributes/properties are not useful.
    /// A 'noop' command is used instead to be sure.
    /// +StatusConnected: its value is mantained using 
    /// </summary>
    /// <returns></returns>
    DateTime dtLast = DateTime.Now;
    Boolean bLast = false;
    TimeSpan tsTrigger = new TimeSpan(0, 0, 0, 0, 500);
    public Boolean IsMailServerReady()
    {


        ErrorMessage = "";

        if (!StatusConnected)
            return (false);
        if (_pop3Client == null)
            return (false);
        if (!_pop3Client.IsConnected)
            return (false);
        if (_pop3Client.Client == null)
            return (false);
        if (!_pop3Client.Client.Connected)
            return (false);

        try
        {
            if (((TimeSpan)(DateTime.Now - dtLast)) > tsTrigger)
            {
                _pop3Client.Noop();
                dtLast = DateTime.Now;
                bLast = true;
            }
            else
            {
                /* Take the last (current) value */
            }
        }
        catch (Exception ex)
        {
            //Allienamento log - Ciro
            if (ex.GetType() != typeof(ManagedException))
            {
                ManagedException mEx = new ManagedException(
                "Errore: " + ex.Message,
                "POP3_ERR_008",
                string.Empty,
                 "Casella mail: " + ((AccountInfo.LoginId != null) ? AccountInfo.LoginId : " vuoto. "), //EnanchedInfo
                ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                log.Error(err);
            }
            ErrorMessage = ex.Message;
            bLast = false;
        }
        return (bLast);
    } // Boolean IsMailServerReady(...


    /// <summary>
    ///  Recupera tutti i messaggi presenti sul server di posta
    ///  non usare mai, genera un traffico enorme e scarica i dati che potrebbero essere privati dell'utente!!!!!
    /// </summary>
    /// <returns>A list of messages</returns>
    public List<Message> RetrieveMessages()
    {
        List<Message> ltemp = new List<Message>();
        Message msg = null;
        int messageCount = this._pop3Client.MessageCount;
        //messageCount = (int)messageCount / 3;

        ErrorMessage = "";
        for (int i = 1; i <= messageCount; i = i + 1)
        {
            msg = this._pop3Client.RetrieveMessageObject(i);
            if (msg != null)
                ltemp.Add(msg);
        }

        return ltemp;
    }


    /// <summary>
    /// Executes the UIDL command and gets the list of <id,uid> from Mail Server
    /// </summary>
    /// <returns></returns>
    public List<Pop3Client.PopServerUniqueId> RetrieveUIds()
    {

        ErrorMessage = "";
        try
        {
            if (IsMailServerReady())
                return (this._pop3Client.GetUniqueIds());
            else
                return (new List<Pop3Client.PopServerUniqueId>());
        }
        catch (Exception ex)
        {
            //Allienamento log - Ciro
            if (ex.GetType() != typeof(ManagedException))
            {
                ManagedException mEx = new ManagedException(
                "Errore per scaricare gli UID dal server",
                "POP3_ERR_003",
                string.Empty,
                 "Casella mail: " + ((AccountInfo.LoginId != null) ? AccountInfo.LoginId : " vuoto. "), //EnanchedInfo
                ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                log.Error(err);
                ErrorMessage = ex.Message;
            }
            return (null);
            //ManagedException mEx = new ManagedException(
            //    "Errore per scaricare gli UID dal server",
            //    "POP3_ERR_003",
            //    "Pop3Controller",
            //    "RetrieveUIds()",
            //    "Ottiene gli UID dal server",
            //    (AccountInfo == null) ? "" : AccountInfo.LoginId,
            //    null,
            //    ex);
            //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
            //log.Error(err);
            //ErrorMessage = ex.Message;
            //return (null);
        }
    }


    /// <summary>
    /// Retrieves the mail headers for POP protocol.
    /// </summary>   
    /// <returns>The mail headers. If a MailHeader has uid="*", that element is no more available in te server.</returns>
    public List<MailHeader> RetrieveHeaders(int pidxStart, int piCount)
    {
        int MessageCount;
        int MessageSize;
        int iCount;
        List<MailHeader> lstRetVal;


        ErrorMessage = "";
        try
        {
            do
            {
                if (!IsMailServerReady())
                    return (new List<MailHeader>());

                MessageCount = this._pop3Client.MessageCount;
                MessageSize = this._pop3Client.TotalSize;

                iCount = 0;
                lstRetVal = new List<MailHeader>();
                for (int i = pidxStart; i <= MessageCount; i = i + 1)
                {
                    Header header;

                    header = this._pop3Client.RetrieveHeaderObject(i);
                    if (header != null)
                    {
                        MailHeader mailHeader = new MailHeader();

                        mailHeader.Index = i.ToString();
                        mailHeader.FillHeader(header);
                        mailHeader.UniqueId = this._pop3Client.GetUniqueId(i);
                        lstRetVal.Add(mailHeader);
                        if (++iCount >= piCount)
                            break;
                    }
                    else
                    {
                        MailHeader mailHeader = new MailHeader();

                        mailHeader.Index = i.ToString();
                        mailHeader.UniqueId = "*";
                        lstRetVal.Add(mailHeader);
                    }
                }

                this._pop3Client.UpdateStats();
                if (MessageCount == this._pop3Client.MessageCount)
                    if (MessageSize == this._pop3Client.TotalSize)
                        return lstRetVal;

            } while (true);
        }
        catch (Exception ex)
        {
            //Allienamento log - Ciro
            if (ex.GetType() != typeof(ManagedException))
            {
                ManagedException mEx = new ManagedException(
                "Errore durante la lettura degli headers dal server",
                "POP3_ERR_004",
                string.Empty,
                "Casella mail: " + ((AccountInfo.LoginId != null) ? AccountInfo.LoginId : " vuoto. "), //EnanchedInfo
                ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                log.Error(err);
            }
            ErrorMessage = ex.Message;
            //ManagedException mEx = new ManagedException(
            //    "Errore durante la lettura degli headers dal server",
            //    "POP3_ERR_004",
            //    "Pop3Controller",
            //    "RetrieveHeaders(int pidxStart, int piCount)",
            //    "Scarica gli headers dal server",
            //    (AccountInfo == null) ? "" : AccountInfo.LoginId,
            //    null,
            //    ex);
            //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
            //log.Error(err);
            //ErrorMessage = ex.Message;
        }

        return (null);
    } // List<MailHeader> RetrieveHeaders(...


    /// <summary>
    /// recupera gli header dei messaggi presenti sul server di posta
    /// </summary>   
    /// <returns>The mail headers</returns>
    public List<MailHeader> RetrieveHeaders()
    {
        return (RetrieveHeaders(1, this._pop3Client.MessageCount));
    } // List<Header> RetrieveHeaders()


    /// <summary>
    /// Cancella un messaggio della inbox del server di posta elettronica
    /// </summary>
    /// <param name="messageId">L'ID del messaggio</param>
    public void DeleteMessage(string messageId)
    {
        int index = 0;
        int messageCount = this._pop3Client.MessageCount;


        ErrorMessage = "";
        try
        {
            if (!IsMailServerReady())
                return;

            for (int i = 1; i <= messageCount; i++)
            {
                Header header = this._pop3Client.RetrieveHeaderObject(i);
                if (header.MessageId.Equals(messageId))
                {
                    index = i;
                    break;
                }
            }

            if (index > 0)
            {
                this._pop3Client.DeleteMessage(index);
            }
        }
        catch (Exception ex)
        {
            //Allienamento log - Ciro
            if (ex.GetType() != typeof(ManagedException))
            {
                ManagedException mEx = new ManagedException(
                "Errore nel cancellare il messaggio dal server",
                "POP3_ERR_005",
                string.Empty,
                "Message Id: " + messageId + " Casella mail: " + ((AccountInfo.LoginId != null) ? AccountInfo.LoginId : " vuoto. "), //EnanchedInfo
                ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                log.Error(err);
            }
            ErrorMessage = ex.Message;
            //ManagedException mEx = new ManagedException(
            //    "Errore per cancellare il messaggio dal server",
            //    "POP3_ERR_005",
            //    "Pop3Controller",
            //    "DeleteMessage(string messageId)",
            //    "Cancella il messaggio dal server",
            //    (AccountInfo == null) ? "" : AccountInfo.LoginId,
            //    "Message Id: " + messageId,
            //    ex);
            //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
            //log.Error(err);
            //ErrorMessage = ex.Message;
        }
    }

    /// <summary>
    /// Cancella un messaggio della inbox del server di posta elettronica
    /// </summary>
    /// <param name="UId">UID del messaggio</param>
    public void DeleteMessageByUID(string UId)
    {
        {
            int index = 0;
            int messageCount = this._pop3Client.MessageCount;


            ErrorMessage = "";
            try
            {
                if (!IsMailServerReady())
                    return;

                for (int i = 1; i <= messageCount; i++)
                {
                    string mUID = null;
                    try
                    {
                        mUID = this._pop3Client.GetUniqueId(i);
                    }
                    catch
                    {

                    }
                    if (!String.IsNullOrEmpty(mUID))
                    {
                        if (mUID.Equals(UId))
                        {
                            index = i;
                            break;
                        }
                    }
                }

                if (index > 0)
                {
                    this._pop3Client.DeleteMessage(index);
                }
            }
            catch (Exception ex)
            {
                //Allienamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(
                    "Errore nel cancellare il messaggio dal server",
                    "POP3_ERR_006",
                    string.Empty,
                    "Message Unique Id: " + UId + " Casella mail: " + ((AccountInfo.LoginId != null) ? AccountInfo.LoginId : " vuoto. "), //EnanchedInfo
                    ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
                ErrorMessage = ex.Message;

                //ManagedException mEx = new ManagedException(
                //    "Errore nel cancellare il messaggio dal server",
                //    "POP3_ERR_006",
                //    "Pop3Controller",
                //    "DeleteMessageByUID(string UId)",
                //    "Cancella il messaggio dal server",
                //    (AccountInfo == null) ? "" : AccountInfo.LoginId,
                //    "Message Unique Id: " + UId,
                //    ex);
                //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
                //log.Error(err);
                //ErrorMessage = ex.Message;
            }
        }
    }

    /// <summary>
    /// Recupera un messaggio per ID dal Server d Posta elettronica
    /// </summary>
    /// <param name="id">L'ID del messaggio</param>
    /// <returns>messaggio di Posta elettronica</returns>
    public Message getMessage(string id)
    {

        ErrorMessage = "";

        try
        {
            if (!IsMailServerReady())
                return (null);

            return getMessageByOrdinal(int.Parse(id));
        }
        catch (Exception ex)
        {
            //Allienamento log - Ciro
            if (ex.GetType() != typeof(ManagedException))
            {
                ManagedException mEx = new ManagedException(
                "Errore nel recupero del messaggio dal server",
                "POP3_ERR_009",
                string.Empty,
                "Casella mail: " + ((AccountInfo.LoginId != null) ? AccountInfo.LoginId : " vuoto. "), //EnanchedInfo
                ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                log.Error(err);
            }
            ErrorMessage = ex.Message;
            return (null);
        }
    }


    /// <summary>
    /// Gets a message through its (server-)index (aka Ordinal) ( 1 based )
    /// </summary>
    /// <param name="id">The index</param>
    /// <returns>The message body</returns>
    public Message getMessageByOrdinal(int piOrdinal)
    {
        Message msgRetVal = null;
        ErrorMessage = "";

        try
        {
            if (!IsMailServerReady())
                return (msgRetVal);

            if (piOrdinal < 1)
                return (msgRetVal);
            else
            {
                msgRetVal = this._pop3Client.RetrieveMessageObject(piOrdinal);
                if (msgRetVal != null)
                    msgRetVal.Uid = Pop3Client.GetUniqueId(piOrdinal);
                return (msgRetVal);
            }
        }
        catch (Exception ex)
        {
            //Allienamento log - Ciro
            if (ex.GetType() != typeof(ManagedException))
            {
                ManagedException mEx = new ManagedException(
                "Errore nel recupero del messaggio dal server",
                "POP3_ERR_007",
                string.Empty,
                "Message Ordinal: " + piOrdinal + " Casella mail: " + ((AccountInfo.LoginId != null) ? AccountInfo.LoginId : " vuoto. "), //EnanchedInfo
                ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                log.Error(err);
            }
            return (null);
            //ManagedException mEx = new ManagedException(
            //    "Errore nel recupero del messaggio dal server",
            //    "POP3_ERR_007",
            //    "Pop3Controller",
            //    "getMessageByOrdinal(int piOrdinal)",
            //    "Ottiene il messaggio dal server",
            //    (AccountInfo == null) ? "" : AccountInfo.LoginId,
            //    "Message Ordinal: " + piOrdinal,
            //    ex);
            //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
            //log.Error(err);
            //return (null);
        }
    }

    #endregion
}

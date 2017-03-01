using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using log4net;
using System.Collections;

    /// <summary>
    /// Message Unique Id.
    /// </summary>
    public class MessageUniqueId 
        {
        private int             i;
        private String          s;


        public int Ordinal
            {
            get { return( i );              }
            set { i = value;                }
            }        

        public String UId
            {
            get { return( s );              }
            set { s = value;                }
            }        

        public MessageUniqueId()
            {

            }

        public MessageUniqueId( int ordinal, String uid )
            {

            i               = ordinal;
            s               = uid;
            }

        } // class MessageUniqueId


/// <summary>
/// Summary description for ImapController
/// </summary>
public class ImapController
{

    private static readonly ILog log = LogManager.GetLogger("ImapController");
    #region Attributes

    /// <summary>
    /// Attribute for make connection to IMAP.
    /// </summary>
    private Imap4Client _imap4Client;

    /// <summary>
    /// Attributes for store messages and headers respectively.
    /// </summary>
    
    public  String          ErrorMessage;

    #endregion

    #region Properties

   

    /// <summary>
    /// Represents the Imap Client
    /// </summary>
    public Imap4Client Imap4Client
    {
        get { return _imap4Client; }
        set { _imap4Client = value; }
    }

   
    #endregion

    #region Constructor

    /// <summary>
    /// The Constructor
    /// </summary>
    public ImapController()
    {
       
    }

    #endregion

    #region Methods

    /// <summary>
    /// IncomingConnect the imap client.
    /// </summary>
    /// <param name="accountInfo">The information account</param>
    public Boolean Connect(MailUser accountInfo)
        {
        Boolean bRetVal         = false;


        ErrorMessage            = "";
        if (this._imap4Client == null || !this._imap4Client.IsConnected)
            {
            if (accountInfo != null) // && accountInfo.IncomingProtocol == IncomingProtocols.POP3)
                {
                this._imap4Client               = new Imap4Client();
                this._imap4Client.Disconnected += new DisconnectedEventHandler( On_ImapClientDisconnected );
                this._imap4Client.Connected    += new ConnectedEventHandler( On_ImapClientConnected );

                int     port            = accountInfo.PortIncomingServer;
                bool    ssl             = accountInfo.IsIncomeSecureConnection;
                string  serverName      = accountInfo.IncomingServer;
                string  user            =accountInfo.LoginId;
                string  password        =accountInfo.Password;
                bool    useInPort       = accountInfo.PortIncomingChecked;

                try
                    {
                    if (ssl)
                        {
                        if (useInPort)
                            {
                            this._imap4Client.ConnectSsl(serverName, port);
                            }
                          else
                            {
                            this._imap4Client.ConnectSsl(serverName);
                            }
                        }
                      else
                        {
                        if (useInPort)
                            {
                            this._imap4Client.Connect(serverName, port);
                            }
                          else
                            {
                            this._imap4Client.Connect(serverName);
                            }
                        }

                    this._imap4Client.Login(user, password);
                    bRetVal             = true;
                    }
                catch( Exception ex )
                    {
                    ErrorMessage        = ex.Message;
                    StatusConnected     = false;
                    bRetVal             = false;
                    }
                }
            }
        return( bRetVal );
        } // Boolean Connect(...

    /// <summary>
    /// IncomingDisconnect the imap client.
    /// </summary>
    public void Disconnect()
        {


        ErrorMessage        = "";
        try
            {
            if ( StatusConnected || ( this._imap4Client != null && this._imap4Client.IsConnected ) )
                this._imap4Client.Disconnect();
            }
        catch( Exception ex )
            {
                log.Error("ImapController Metodo: Disconnect: ", ex);
                ErrorMessage        = ex.Message;
            }
        } // void Disconnect()


#region StatusConnected

    private bool   StatusConnected      = false;

    public void On_ImapClientDisconnected( Object sender, EventArgs ea )
        {

        StatusConnected      = false;
        } // void On_ImapClientDisconnected(...

    public void On_ImapClientConnected( Object sender, EventArgs ea )
        {

        StatusConnected      = true;
        } // void On_ImapClientConnected(...
#endregion

    /// <summary>
    /// It does check if the client is still connected.
    /// The attributes/properties are not useful.
    /// A 'noop' command is used instead to be sure.
    /// </summary>
    /// <returns></returns>
    public  Boolean                 CanIReceive()
        {


        ErrorMessage        = "";
        if ( ! StatusConnected )
            return( false );
        if ( Imap4Client == null )
            return( false );
        if ( ! Imap4Client.IsConnected )
            return( false );
        if ( Imap4Client.Client == null )
            return( false );
        if ( ! Imap4Client.Client.Connected )
            return( false );

        try {
            Imap4Client.Check();
            }
        catch ( Exception ex )
            {
                log.Error("ImapController Metodo: CanIReceive: ", ex);
            ErrorMessage        = ex.Message;
            return( false );
            }

        return( true );
        } // Boolean IsMailServerReady(...


    /// <summary>
    /// Method for retrieve the mail messages for IMAP protocol.
    /// </summary>
    /// <param name="mailBox">The mail box</param>
    /// <returns>The mail messages</returns>
    public List<Message> RetrieveMessages( string mailBox )
        {
        List<Message> lstRetVal = new List<Message>();
        Message msg = null;        

        ErrorMessage        = "";
        try
            {
            if ( ! CanIReceive() )
                return( null );
            Mailbox inbox       = this._imap4Client.SelectMailbox(mailBox);
            Fetch fetch         = inbox.Fetch;
            int messageCount    = inbox.MessageCount;
         // messageCount        = (int)messageCount / 3;

            for (int i = 1; i <= messageCount; i++)
                {
                msg = fetch.MessageObject(i);            
                lstRetVal.Add(msg);
                }
            }
        catch ( Exception ex )
            {
                log.Error("ImapController Metodo: RetrieveMessages: ", ex);
            ErrorMessage        = ex.Message;
            lstRetVal.Clear();
            }
        return lstRetVal;
        } // List<Message> RetrieveMessages(...


    /// <summary>
    /// This method retrieves the mail messages using the IMAP protocol.
    /// </summary>
    /// <param name="psMailBoxName"></param>
    /// <param name="piStart"></param>
    /// <param name="piCount"></param>
    /// <returns>A list of Header object(s)</returns>
    public List<MailHeader> RetrieveHeaders( String psMailBoxName, int piStart, int piCount )
        {
        int                     iCount;
        List<MailHeader>        lstRetVal;


        do {
            ErrorMessage            = "";
            lstRetVal               = new List<MailHeader>();
            try
                {
                Mailbox inbox           = this._imap4Client.SelectMailbox( psMailBoxName );
                Fetch fetch             = inbox.Fetch;
                int messageCount        = inbox.MessageCount;

                iCount                  = 0;
                for (int i = piStart; ( ( i <= messageCount )  &&  ( iCount <= piCount ) ); i++)
                    {
                    MailHeader mailHeader   = new MailHeader();
                    Header header           = fetch.HeaderObject(i);
                    String uid              = fetch.Uid(i).ToString();
                    mailHeader.Index        = i.ToString();
                    mailHeader.FillHeader( header );
                    mailHeader.UniqueId     = uid;
                    lstRetVal.Add( mailHeader );
                    if ( ++iCount >= piCount )
                        break;
                    }

                fetch                   = inbox.Fetch;
                if ( messageCount == this._imap4Client.SelectMailbox( psMailBoxName ).MessageCount )
                    break;
                }
            catch ( Exception ex )
                {
                    log.Error("ImapController Metodo: RetrieveHeaders: ", ex);
                ErrorMessage            = ex.Message;
                lstRetVal.Clear();
                return( lstRetVal );
                }
            } while ( true );

        return( lstRetVal );
        }


    /// <summary>
    /// Method for retrieve the mail headers for IMAP protocol.
    /// </summary>
    /// <param name="mailBox">The mail box</param>
    /// <returns>The mail headers</returns>
    public List<Header> RetrieveHeaders(string mailBox)
        {
        List<Header>        lstRetVal       = new List<Header>();


        ErrorMessage        = "";      
        try
            {
            Mailbox inbox = this._imap4Client.SelectMailbox(mailBox);
            Fetch fetch = inbox.Fetch;
            int messageCount = inbox.MessageCount;

            for (int i = 1; i <= messageCount; i++)
                {
                Header header       = new Header();
                header              = fetch.HeaderObject(i);

                header.IndexOnServer= i;
         //LR// header.Uid          = this._imap4Client.GetUniqueId(i);
                lstRetVal.Add( header );            
                }
            }
        catch ( Exception ex )
            {
                log.Error("ImapController Metodo: RetrieveHeaders: ", ex);
            ErrorMessage        = ex.Message;
            return null;       
            }
        return( lstRetVal );
        }



    public List<MessageUniqueId> RetrieveUIds( String psMailBoxName )
        {
        List<MessageUniqueId>      lstMUId         = null;


        ErrorMessage        = "";
        try
            {
            Mailbox inbox = this._imap4Client.SelectMailbox( psMailBoxName );

            //Fetch fetch = inbox.Fetch;
            //int messageCount = inbox.MessageCount;

//http://stackoverflow.com/questions/5015236/imap-getting-uid-list-of-selected-folder
//The easiest (and most compact) way to list all the UIDs in the currently-selected folder is via UID SEARCH ALL:
//A001 UID SEARCH ALL
//* SEARCH 288 291 292 293 295 323 324 325 326 327 385 387 472 474 641 720 748
//A001 OK UID SEARCH completed
//The errors in your UID FETCH request were leaving out the sequence-set and including a list of flags. If you rewrote it as
//A002 UID FETCH 1:* (UID)
//or
//A002 UID FETCH 1:* (UID FLAGS)
//it would work.

            String s = Imap4Client.Command( "UID SEARCH ALL" );
            if ( ! s.StartsWith( "+Ok" ) )
                {
                ErrorMessage        = "Problems retrieving...UID List";
                lstMUId.Clear();       
                }
            }
        catch ( Exception ex )
            {
                log.Error("ImapController Metodo: RetrieveUIds: ", ex);
            ErrorMessage        = ex.Message;
            lstMUId.Clear();       
            }
        return( lstMUId );
        }


    /// <summary>
    /// Gets a message through its MessageId
    /// </summary>
    /// <param name="id">The Message id</param>
    /// <returns>A message</returns>
    public Message getMessage(string id)
        {
            throw new Exception("to be implemented");
        }


    /// <summary>
    /// Gets a message through its ordinal value ( index of Mail Server for that message )
    /// </summary>
    /// <param name="id">The index into the Mail Server</param>
    /// <returns>A message</returns>
    public Message getMessageByOrdinal( int piOrdinal )
        {


        ErrorMessage        = "";
        try
            {
            Mailbox inbox = this._imap4Client.SelectMailbox("inbox");
            Fetch fetch = inbox.Fetch;
            return fetch.MessageObject( piOrdinal );
            }
        catch ( Exception ex )
            {
                log.Error("ImapController Metodo: getMessageByOrdinal: ", ex);
            ErrorMessage        = ex.Message;
            return( null );
            }
        }

    #endregion
    }

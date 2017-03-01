using System;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using log4net;


/// <summary>
/// This controller is used for send
/// mail messages for SMTP protocol
/// </summary>
/// 
public class SmtpController
{
    #region Constructor

	public SmtpController()
	{
    }

    #endregion

    private static readonly ILog log = LogManager.GetLogger("SmtpController");
    #region Method

    /// <summary>
    /// Method used for send a message.
    /// </summary>
    /// <param name="message">The message will be sent.</param>  
    /// <param name="acc">The account information.</param>       
    public void SendMail(Message message, MailUser acc)
    {
        MailUser arrayAcc_info = acc;

        try
        {
            if (arrayAcc_info != null)
            {
                //LR// message.From.Email = arrayAcc_info.EmailAddress;
                string outgoing = arrayAcc_info.OutgoingServer;
                int smtp_Port = arrayAcc_info.PortOutgoingServer;
                String                  ogServer;       // OutgoingServer
                System.Net.IPAddress    IPAddr;
                System.Net.IPHostEntry  IPHostEntry;
                //TODO:COMMENTETO ENCRYPT DECRYPT
                string email = null;// EncryptDecrypt.CryptDecrypt(arrayAcc_info.EmailAddress);
                string password = null;// EncryptDecrypt.CryptDecrypt(arrayAcc_info.Password);

                bool ssl = arrayAcc_info.IsOutgoingSecureConnection;
                bool port = arrayAcc_info.PortOutgoingChecked;

                //LR// Now ... We can!
                ogServer = acc.OutgoingServer;
                if ( System.Net.IPAddress.TryParse( ogServer, out IPAddr ) )
                    {
                    IPHostEntry = System.Net.Dns.GetHostEntry( ogServer );
                    ogServer = IPHostEntry.HostName;
                    }
                if (!(acc.EmailAddress.ToUpper().Contains("PEC")))
                {
                    message.From.Email = acc.EmailAddress + "@" + arrayAcc_info.Dominus;
                }
                else { message.From.Email = acc.EmailAddress; }
                email = message.From.Email;
                password = acc.Password;

                if (ssl)
                {
                     
                        ActiveUp.Net.Mail.SmtpClient.SendSsl(message, outgoing, smtp_Port, email, password, ActiveUp.Net.Mail.SaslMechanism.Login);
                    
                    //else
                    //{
                    //    ActiveUp.Net.Mail.SmtpClient.SendSsl(message, outgoing, email, password, ActiveUp.Net.Mail.SaslMechanism.Login);
                    //}
                }
                else
                {
                    if (port)
                    {
                        ActiveUp.Net.Mail.SmtpClient.Send(message, outgoing, smtp_Port, email, password, ActiveUp.Net.Mail.SaslMechanism.Login);
                    }
                    else
                    {
                        //LR// ActiveUp.Net.Mail.SmtpClient.SendSsl(message, outgoing, email, password, ActiveUp.Net.Mail.SaslMechanism.Login);
                        // modifica per sistema autenticazione server smtp mail.comune.roma.it NR
                        ActiveUp.Net.Mail.SmtpClient.Send(message, outgoing, acc.EmailAddress, password, ActiveUp.Net.Mail.SaslMechanism.Login);
                    }
                }
                this.storeMessageSent(message);
            }
        }
        catch ( Exception ex )
            {
                ManagedException mex = new ManagedException(ex.Message, "MAIL_001", "SendMail", ex.Source, ex.InnerException);
                log.Error(mex);
            throw mex;
            }
    }


    private void storeMessageSent(Message msg)
    {
        //msg.StoreToFile();
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail.DeltaExt;
using System.Web;
using ActiveUp.Net.Mail;
using System.Text.RegularExpressions;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Web;
using log4net;


namespace Com.Delta.Mail.MailMessage
{
    public class MailMessageComposer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MailMessageComposer));

        //public const string MAIL_VALIDATION_PATTERN = @"^[_a-zA-Z0-9-]+(\.[_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(([0-9]{1,3})|([a-zA-Z]{2,4}))$";
        // forse quella qui sotto va bene - Luigi
        //public const string MAIL_VALIDATION_PATTERN = "^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";
        /* Aggiornamento della regex Ciro - 16/02/2016
        public const string MAIL_VALIDATION_PATTERN = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
         * Nuovo codice */
        //public const string MAIL_VALIDATION_PATTERN = @"^\w+([!#$%&'*+-/=?^_{|}~.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public static readonly string MAIL_VALIDATION_PATTERN = RegexUtils.EMAIL_REGEX.ToString();


        public Message message
        {
            get { return (Message)HttpContext.Current.Session[WebMailClientManager.SessionKeys.MAIL_CURRENTSENDMAIL.ToString()]; }
            set
            {
                if (!CurrentSendMailExist())
                    HttpContext.Current.Session[WebMailClientManager.SessionKeys.MAIL_CURRENTSENDMAIL.ToString()] = value;
                else
                    HttpContext.Current.Session.Add(WebMailClientManager.SessionKeys.MAIL_CURRENTSENDMAIL.ToString(), value);
            }
        }

        public MailMessageComposer CurrentSendMail_NEW()
        {
            CurrentSendMailClear();
            Message m = new Message();
            m.Date = System.DateTime.Now;
            message = m;
            return this;

        }

        public static Message CurrentSendMailGet()
        {
            if (HttpContext.Current.Session[WebMailClientManager.SessionKeys.MAIL_CURRENTSENDMAIL.ToString()] != null)
                return (Message)HttpContext.Current.Session[WebMailClientManager.SessionKeys.MAIL_CURRENTSENDMAIL.ToString()];
            else return null;
        }

        public static bool CurrentSendMailExist()
        {
            if (HttpContext.Current.Session[WebMailClientManager.SessionKeys.MAIL_CURRENTSENDMAIL.ToString()] == null) return false;
            else return true;
        }

        public static void CurrentSendMailClear()
        {
            HttpContext.Current.Session.Remove(WebMailClientManager.SessionKeys.MAIL_CURRENTSENDMAIL.ToString());
        }

        public MailMessageComposer CurrentSendMail_TO_Add(string to)
        {
            to = to.Trim();
            to.Replace(" ", "");
            string[] toArray = to.Split(new char[] { ',', ';' });

            for (int i = 0; i < toArray.Length; i++)
                if (toArray[i] != string.Empty)
                    if (validateMailAddress(toArray[i]) == false)
                    {
                        ManagedException mEx = new ManagedException("La mail " + toArray[i] + "non è una mail valida",
                            "ERR_MMSG01",
                            string.Empty,
                            string.Empty,
                            null);
                        ErrorLogInfo error = new ErrorLogInfo(mEx);
                        log.Error(error);
                        throw mEx;
                    }
                        //throw new ManagedException("La mail " + toArray[i] + "non è una mail valida", "ERR_MF_001", null, null, null, null, null, null);


            foreach (string s in toArray)
                if (s != string.Empty)
                    message.To.Add(s);
            return this;
        }

        public MailMessageComposer CurrentSendMail_CC_Add(string cc)
        {
            cc = cc.Trim();
            cc.Replace(" ", "");
            string[] toArray = cc.Split(new char[] { ',', ';' });

            for (int i = 0; i < toArray.Length; i++)
                if (toArray[i] != string.Empty)
                    if (validateMailAddress(toArray[i]) == false)
                    {
                        ManagedException mEx = new ManagedException("La mail " + toArray[i] + "non è una mail valida",
                            "ERR_MMSG02",
                            string.Empty,
                            string.Empty,
                            null);
                        ErrorLogInfo error = new ErrorLogInfo(mEx);
                        log.Error(error);
                        throw mEx;
                    }
                        //throw new ManagedException("La mail " + toArray[i] + "non è una mail valida", "ERR_MF_001", null, null, null, null, null, null);


            foreach (string s in toArray)
                if (s != string.Empty)
                    message.Cc.Add(s);
            return this;
        }

        public MailMessageComposer CurrentSendMail_BODY(string body)
        {
            message.BodyHtml.Text = "<html><body>" + body + "</body></html>";
            return this;
        }

        public MailMessageComposer CurrentSendMail_BODYTEXT(string body)
        {
            message.BodyText.Text = body;
            return this;
        }

        public MailMessageComposer CurrentSendMail_FROM(string from)
        {
            if (validateMailAddress(from))
            {
                message.From.Email = from;
                return this;
            }
            else throw new FormatException();
        }

        public MailMessageComposer CurrentSendMail_SUBJECT(string subject)
        {
            message.Subject = subject;
            return this;
        }

        public MailMessageComposer CurrentSendMail_ATTACHMENT_Add(string name, byte[] doc)
        {
            message.Attachments.Add(new MimePart(doc, name));
            return this;
        }

        public static bool ValidateMails(ref string mails)
        {
            mails = Regex.Replace(mails.Trim(), "([ \t])*([,;])+([ \t])*", ";");
            string[] mail = mails.Split(';', ',');
            bool valid = true;
            for (int j = 0, n = mail.Length; j < n && valid; j++)
            {
                valid = validateMailAddress(mail[j]);
            }
            return valid;
        }

        public static bool validateMailAddress(string mail)
        {
            Match mailMatch = Regex.Match(mail, MailMessageComposer.MAIL_VALIDATION_PATTERN);
            if (mailMatch.Success && mailMatch.Value.Equals(mail))
                return true;
            else return false;
        }


    }
}

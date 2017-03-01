using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Mail;
using System.Text.RegularExpressions;
using ActiveUp.Net.Common.DeltaExt;

namespace Com.Delta.Mail.MailMessage
{
    public class WebMailClientManager
    {
        public enum CacheKeys
        {
            MAIL_SERVERS
        }

        public enum SessionKeys
        {
            MAIL_CURRENTMAIL,
            MAIL_CURRENTSENDMAIL,
            MAIL_ACCOUNT,
            MAIL_CURRENTFOLDER,
            MAIL_PARENTFOLDER
        }

        public const string MAIL_VALIDATION_PATTERN = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

        #region "Account"
        
        public static bool AccountIsValid()
        {
            MailUser m = (MailUser)HttpContext.Current.Session[SessionKeys.MAIL_ACCOUNT.ToString()];
            if (m == null) return false;
            if (m.Validated) return true;
            else return false;
        }

        public static bool AccountExist()
        {
            if (HttpContext.Current.Session[SessionKeys.MAIL_ACCOUNT.ToString()] != null) return true;
            else return false;
        }

        public static MailUser getAccount()
        {
            return (MailUser)HttpContext.Current.Session[SessionKeys.MAIL_ACCOUNT.ToString()];
        }

        public static void AccountRemove()
        {
            HttpContext.Current.Session.Remove(SessionKeys.MAIL_ACCOUNT.ToString());
        }

        public static void SetAccount(MailUser u)
        {
            MailUser m = (MailUser)HttpContext.Current.Session[SessionKeys.MAIL_ACCOUNT.ToString()];
            if (m == null) HttpContext.Current.Session.Add(SessionKeys.MAIL_ACCOUNT.ToString(), u);
            else if (m != u)
            {
                HttpContext.Current.Session.Add(SessionKeys.MAIL_ACCOUNT.ToString(), u);
            }
        }

        #endregion

        #region "Current Mail"
        
        public static void CurrentMailSet(Message mail)
        {
            if (CurrentMailExist())
            {
                HttpContext.Current.Session[SessionKeys.MAIL_CURRENTMAIL.ToString()] = null;
                HttpContext.Current.Session[SessionKeys.MAIL_CURRENTMAIL.ToString()] = mail;
            }
            else
                HttpContext.Current.Session.Add(SessionKeys.MAIL_CURRENTMAIL.ToString(), mail);

        }

        public static bool CurrentMailExist()
        {
            if (HttpContext.Current.Session[SessionKeys.MAIL_CURRENTMAIL.ToString()] == null) return false;
            else return true;
        }
        
        //public static bool CurrentMailIsRoot()
        //{
        //    if (CurrentMailExist() && CurrentMailGet().Container == null)
        //        return true;
        //    else
        //        return false;
        //}

        public static bool CurrentMailIsRoot()
        {
            if (CurrentMailExist() && CurrentMailGet().SubMessages.Count > 0)
                return true;
            else
                return false;
        }
        
        public static Message CurrentMailGet()
        {
            return (Message)HttpContext.Current.Session[SessionKeys.MAIL_CURRENTMAIL.ToString()];
        }

        public static void CurrentMailRemove()
        {
            HttpContext.Current.Session.Remove(SessionKeys.MAIL_CURRENTMAIL.ToString());
        }

        public static bool CurrentFolderExists()
        {
            if (HttpContext.Current.Session[SessionKeys.MAIL_CURRENTFOLDER.ToString()] == null) return false;
            else return true;
        }

        public static bool ParentFolderExists()
        {
            if (HttpContext.Current.Session[SessionKeys.MAIL_PARENTFOLDER.ToString()] == null) return false;
            else return true;
        }

        public static void CurrentFolderSet(string currFolder)
        {
             if (CurrentFolderExists())
               HttpContext.Current.Session[SessionKeys.MAIL_CURRENTFOLDER.ToString()] = currFolder;
            else 
              HttpContext.Current.Session.Add(SessionKeys.MAIL_CURRENTFOLDER.ToString(),currFolder);

        }

        public static string CurrentFolderGet()
        {
            if (CurrentFolderExists())
                return (string)HttpContext.Current.Session[SessionKeys.MAIL_CURRENTFOLDER.ToString()];
            else
                return "99";
        }

        public static string ParentFolderGet()
        {
            if (ParentFolderExists())
                return (string)HttpContext.Current.Session[SessionKeys.MAIL_PARENTFOLDER.ToString()];
            else
                return "99";
        }

        public static void ParentFolderSet(string parentFolder)
        {
            if (ParentFolderExists())
                HttpContext.Current.Session[SessionKeys.MAIL_PARENTFOLDER.ToString()] = parentFolder;
            else
                HttpContext.Current.Session.Add(SessionKeys.MAIL_PARENTFOLDER.ToString(), parentFolder);

        }

        //public static void CurrentFolderSet(MailFolder currFolder)
        //{
        //    if (CurrentFolderExists())
        //        HttpContext.Current.Session[SessionKeys.MAIL_CURRENTFOLDER.ToString()] = (int)currFolder;
        //    else 
        //        HttpContext.Current.Session.Add(SessionKeys.MAIL_CURRENTFOLDER.ToString(), (int)currFolder);
        //}

        //public static MailFolder CurrentFolderGet()
        //{
        //    if (CurrentFolderExists())
        //        return (MailFolder)HttpContext.Current.Session[SessionKeys.MAIL_CURRENTFOLDER.ToString()];
        //    else
        //        return MailFolder.Tutte;
        //}

        public static void CurrentFolderRemove()
        {
            HttpContext.Current.Session.Remove(SessionKeys.MAIL_CURRENTFOLDER.ToString());
        }

        #endregion

        #region "Current Send Mail"

        public static bool CurrentSendMailExist()
        {
            if (HttpContext.Current.Session[SessionKeys.MAIL_CURRENTSENDMAIL.ToString()] == null) return false;
            else return true;
        }
        
        public static Message CurrentSendMailGet()
        {
            return (Message)HttpContext.Current.Session[SessionKeys.MAIL_CURRENTSENDMAIL.ToString()];
        }
        
        public static void CurrentSendMailSet(Message mail)
        {
            if (CurrentSendMailExist())
                HttpContext.Current.Session[SessionKeys.MAIL_CURRENTSENDMAIL.ToString()] = mail;
            else
                HttpContext.Current.Session.Add(SessionKeys.MAIL_CURRENTSENDMAIL.ToString(), mail);
        }
        
        public static void CurrentSendMailClear()
        {
            HttpContext.Current.Session.Remove(SessionKeys.MAIL_CURRENTSENDMAIL.ToString());
        }

        public static void CurrentSendMail_NEW()
        {
            CurrentSendMailClear();
            Message m = new Message();
            m.Date = System.DateTime.Now;
            CurrentSendMailSet(m);
        }

        public static void CurrentSendMail_TO_Add(string to)
        {
            if (validateMailAddress(to))
            {
                Message message = CurrentSendMailGet();
                message.To.Add(to);
                CurrentSendMailSet(message);
            }
            else throw new FormatException();
        }

        public static void CurrentSendMail_CC_Add(string cc)
        {
            if (validateMailAddress(cc))
            {
                Message message = CurrentSendMailGet();
                message.Cc.Add(cc);
                CurrentSendMailSet(message);
            }
            else throw new FormatException();
        }

        public static void CurrentSendMail_BODY(string body)
        {
            Message message = CurrentSendMailGet();
            message.BodyHtml.Text = "<html><body>" + body + "</body></html>";
            CurrentSendMailSet(message);
        }

        public static void CurrentSendMail_FROM(string from)
        {
            if (validateMailAddress(from))
            {
                Message message = CurrentSendMailGet();
                message.From.Email = from;
                CurrentSendMailSet(message);
            }
            else throw new FormatException();
        }

        public static void CurrentSendMail_SUBJECT(string subject)
        {
            Message message = CurrentSendMailGet();
            message.Subject = subject;
            CurrentSendMailSet(message);
        }

        public static void CurrentSendMail_ATTACHMENT_Add(string name, byte[] doc)
        {
            Message message = CurrentSendMailGet();
            message.Attachments.Add(new MimePart(doc, name));
            CurrentSendMailSet(message);
        }

        #endregion

        public static bool validateMailAddress(string mail)
        {
            return Regex.IsMatch(mail, MAIL_VALIDATION_PATTERN);
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail.DeltaExt;

namespace Com.Delta.Mail.Facades
{
    public abstract class BaseFacade
    {
        protected Pop3Controller popController = new Pop3Controller();
        protected ImapController imapController = new ImapController();
        protected SmtpController smtpController = new SmtpController();

        protected MailUser _accSettings = null;

        protected List<MailHeader> _ListHeaders = null;
        protected DateTime _LastRefresh = System.DateTime.MinValue;

        protected string MAILBOX = "Inbox";
        protected string INBOX_QUERY = "inbox";
        protected string SENTMAIL_QUERY = "sentMail";
        protected string SENT_MAILS_FOLDER = "SentMails";

        public DateTime LastRefresh
        {
            get { return _LastRefresh; }
        }

        public MailUser AccSettings
        {
            get { return _accSettings; }
        }

    }
}

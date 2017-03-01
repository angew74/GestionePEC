using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Common.DeltaExt;

namespace ActiveUp.Net.Mail.DeltaExt
{
    /// <summary>
    /// Mail Account
    /// </summary>
    /// 
    [Serializable]
    public class MailUser : MailServer
    {
        decimal userId;
        string loginId;
        string emailAddress;
        string password;
        bool validated;
        string dominus;
        string casella;
        Nullable<int> flgManaged;
        List<Folder> folders;

        public MailUser()
        { }

        public MailUser(MailServer e)
            : base(e)
        {

        }

        public List<Folder> Folders
        {
            get { return folders; }
            set { folders = value; }
        }


        public decimal UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public new string Dominus
        {
            get { return dominus; }
            set { dominus = value; }
        }

        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string LoginId
        {
            get { return loginId; }
            set { loginId = value; }
        }

        public bool Validated
        {
            get { return validated; }
            set { validated = value; }
        }

        public string Casella
        {
            get { return casella; }
            set { casella = value; }
        }

        public bool IsManaged
        {
            get { return flgManaged.HasValue; }
        }

        public Nullable<int> FlgManaged
        {
            get { return flgManaged; }
            set { flgManaged = value; }
        }

        #region "Public Ods Methods"
        public MailUser selectUser()
        {
            return this;
        }

        public MailUser saveUser(MailUser e)
        {
            return e;
        }
        #endregion
    }
}

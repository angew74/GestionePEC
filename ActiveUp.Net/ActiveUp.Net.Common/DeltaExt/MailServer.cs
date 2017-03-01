using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveUp.Net.Mail.DeltaExt
{
    /// <summary>
    /// Classe per la gestione ...
    /// </summary>
    /// 
    [Serializable]
    public class MailServer
    {

        #region "C.tor"
        public MailServer()
        {

        }

        public MailServer(MailServer e)
        {

            this.Id = e.Id;
            this.DisplayName = e.DisplayName;

            this.OutgoingServer = e.OutgoingServer;
            this.PortIncomingServer = e.PortIncomingServer;
            this.PortOutgoingServer = e.PortOutgoingServer;
            this.IsIncomeSecureConnection = e.IsIncomeSecureConnection;
            this.IsOutgoingSecureConnection = e.IsOutgoingSecureConnection;
            this.IsOutgoingWithAuthentication = e.IsOutgoingWithAuthentication;
            this.PortIncomingChecked = e.PortIncomingChecked;
            this.PortOutgoingChecked = e.PortOutgoingChecked;
            this.IncomingProtocol = e.IncomingProtocol;
            this.IncomingServer = e.IncomingServer;
            this.Dominus = e.Dominus;
            this.isPec = e.IsPec;

        }
        #endregion

        #region "Private fields"
        private decimal id;
        private string displayName;
        private bool isOutgoingWithAuthentication;

        private string incomingServer;
        private string outgoingServer;

        private int portIncomingServer;
        private int portOutgoingServer;

        private bool isIncomeSecureConnection;
        private bool isOutgoingSecureConnection;

        private bool portIncomingChecked;
        private bool portOutgoingChecked;

        private string incomingProtocol;
        private string dominus;

        private bool isPec;
        #endregion

        #region "Public Properties"
        public decimal Id
        {
            get { return id; }
            set { id = value; }
        }

        public bool PortIncomingChecked
        {
            get { return portIncomingChecked; }
            set { portIncomingChecked = value; }
        }

        public string IncomingServer
        {
            get { return incomingServer; }
            set { incomingServer = value; }
        }

        public bool PortOutgoingChecked
        {
            get { return portOutgoingChecked; }
            set { portOutgoingChecked = value; }
        }

        public string IncomingProtocol
        {
            get { return incomingProtocol; }
            set { incomingProtocol = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public string OutgoingServer
        {
            get { return outgoingServer; }
            set { outgoingServer = value; }
        }

        public int PortIncomingServer
        {
            get { return portIncomingServer; }
            set { portIncomingServer = value; }
        }

        public int PortOutgoingServer
        {
            get { return portOutgoingServer; }
            set { portOutgoingServer = value; }
        }

        public bool IsIncomeSecureConnection
        {
            get { return isIncomeSecureConnection; }
            set { isIncomeSecureConnection = value; }
        }

        public bool IsOutgoingSecureConnection
        {
            get { return isOutgoingSecureConnection; }
            set { isOutgoingSecureConnection = value; }
        }

        public bool IsOutgoingWithAuthentication
        {
            get { return isOutgoingWithAuthentication; }
            set { isOutgoingWithAuthentication = value; }
        }

        public string Dominus
        {
            get { return dominus; }
            set { dominus = value; }
        }

        public bool IsPec
        {
            get { return isPec; }
            set { isPec = value; }
        }

        #endregion

        #region "Public Ods Methods"
        public MailServer selectServer()
        {
            return this;
        }

        public MailServer saveServer(MailServer e)
        {
            return e;
        }
        #endregion

        #region "Cloneable"
        public MailServer Clone()
        {
            MailServer s = new MailServer(this);
            return s;
        }
        #endregion
    }
}


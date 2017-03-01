using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Common.DeltaExt;

namespace SendMail.Model
{
    [Serializable()]
    public class MailRefs : IDomainObject
    {

        #region IDomainObject Membri di

        public bool IsValid
        {
            get { return (IdRef > 0 && RefIdMail > 0 && !String.IsNullOrEmpty(AddresseeMail)); }
        }

        public bool IsPersistent
        {
            get { return false; }
        }

        #endregion

        #region "Properties"

        [DatabaseField("ID_REF")]
        public virtual Int64 IdRef { get; set; }

        [DatabaseField("REF_ID_MAIL")]
        public Int64 RefIdMail { get; set; }

        [DatabaseField("MAIL_DESTINATARIO")]
        public String AddresseeMail { get; set; }

        [DatabaseField("TIPO_REF")]
        private String _AddresseeClass { get; set; }

        public AddresseeType AddresseeClass
        {
            get
            {
                if (Enum.IsDefined(typeof(AddresseeType), _AddresseeClass))
                {
                    return (AddresseeType)Enum.Parse(typeof(AddresseeType), _AddresseeClass);
                }
                else return AddresseeType.UNDEFINED;
            }
            set
            {
                _AddresseeClass = value.ToString();
            }
        }

        #endregion


        #region "C.tor"

        public MailRefs()
        {

        }

        public MailRefs(String addresseMail, AddresseeType addrclass)
        {
            this.IdRef = 0;
            this.RefIdMail = 0;

            this.AddresseeMail = addresseMail;
            this.AddresseeClass = addrclass;
        }

        #endregion
    }
}

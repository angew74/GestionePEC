using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Unisys.Pdf.ManagedModules.ModuleParts
{
    public class NullaOsta
    {

        private string codiceFiscaleField;

        private string cognomeField;

        private string nomeField;

        private string giornoNasField;

        private string meseNasField;

        private string annoNasField;

        private string sessoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceFiscale
        {
            get
            {
                return this.codiceFiscaleField;
            }
            set
            {
                this.codiceFiscaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Cognome
        {
            get
            {
                return this.cognomeField;
            }
            set
            {
                this.cognomeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Nome
        {
            get
            {
                return this.nomeField;
            }
            set
            {
                this.nomeField = value;
            }
        }

        /// <remarks/>
        public string DataDiNascita
        {
            get
            {
                string ddn = String.Format("{0}/{1}/{2}",
                                        GiornoNas,
                                        MeseNas,
                                        AnnoNas);
                // date parziali ammissibili ???
                return (ddn.Length != 10) ? string.Empty : ddn;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string GiornoNas
        {
            get
            {
                return (string.IsNullOrEmpty(this.giornoNasField)) ? string.Empty : (this.giornoNasField.Length == 1) ? "0" + this.giornoNasField : this.giornoNasField;
            }
            set
            {
                this.giornoNasField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MeseNas
        {
            get
            {
                return (string.IsNullOrEmpty(this.giornoNasField)) ? string.Empty : (this.meseNasField.Length == 1) ? "0" + this.meseNasField : this.meseNasField;
            }
            set
            {
                this.meseNasField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AnnoNas
        {
            get
            {
                return this.annoNasField;
            }
            set
            {
                this.annoNasField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Sesso
        {
            get
            {
                return this.sessoField;
            }
            set
            {
                this.sessoField = value;
            }
        }

    }
}

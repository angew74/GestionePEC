using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Unisys.Pdf.ManagedModules.ModuleParts
{
    public class Richiedente
    {

        private string tipoRichiedenteField;

        private string ragioneSocialeField;

        private string partitaIVAField;

        private string ufficioField;

        private string cognomeField;

        private string telefonoField;

        private string codiceFiscaleField;

        private string nomeField;

        /// <summary>
        /// "1" Privato; "2" Pubblico.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TipoRichiedente
        {
            get
            {
                return this.tipoRichiedenteField;
            }
            set
            {
                this.tipoRichiedenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RagioneSociale
        {
            get
            {
                return this.ragioneSocialeField;
            }
            set
            {
                this.ragioneSocialeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PartitaIVA
        {
            get
            {
                return this.partitaIVAField;
            }
            set
            {
                this.partitaIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Ufficio
        {
            get
            {
                return this.ufficioField;
            }
            set
            {
                this.ufficioField = value;
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
        public string Telefono
        {
            get
            {
                return this.telefonoField;
            }
            set
            {
                this.telefonoField = value;
            }
        }

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
    }
}

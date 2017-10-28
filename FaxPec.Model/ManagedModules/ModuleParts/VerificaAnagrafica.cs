using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Unisys.Pdf.ManagedModules.ModuleParts
{
    public class VerificaAnagrafica
    {

        private string tipoField;

        private string codiceFiscaleField;

        private string cognomeField;

        private string nomeField;

        private string giornoNasField;

        private string meseNasField;

        private string annoNasField;

        private string sessoField;

        private string giornoRifField;

        private string meseRifField;

        private string annoRifField;

        private string cartaIDField;

        private string noteRichiesta;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NoteRichiesta
        {
            get { return noteRichiesta; }
            set { noteRichiesta = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Tipo
        {
            get
            {
                return this.tipoField;
            }
            set
            {
                this.tipoField = value;
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
        public string CartaID
        {
            get
            {
                return this.cartaIDField;
            }
            set
            {
                this.cartaIDField = value;
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
                                            !string.IsNullOrEmpty(GiornoNas) ? GiornoNas : "00",
                                            !string.IsNullOrEmpty(MeseNas) ? MeseNas : "00",
                                            AnnoNas);
                // salta solo se anno non presente
                return (ddn.Length != 10) ? string.Empty : ddn;
                
                //string ddn = String.Format("{0}/{1}/{2}",
                //                        GiornoNas,
                //                        MeseNas,
                //                        AnnoNas);
                //// date parziali ammissibili ??? ***
                //return (ddn.Length != 10)? string.Empty : ddn;
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

        /// <remarks/>
        public string DataRiferimento
        {
            get
            {
                return String.Format("{0}/{1}/{2}",
                                        GiornoRif,
                                        MeseRif,
                                        AnnoRif);
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string GiornoRif
        {
            get
            {
                return (this.giornoRifField.Length == 1) ? "0" + this.giornoRifField : this.giornoRifField;
            }
            set
            {
                this.giornoRifField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MeseRif
        {
            get
            {
                return (this.meseRifField.Length == 1) ? "0" + this.meseRifField : this.meseRifField;
            }
            set
            {
                this.meseRifField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AnnoRif
        {
            get
            {
                return this.annoRifField;
            }
            set
            {
                this.annoRifField = value;
            }
        }
    }
}

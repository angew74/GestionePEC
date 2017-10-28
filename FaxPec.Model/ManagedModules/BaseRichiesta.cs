using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Com.Unisys.Pdf.ManagedModules.ModuleParts;
using FaxPec.Model;

namespace Com.Unisys.Pdf.ManagedModules
{
    public class BaseRichiesta
    {
        #region "Private Fields"

        private bool isAuto = true;
        private string codiceModField;
        private System.Xml.XmlDocument rawRequest;

        protected string ol;
        protected string ov;
        protected string c = "</label>";
        protected string sp = " "; 

        #endregion

        #region "Public Properties"

        [System.Xml.Serialization.XmlIgnore]
        public System.Xml.XmlDocument RawRequest
        {
            get { return rawRequest; }
            set { rawRequest = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceMod
        {
            get
            {
                return this.codiceModField;
            }
            set
            {
                this.codiceModField = value;
            }
        }

        public bool IsAuto
        {
            get
            {
                return this.isAuto;
            }
            set
            {
                this.isAuto = value;
            }
        }

        #endregion

        #region "Public Methods"

        public virtual string getRichiedente(string labelStyle, string valueStyle)
        {
            ol = "<label class='" + labelStyle + "'>";
            ov = "<label class='" + valueStyle + "'>";
            return null;
        }

        public virtual string getRichiesta(string labelStyle, string valueStyle)
        {
            ol = "<label class='" + labelStyle + "'>";
            ov = "<label class='" + valueStyle + "'>";
            return null;
        }

        public virtual Richiedente getRichiedenteField()
        {
            return null;
        }

        public virtual string getSottoTitolo()
        {
            return null;
        }

        public virtual int getNumeroProcedure()
        {
            return 1;   //di default una
        }

        public virtual Pratica getDatiComuniRichiesta()
        {
            return null;
        }

        public virtual void setDatiComuniRichiesta(Pratica dati)
        {
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Com.Unisys.Pdf.ManagedModules.ModuleParts;
using FaxPec.Model;

namespace Com.Unisys.Pdf.ManagedModules
{
    [System.Xml.Serialization.XmlRoot(ElementName = "Richiesta")]
    public class RichiestaCertificati : BaseRichiesta
    {
        private Richiedente richiedenteField;

        private Certificato datiRichiestaField;

        public const string CODICE_MOD_PRI = "Mod.Cpri";
        public const string CODICE_MOD_PUB = "Mod.Cpub";

        //public static RichiestaCertificati deserializePdf(BaseRichiesta baseRequest)
        //{
        //    if (baseRequest.CodiceMod != CODICE_MOD_PRI && baseRequest.CodiceMod != CODICE_MOD_PUB)
        //        throw new System.Configuration.ConfigurationException("Formato dell'Xml da deserializzare non valido");
        //    return XmlSerialize.deserializeRequest<RichiestaCertificati>(baseRequest.RawRequest);

        //    /////////TEST///////
        //    //System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        //    //doc.Load(@"C:\dev\FaxPec\CertiWebAppAnag\test_certificati.xml");
        //    ////System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();
        //    //BaseRichiesta b = new BaseRichiesta();
        //    //b.RawRequest = doc;
        //    //b.CodiceMod = CODICE_MOD_PRI;
        //    //RichiestaCertificati v = XmlSerialize.deserializeRequest<RichiestaCertificati>(doc);
        //    //return v;
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Richiedente", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Richiedente Richiedente
        {
            get
            {
                return this.richiedenteField;
            }
            set
            {
                this.richiedenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiRichiesta", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Certificato DatiRichiesta
        {
            get
            {
                return this.datiRichiestaField;
            }
            set
            {
                this.datiRichiestaField = value;
            }
        }

        public override string getRichiedente(string labelStyle, string valuesStyle)
        {
            base.getRichiedente(labelStyle, valuesStyle);

            if (this.Richiedente.TipoRichiedente.Equals("1"))//privato
            {
                return ol + "Privato:" + c +
                           ov + this.Richiedente.CodiceFiscale.ToUpper() + sp +
                                this.Richiedente.Cognome.ToUpper() + sp +
                                this.Richiedente.Nome.ToUpper() + c +
                           ol + "Tel.:" + c +
                           ov + this.Richiedente.Telefono + c;
            }
            if (this.Richiedente.TipoRichiedente.Equals("2"))//ente
            {
                return ol + "Ente:" + c +
                           ov + this.Richiedente.RagioneSociale.ToUpper() + sp +
                                this.Richiedente.Ufficio.ToUpper() + c +
                           ol + "Riferimento:" + c +
                           ov + this.Richiedente.Cognome.ToUpper() + sp +
                                this.Richiedente.Nome.ToUpper() + sp +
                                this.Richiedente.Telefono + c;
            }

            if (this.Richiedente.TipoRichiedente.Equals("3"))//azienda
            {
                return ol + "Ente:" + c +
                          ov + this.Richiedente.RagioneSociale.ToUpper() + sp +
                               this.Richiedente.Ufficio.ToUpper() + c +
                          ol + "P.Iva:" + c +
                          ov + this.Richiedente.PartitaIVA + c +
                          ol + "Riferimento:" + c +
                          ov + this.Richiedente.Cognome.ToUpper() + sp +
                               this.Richiedente.Nome.ToUpper() + sp +
                               this.Richiedente.Telefono + c;
            }
            else return null;
        }

        public override string getRichiesta(string labelStyle, string valueStyle)
        {
            
            base.getRichiesta(labelStyle, valueStyle);

            string certificato = ol + "Certificato:" + c +
                          ov + "#" + this.DatiRichiesta.Tipo + "#" + c + "<br/>" +
                          ol + "Esenzione:" + c +
                          ov + "#" + this.DatiRichiesta.Esenzione + c ;

            string intestatario = ol + "Intestatario:" + c +
                               ov + this.DatiRichiesta.Cognome + sp +
                                    this.DatiRichiesta.Nome + sp +
                                    this.DatiRichiesta.CodiceFiscale + c +
                               ol + "Nato/a il:" + c +
                               ov + this.DatiRichiesta.DataDiNascita + c;

            return certificato + "<br/>" + intestatario;
        }
        
        public override Richiedente getRichiedenteField()
        {
            return this.Richiedente;
        }

        public override string getSottoTitolo()
        {
            return base.CodiceMod;
        }

        public override int getNumeroProcedure()
        {
            return 1;
        }

        public override Pratica getDatiComuniRichiesta()
        {
            Pratica d = new Pratica();
            d.Cognome = this.DatiRichiesta.Cognome;
            d.Nome = this.DatiRichiesta.Nome;
            d.CodFis = this.DatiRichiesta.CodiceFiscale;
            d.Sesso = this.DatiRichiesta.Sesso;
            d.GiornoNascita = this.DatiRichiesta.GiornoNas;
            d.MeseNascita = this.DatiRichiesta.MeseNas;
            d.AnnoNascita = this.DatiRichiesta.AnnoNas;
            return d;
        }

        public override void setDatiComuniRichiesta(Pratica d)
        {
            this.DatiRichiesta = new Certificato();
            this.DatiRichiesta.Cognome = d.Cognome;
            this.DatiRichiesta.Nome = d.Nome;
            this.DatiRichiesta.CodiceFiscale = d.CodFis;
            this.DatiRichiesta.Sesso = d.Sesso;
            this.DatiRichiesta.GiornoNas = d.GiornoNascita;
            this.DatiRichiesta.MeseNas = d.MeseNascita;
            this.DatiRichiesta.AnnoNas = d.AnnoNascita;
            if(string.IsNullOrEmpty(d.Cognome) && string.IsNullOrEmpty(d.CodFis))
            this.IsAuto = false;
            else this.IsAuto = true;
        }
    }
}

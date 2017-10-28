using System;
using System.Collections.Generic;
using System.Text;
using Com.Unisys.Pdf.ManagedModules.ModuleParts;
using FaxPec.Model;

namespace Com.Unisys.Pdf.ManagedModules
{
    [System.Xml.Serialization.XmlRoot(ElementName = "Richiesta")]
    public class RichiestaVerificheAnagrafiche : BaseRichiesta
    {
        #region "Private fields"

        private Richiedente richiedenteField;

        private VerificaAnagrafica datiRichiestaField;

        #endregion

        #region "Public properties"

        public const string CODICE_MOD_VANAG = "Mod.Veri";

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
        public VerificaAnagrafica DatiRichiesta
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

        #endregion

        #region "Public methods"

        //public static RichiestaVerificheAnagrafiche deserializePdf(BaseRichiesta baseRequest)
        //{
        //    if (baseRequest.CodiceMod != CODICE_MOD_VANAG)
        //        throw new System.Configuration.ConfigurationException("Formato dell'Xml da deserializzare non valido");
        //    return XmlSerialize.deserializeRequest<RichiestaVerificheAnagrafiche>(baseRequest.RawRequest);

        //    /////////TEST///////
        //    //System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        //    //doc.Load(@"C:\dev\FaxPec\CertiWebAppAnag\test_verifiche.xml");
        //    ////System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();
        //    //BaseRichiesta b = new BaseRichiesta();
        //    //b.RawRequest = doc;
        //    //b.CodiceMod = CODICE_MOD_VANAG;
        //    ////b.CodiceMod = nav.SelectSingleNode("//CodiceMod").Value;
        //    //RichiestaVerificheAnagrafiche v = XmlSerialize.deserializeRequest<RichiestaVerificheAnagrafiche>(doc);
        //    //return v;
        //}

        public override string getRichiedente(string labelStyle, string valuesStyle)
        {
            base.getRichiedente(labelStyle, valuesStyle);

            string richiedente=string.Empty;
                if (! string.IsNullOrEmpty(this.Richiedente.RagioneSociale))
                    richiedente = this.Richiedente.RagioneSociale.ToUpper();
                else if(! string.IsNullOrEmpty(this.Richiedente.Cognome) || !string.IsNullOrEmpty(this.Richiedente.CodiceFiscale))
                    richiedente= this.Richiedente.CodiceFiscale.ToUpper() + sp +
                                this.Richiedente.Cognome.ToUpper() + sp +
                                this.Richiedente.Nome.ToUpper();

                if(!string.IsNullOrEmpty(richiedente))
                    return ol + "Richiedente:" + c + ov + richiedente + c ;
            else return null;
            
        }

        public override string getRichiesta(string labelStyle, string valueStyle)
        {
            base.getRichiesta(labelStyle, valueStyle);
            string ret = ol + "Verifica per" + c;
            if(!string.IsNullOrEmpty(this.DatiRichiesta.Cognome))
                   ret= ret+ol + "Nominativo:" + c +
                        ov + this.DatiRichiesta.Cognome.ToUpper() + sp + this.DatiRichiesta.Nome.ToUpper() +c;
            if(!string.IsNullOrEmpty(this.DatiRichiesta.CodiceFiscale))
                   ret=ret+ ol + "CdF:" +c+
                           ov+ this.DatiRichiesta.CodiceFiscale.ToUpper() + c;
            if(!string.IsNullOrEmpty(this.DatiRichiesta.DataDiNascita))
                 ret=ret+ ol + "Data Nascita:" +c+
                           ov+ this.DatiRichiesta.CodiceFiscale.ToUpper() + c;
            if(!string.IsNullOrEmpty(this.DatiRichiesta.NoteRichiesta))
                 ret=ret+ ol + "Testo Libero:" +c+
                           ov+ this.DatiRichiesta.NoteRichiesta + c;
            
            return ret;
        }

        public override Richiedente getRichiedenteField()
        {
            return this.Richiedente;
        }

        public override string getSottoTitolo()
        {
            return this.DatiRichiesta.Tipo;
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
            d.Note = this.DatiRichiesta.NoteRichiesta;
            return d;
        }

        public override void setDatiComuniRichiesta(Pratica d)
        {
            this.DatiRichiesta = new VerificaAnagrafica();
            this.DatiRichiesta.Cognome = d.Cognome;
            this.DatiRichiesta.Nome = d.Nome;
            this.DatiRichiesta.CodiceFiscale = d.CodFis;
            this.DatiRichiesta.Sesso = d.Sesso;
            this.DatiRichiesta.GiornoNas = d.GiornoNascita;
            this.DatiRichiesta.MeseNas = d.MeseNascita;
            this.DatiRichiesta.AnnoNas = d.AnnoNascita;
            this.DatiRichiesta.NoteRichiesta= d.Note;
            this.IsAuto = true;
        }

        #endregion
 
    }
}

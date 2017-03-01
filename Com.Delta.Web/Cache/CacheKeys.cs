using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Delta.Web.Cache
{
    //TODO: per ogni chiave aggiungere se esite il dataset
    //di riferimento nel quale occorre caricarle
    public enum CacheKeys
    {
        NULL,
        LISTA_DECODIFICA_MESSAGGI,
        XSLT_NETVA2_TO_HTML,
        XSLT_NETVA2_TAGLIO,
        XSLT_VELE_TAGLIO,
        XSLT_VELE_TO_HTML,
        NETNASCITA,
        NETNASCITAP,
        NETMATRIMONIO,
        NETDECESSO,
        modello_13,
        #region OLD

        DATASET_FLAGVIVORESIDENTE,
        DATASET_LISTA_DECODIFICA_ASSOCIAZIONI,
        DATASET_LISTA_INDIRIZZI_MUNICIPI,
        DATASET_MUNICIPI_SEDI,

        LISTA_ESPONENTE,
        DATASET_RAPPORTI_PARENTELA_M,
        DATASET_RAPPORTI_PARENTELA_F,
        //DATASET_RAPPORTI_PARENTELA_CONV,
        LISTA_TIPO_CARTA_IDENTITA,

        TIPOLOGICA_STATOCIVILE_M,
        TIPOLOGICA_STATOCIVILE_F,
        TIPOLOGICA_PROFESSIONE_M,
        TIPOLOGICA_PROFESSIONE_F,
        TIPOLOGICA_POSIZIONE_PROF,
        TIPOLOGICA_ATTIVITA,
        TIPOLOGICA_CONDIZIONE_NON_PROF,
        TIPOLOGICA_TITOLOSTUDIO,
        //TIPOLOGICA_COMUNITARI,
        TIPOLOGICA_NAZIONI,

        SEDI_PENSIONI,
        CATEGORIE_PENSIONI,
        ENTE_PENSIONI,
        CATEGORIE_PATENTE,
        CATEGORIE_RIC_CON_CODICI,
        CATEGORIE_VEICOLI,
        MOTIVAZIONI,
        #endregion

        LISTE_SEZIONI_COMMISSIONI,


        //#region Chiavi Comuni dell'Anagrafe

        //ANAG_PERS_FLAGVIVORESIDENTE,
        //ANAG_PERS_RAPPORTIPARENTELAM,
        //ANAG_PERS_RAPPORTIPARENTELAF,
        //ANAG_PERS_STATOCIVILEM,
        //ANAG_PERS_STATOCIVILEF,
        //ANAG_PERS_PROFESSIONEM,
        //ANAG_PERS_PROFESSIONEF,
        //ANAG_PERS_POSIZIONEPROF,
        //ANAG_PERS_ATTIVITA,
        //ANAG_PERS_CONDIZIONENONPROF,
        //ANAG_PERS_TITOLOSTUDIO,
        //ANAG_PERS_COMUNITARI,

        //ANAG_DOCUM_CARTAIDENTITATIPO,
        //ANAG_DOCUM_PENSIONISEDI,
        //ANAG_DOCUM_PENSIONICATEGORIE,

        //ANAG_TERRIT_ESPONENTI,
        //ANAG_TERRIT_ASSOCIAZIONI,

        //ANAG_CERTI_CERTIFICATI,
        //ANAG_CERTI_MOTIVAZIONIESENZABOLLO,

        //ANAG_CRAB_ESITOACCERTAMENTO,
        //ANAG_CRAB_TIPOIMMIGRAZIONE,
        //#endregion

        //#region Applicazione:Certificati

        //CERTI_MESSAGGI_MAPPER,

        //#endregion

        //#region Applicazione:Cambi di Residenza e Domicilio
        //CRAB_MESSAGGI_MAPPER,
        //CRAB_STATUS_CDF,
        //CRAB_STATUS_CRE,
        //CRAB_STATUS_CRI,

        //XSLT_CRAB_XMLHTML_CRI_OMONIMIE,
        //XSLT_CRAB_XMLHTML_CRI_ATTISOSPESI,
        //XSLT_CRAB_XMLHTML_CRI_CONFERMA,
        //XSLT_CRAB_XMLHTML_CRI_CONFERMAFAMIGLIA,
        //XSLT_CRAB_XMLHTML_CRI_ELENCO_MACROERRORI,
        //XSLT_CRAB_XMLHTML_CRI_ISERRBD1,
        //XSLT_CRAB_XMLHTML_CRI_ISERRBD2,
        //#endregion

        //#region XML2PDF

        //FO_XMLPDF_CERTIFICATI_BASEPDF,
        //FO_XMLPDF_CERTIFICATI_TESTAPDF,
        //FO_XMLPDF_CERTIFICATI_CODAPDF,
        //FO_XMLPDF_CERTIFICATI_NASCITAPDF,
        //FO_XMLPDF_CERTIFICATI_MATRIMONIOPDF,
        //FO_XMLPDF_CERTIFICATI_DECESSOPDF,
        //FO_XMLPDF_CERTIFICATI_CITTADINANZAPDF,
        //FO_XMLPDF_CERTIFICATI_RESIDENZAPDF,

        //#endregion

        //#region XML2XML
        //XSLT_STRXML_SUPERNETVA_CNASCITA,
        //XSLT_STRXML_SUPERNETVA_CMATRIMONIO,
        //XSLT_STRXML_SUPERNETVA_CDECESSO,
        //XSLT_STRXML_SUPERNETVA_CCITTADINANZA,
        //XSLT_STRXML_SUPERNETVA_CRESIDENZA,
        //XSLT_STRXML_SUPERNETVA_NETVA2,
        //#endregion

        //#region XML2HTML
        //XSLT_XMLHTML_NETVA2ANAGRAFE,
        //XSLT_XMLHTML_PRATICACRI,
        //XSLT_XMLHTML_PRATICACRE,
        //XSLT_XMLHTML_PRATICACDF,
        //XSLT_XMLHTML_ITERPRATICACRAB,
        //XSLT_XMLHTML_PRATICAIRREPERIBILITA,
        //#endregion
        SUPERNETVA_STRING_TO_XML,
        SMALLVA_XML_TO_HTML,
        SUPERNETVA_XML_TO_HTML,
        NETMATRIMONIO_SMALL,
        NETNASCITA_SMALL,
        NETUNIONE_SMALL,
        NETUNIONE,
        FOLDERS_ACTIONS,
        DATI_MEMO,
        FOLDERS,
        IMMAGINE_ROMA_CAPITALE
    }
}

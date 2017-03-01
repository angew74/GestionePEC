using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SendMail.Model
{
    public class RawMessage
    {

        private List<StampeCRBean> _list = new List<StampeCRBean>();

        public List<StampeCRBean> RawList
        {
            get { return _list; }
            set { _list = value; }
        }

        public class StampeCRBean
        {
            private string _annPrt;

            public string AnnPrt
            {
                get { return _annPrt; }
                set { _annPrt = value; }
            }
            private string _numPrt;

            public string NumPrt
            {
                get { return _numPrt; }
                set { _numPrt = value; }
            }
            private string _tipRic;

            public string TipRic
            {
                get { return _tipRic; }
                set { _tipRic = value; }
            }

            private string prgRic;

            public string PrgRic
            {
                get { return prgRic; }
                set { prgRic = value; }
            }

            private string _nomeTpu;

            public string NomeTpu
            {
                get { return _nomeTpu; }
                set { _nomeTpu = value; }
            }
            private string _progTpu;

            public string ProgTpu
            {
                get { return _progTpu; }
                set { _progTpu = value; }
            }

            private string _progPru;

            public string ProgPru
            {
                get { return _progPru; }
                set { _progPru = value; }
            }

            private string _datiPru;

            public string DatiPru
            {
                get { return _datiPru; }
                set { _datiPru = value; }
            }

            private string stringaID;

            public string StringaID
            {
                get { return stringaID; }
                set { stringaID = value; }
            }

            private string _codDestinatario;

            public string CodDestinatario
            {
                get { return _codDestinatario; }
                set { _codDestinatario = value; }
            }

            private string _emailMittente;
            public string EmailMittente
            {
                get { return _emailMittente; }
                set { _emailMittente = value; }

            }
             
        }

        public string AnnPrt
        {
            get { return _list[0].AnnPrt; }
        }

        public string NumPrt
        {
            get { return _list[0].NumPrt; }
        }

        public string TipRic
        {
            get { return _list[0].TipRic; }
        }

        public string StringaID
        {
            get { return _list[0].StringaID; }
        }

        string _emailMittente;
        public string EmailMittente
        {
            get { return _list[0].EmailMittente; }
        }
        
        public string[] FormattedDestinatariCodes
        {
            get{return _list[0].CodDestinatario.Split(';');}
        }

        public string FormattedTestoMail
        {
            get
            {
                StringBuilder dati = new StringBuilder();
                for (int i = 0; i < _list.Count; i++)
                {
                    if (_list[i].NomeTpu.ToUpper().Trim() == "TESTO_MAIL")
                    {
                        dati.Append(_list[i].DatiPru);
                    }
                }
                return dati.ToString();
            }
        }

        public System.Collections.Generic.Dictionary<int, string> FormattedDatiTpus
        {
            get
            {
                System.Collections.Generic.Dictionary<int, string> prus = new System.Collections.Generic.Dictionary<int, string>();
                for (int i = 0; i < _list.Count; i++)
                {
                    if (!(string.IsNullOrEmpty(_list[i].ProgPru) || _list[i].NomeTpu.ToLower().Equals("testo_mail")))
                    {
                        if (!prus.ContainsKey(int.Parse(_list[i].ProgTpu.Trim())))
                            prus.Add(int.Parse(_list[i].ProgTpu.Trim()), string.Empty);
                        prus[int.Parse(_list[i].ProgTpu.Trim())] = prus[int.Parse(_list[i].ProgTpu.Trim())] + _list[i].DatiPru;
                    }
                }
                return prus;
            }
        }

        public System.Collections.Generic.Dictionary<int, string> FormattedNomiTpus
        {
            get
            {
                System.Collections.Generic.Dictionary<int, string> tpus = new System.Collections.Generic.Dictionary<int, string>();
                for (int i = 0; i < _list.Count; i++)
                    if (!(string.IsNullOrEmpty(_list[i].ProgPru) || _list[i].NomeTpu.ToLower().Equals("testo_mail")))
                    {
                        if (!tpus.ContainsKey(int.Parse(_list[i].ProgTpu.Trim())))
                            tpus.Add(int.Parse(_list[i].ProgTpu.Trim()), _list[i].NomeTpu);
                    }
                return tpus;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model
{
    public class RubricaBean
    {
        private string _codEnte;

        public string CodEnte
        {
            get { return _codEnte; }
            set { _codEnte = value; }
        }

        private string _prgEnte;

        public string PrgEnte
        {
            get { return _prgEnte; }
            set { _prgEnte = value; }
        }

        private string _tipRic;

        public string TipRic
        {
            get { return _tipRic; }
            set { _tipRic = value; }
        }

        private string _ragioneSociale;

        public string RagioneSociale
        {
            get { return _ragioneSociale; }
            set { _ragioneSociale = value; }
        }

        private string _ufficio;

        public string Ufficio
        {
            get { return _ufficio; }
            set { _ufficio = value; }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private string _provincia;

        public string Provincia
        {
            get { return _provincia; }
            set { _provincia = value; }
        }

        private string _regione;

        public string Regione
        {
            get { return _regione; }
            set { _regione = value; }
        }

        private string _codRubrica;

        public string CodRubrica
        {
            get { return _codRubrica; }
            set { _codRubrica = value; }
        }

        private string _citta;
        public string Citta
        {
            get { return _citta; }
            set { _citta = value; }
        }
    }
}

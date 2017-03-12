using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Extensions
{
    public class CodFisCheck
    {
        private string strCodFisc;
        private List<string> vctCd = new List<string>();
        private List<string> vctCp = new List<string>();
        private List<string> vctNd = new List<string>();
        private List<string> vctNp = new List<string>();
        private List<string> vctVm = new List<string>();
        private Dictionary<string, int> ctr_giorno = new Dictionary<string, int>();

        private static String ritorno;


        public void UCheckDigit(string g)
        {
            strCodFisc = g.ToUpper();
            creaCarattereDispari();
            creaCaratterePari();
            creaNumeroDispari();
            creaNumeroPari();
            creaMese();
            crea_ctr_giorno();
        }


        public bool controllaCorrettezza(string CF)
        {
            UCheckDigit(CF);
            if (controllaCorrettezzaChar() == '0')
                return true;
            else
                return false;
        }


        public char controllaCorrettezzaChar()
        {
            bool bolLettera = false;

            //------------------------------------------
            //primi 6 caratteri alfabetici
            for (int i = 0; i < 6; i++)
            {
                if (!char.IsLetter(char.Parse(strCodFisc.Substring(i, 1))))
                    return '2';
            }
            //-----------------------------------------


            //-----------------------------------------
            //controllo sull'anno
            for (int i = 6; i < 8; i++)
            {
                if (!(char.IsDigit(char.Parse(strCodFisc.Substring(i, 1)))) && ((strCodFisc.Substring(i, 1) != "L")) &&
                ((strCodFisc.Substring(i, 1) != "M")) && ((strCodFisc.Substring(i, 1) != "N")) && ((strCodFisc.Substring(i, 1) != "P"))
                && ((strCodFisc.Substring(i, 1) != "Q")) && ((strCodFisc.Substring(i, 1) != "R")) &&
                ((strCodFisc.Substring(i, 1) != "S")) && ((strCodFisc.Substring(i, 1) != "T")) &&
                ((strCodFisc.Substring(i, 1) != "U")) && ((strCodFisc.Substring(i, 1) != "V")))
                    return '2';
            }
            //--------------------------------------------------------



            //--------------------------------------------------------
            //controllo mese
            if (!((strCodFisc.Substring(8, 1) != "A") || (strCodFisc.Substring(8, 1) != "B") ||
                (strCodFisc.Substring(8, 1) != "C") || (strCodFisc.Substring(8, 1) != "D") ||
                (strCodFisc.Substring(8, 1) != "E") || (strCodFisc.Substring(8, 1) != "H") ||
                (strCodFisc.Substring(8, 1) != "L") || (strCodFisc.Substring(8, 1) != "M") ||
                (strCodFisc.Substring(8, 1) != "P") || (strCodFisc.Substring(8, 1) != "R") ||
                (strCodFisc.Substring(8, 1) != "S") || (strCodFisc.Substring(8, 1) != "T")))
                return '2';
            //-----------------------------------------------------------------



            //controllo giorno -----------------------------------------------

            for (int i = 9; i < 11; i++)
            {
                if (!(char.IsDigit(char.Parse(strCodFisc.Substring(i, 1)))) && ((strCodFisc.Substring(i, 1) != "L")) &&
                ((strCodFisc.Substring(i, 1) != "M")) && ((strCodFisc.Substring(i, 1) != "N")) && ((strCodFisc.Substring(i, 1) != "P"))
                && ((strCodFisc.Substring(i, 1) != "Q")) && ((strCodFisc.Substring(i, 1) != "R")) &&
                ((strCodFisc.Substring(i, 1) != "S")) && ((strCodFisc.Substring(i, 1) != "T")) &&
                ((strCodFisc.Substring(i, 1) != "U")) && ((strCodFisc.Substring(i, 1) != "V")))
                    return '2';
            }

            int intGiorno = trasforma_giorno(9);
            if (intGiorno > 31)
                intGiorno -= 40;
            if (intGiorno < 1 || intGiorno > 31)
                return '2';
            //---------------------------------------------------------------------------------



            // formatta giorno mese anno per controllo data------------------------------------------------
            string strElem = strCodFisc.Substring(8, 1);
            string strMese = vctVm.IndexOf(strElem).ToString();
            if (strMese.Length == 1)
                strMese = "0" + strMese;
            string strAnno = trasforma_giorno(6).ToString();
            if (strAnno.Length == 1)
                strAnno = "0" + strAnno;
            string strGiorno = intGiorno.ToString();
            if (strGiorno.Length == 1)
                strGiorno = "0" + strGiorno;

            string data = strGiorno + strMese + strAnno;
            if (!(controllaData(data))) // QUI CHIAMA METODO PER CONTROLLO DATA
                return '2';

            //controlla che la prima lettera del codice comune belfiore sia valida
            if ((strCodFisc.Substring(11, 1) != "A") && (strCodFisc.Substring(11, 1) != "B") &&
                (strCodFisc.Substring(11, 1) != "C") && (strCodFisc.Substring(11, 1) != "D") &&
                (strCodFisc.Substring(11, 1) != "E") && (strCodFisc.Substring(11, 1) != "F") &&
                (strCodFisc.Substring(11, 1) != "G") && (strCodFisc.Substring(11, 1) != "H") &&
                (strCodFisc.Substring(11, 1) != "I") && (strCodFisc.Substring(11, 1) != "L") &&
                (strCodFisc.Substring(11, 1) != "M") && (strCodFisc.Substring(11, 1) != "Z"))
                return '2';


            //controlla le lettere per il codice comune (che sostituiscono i numeri in caso di omocodice)
            for (int i = 12; i < 15; i++)
            {
                if (!(char.IsDigit((char.Parse(strCodFisc.Substring(i, 1))))))
                {
                    bolLettera = true;
                    if ((strCodFisc.Substring(i, 1) != "L") && (strCodFisc.Substring(i, 1) != "M") &&
                        (strCodFisc.Substring(i, 1) != "N") && (strCodFisc.Substring(i, 1) != "P") &&
                        (strCodFisc.Substring(i, 1) != "Q") && (strCodFisc.Substring(i, 1) != "R") &&
                        (strCodFisc.Substring(i, 1) != "S") && (strCodFisc.Substring(i, 1) != "T") &&
                        (strCodFisc.Substring(i, 1) != "U") && (strCodFisc.Substring(i, 1) != "V"))
                        return '3';
                }
            }

            //controlla i numeri per il codice comune 
            if (bolLettera == false)
            {
                int intNumeroCodCat = int.Parse(strCodFisc.Substring(12, 3));
                if (intNumeroCodCat == 000)
                    return '2';
                if ((strCodFisc.Substring(11, 1) == "M") && (intNumeroCodCat > 399))
                    return '2';
            }

            if (controllaCheckDigit())
                return '0';
            return '1';
        }



        #region Metodi


        //METODO SULLA VERIFICA DIGITAZIONE IN BASE ALL'ULTIMO CARATTERE
        public bool controllaCheckDigit()
        {
            int intAppoggio = 0;
            char chrCarattereEsaminato;

            for (int i = 0; i < 15; i++)
            {
                chrCarattereEsaminato = char.Parse(strCodFisc.Substring(i, 1));
                string strElem = strCodFisc.Substring(i, 1);
                int intResto = i % 2;

                switch (intResto)
                {
                    case 0:
                        if (char.IsDigit(chrCarattereEsaminato) == false)
                            intAppoggio += vctCd.IndexOf(strElem);
                        else
                            intAppoggio += vctNd.IndexOf(strElem);
                        break;
                    case 1:
                        if (char.IsDigit(chrCarattereEsaminato) == false)
                            intAppoggio += vctCp.IndexOf(strElem);
                        else
                            intAppoggio += vctNp.IndexOf(strElem);
                        break;
                    default: break;
                }
            }
            string checkdigit = strCodFisc.Substring(15, 1);
            return (intAppoggio % 26) == vctCp.IndexOf(checkdigit);
        }


        //METODO SUL CONTROLLO DELLA DATA
        public static bool controllaData(string s)
        {
            try
            {
                string strAnno = s.Substring(4, 2);
                string strMese = s.Substring(2, 2);
                string strGiorno = s.Substring(0, 2);

                int intAnno = int.Parse(strAnno);
                int intMese = int.Parse(strMese);
                int intGiorno = int.Parse(strGiorno);

                if ((intMese > 12) || (intGiorno > 31) || (intMese < 1) || (intGiorno < 1))
                { return false; }

                switch (intMese)
                {
                    case 2:
                        bool bisestile = false;
                        if (intAnno % 4 == 0)
                            bisestile = true;
                        if ((bisestile && (intGiorno > 29)) || (!bisestile && (intGiorno > 28)))
                        { return false; }
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        if (intGiorno > 30)
                        { return false; }
                        break;
                    default:
                        break;
                }
                return true;
            }

            catch
            { return false; }
        }



        // metodo controllo sul giorno di nascita----------------------------------------------------
        public int trasforma_giorno(int c)
        {
            string appo = "";

            for (int i = c; i < c + 2; i++)
            {
                if (char.IsDigit((char.Parse(strCodFisc.Substring(i, 1)))))
                    appo += strCodFisc.Substring(i, 1);
                else
                {
                    appo += ctr_giorno[strCodFisc.Substring(i, 1)];
                }
            }
            return int.Parse(appo);
        }


        public void creaMese()
        {
            vctVm.Add(" ");
            vctVm.Add("A");  //Gennaio
            vctVm.Add("B");
            vctVm.Add("C");
            vctVm.Add("D");
            vctVm.Add("E");
            vctVm.Add("H");
            vctVm.Add("L");
            vctVm.Add("M");
            vctVm.Add("P");
            vctVm.Add("R");
            vctVm.Add("S");
            vctVm.Add("T");  //Dicembre
        }


        public void creaCarattereDispari()
        {
            //List<string> vctCd = new List<string>();

            vctCd.Add("B"); //valore dei caratteri dispari
            vctCd.Add("A");
            vctCd.Add("K");
            vctCd.Add("P");
            vctCd.Add("L");
            vctCd.Add("C");
            vctCd.Add("Q");
            vctCd.Add("D");
            vctCd.Add("R");
            vctCd.Add("E");
            vctCd.Add("V");
            vctCd.Add("O");
            vctCd.Add("S");
            vctCd.Add("F");
            vctCd.Add("T");
            vctCd.Add("G");
            vctCd.Add("U");
            vctCd.Add("H");
            vctCd.Add("M");
            vctCd.Add("I");
            vctCd.Add("N");
            vctCd.Add("J");
            vctCd.Add("W");
            vctCd.Add("Z");
            vctCd.Add("Y");
            vctCd.Add("X");
            //return vctCd<string>;
        }


        public void creaCaratterePari()
        {
            //List<string> vctCp = new List<string>();

            vctCp.Add("A"); //valore dei caratteri pari
            vctCp.Add("B");
            vctCp.Add("C");
            vctCp.Add("D");
            vctCp.Add("E");
            vctCp.Add("F");
            vctCp.Add("G");
            vctCp.Add("H");
            vctCp.Add("I");
            vctCp.Add("J");
            vctCp.Add("K");
            vctCp.Add("L");
            vctCp.Add("M");
            vctCp.Add("N");
            vctCp.Add("O");
            vctCp.Add("P");
            vctCp.Add("Q");
            vctCp.Add("R");
            vctCp.Add("S");
            vctCp.Add("T");
            vctCp.Add("U");
            vctCp.Add("V");
            vctCp.Add("W");
            vctCp.Add("X");
            vctCp.Add("Y");
            vctCp.Add("Z");
        }


        public void creaNumeroDispari()
        {
            //List<string> vctNd = new List<string>();

            vctNd.Add("1");
            vctNd.Add("0");
            vctNd.Add(" ");
            vctNd.Add(" ");
            vctNd.Add(" ");
            vctNd.Add("2");
            vctNd.Add(" ");
            vctNd.Add("3");
            vctNd.Add(" ");
            vctNd.Add("4");
            vctNd.Add(" ");
            vctNd.Add(" ");
            vctNd.Add(" ");
            vctNd.Add("5");
            vctNd.Add(" ");
            vctNd.Add("6");
            vctNd.Add(" ");
            vctNd.Add("7");
            vctNd.Add(" ");
            vctNd.Add("8");
            vctNd.Add(" ");
            vctNd.Add("9");
        }


        public void creaNumeroPari()
        {
            //List<string> vctNp = new List<string>();

            vctNp.Add("0"); //valore dei numeri pari
            vctNp.Add("1");
            vctNp.Add("2");
            vctNp.Add("3");
            vctNp.Add("4");
            vctNp.Add("5");
            vctNp.Add("6");
            vctNp.Add("7");
            vctNp.Add("8");
            vctNp.Add("9");
        }


        public Dictionary<string, int> crea_ctr_giorno()
        {
            //Dictionary<string, int> ctr_giorno = new Dictionary<string, int>();

            ctr_giorno.Add("L", 0);
            ctr_giorno.Add("M", 1);
            ctr_giorno.Add("N", 2);
            ctr_giorno.Add("P", 3);
            ctr_giorno.Add("Q", 4);
            ctr_giorno.Add("R", 5);
            ctr_giorno.Add("S", 6);
            ctr_giorno.Add("T", 7);
            ctr_giorno.Add("U", 8);
            ctr_giorno.Add("V", 9);

            return ctr_giorno;
        }

        #endregion

    }
}
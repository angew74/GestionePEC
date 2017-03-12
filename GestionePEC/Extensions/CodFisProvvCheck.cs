using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Extensions
{
    public class CodFisProvvCheck
    {
        //ingresso
        public bool CFprovvCheck(string CF)
        {
            CF = CF.ToUpper();
            char c = CFtest(CF);

            if (c == 'T')
                return true;
            if (c == 'F')
                return false;
            else return false;
        }

        public char CFtest(string CF)
        {
            for (int i = 0; i < 11; i++)
            {
                if (!(char.IsDigit(CF[i])))
                    return 'F';
            }

            if (CF.Substring(0, 7) == "0000000")
                return 'F';

            int codUff = int.Parse(CF.Substring(7, 3));

            if (codUff == 0)
            {
                if ((int.Parse(CF.Substring(0, 7)) > 0) && (int.Parse(CF.Substring(0, 7)) < 273961))
                    return 'T';
                if ((int.Parse(CF.Substring(0, 7)) > 400000) && (int.Parse(CF.Substring(0, 7)) < 1072480)
                    || (int.Parse(CF.Substring(0, 7)) > 1500000) && (int.Parse(CF.Substring(0, 7)) < 1828637)
                    || (int.Parse(CF.Substring(0, 7)) > 2000000) && (int.Parse(CF.Substring(0, 7)) < 2054096))
                {
                    if (controlCFnum(CF))
                        return 'T';
                    else
                        return 'F';
                }
                else
                    return 'F';
            }

            else if (((codUff > 0) && (codUff < 101)) || ((codUff > 119) && (codUff < 122)))
                return controlCFuff(CF);

            else if ((codUff > 150) && (codUff < 246))
            {
                if (controlCFnum(CF))
                    return 'T';
                else
                    return 'F';
            }

            else if ((codUff > 300) && (codUff < 767))
            {
                if (controlCFnum(CF.Substring(0, 11)))
                    return 'T';
                else
                    return 'F';
            }

            else if ((codUff > 899) && (codUff < 951))
            {
                if (controlCFnum(CF.Substring(0, 11)))
                    return 'T';
                else
                    return 'F';
            }
            else return 'F';
        }


        //controllo sulla digitazione
        public bool controlCFnum(string CF)
        {
            //int intAppoggio;
            int pintAppo;
            int pintUltimoCarattere;
            int pintTotale = 0;

            //cifre pari
            for (int i = 1; i < 11; i += 2)
            {
                string strElem = CF.Substring(i, 1);
                //intAppoggio = int.Parse(strElem);
                pintAppo = (int.Parse(strElem)) * 2;
                string strs2 = pintAppo.ToString();

                for (int j = 0; j < strs2.Length; j++)
                {
                    string strElem1 = strs2.Substring(j, 1);
                    pintTotale += int.Parse(strElem1);
                }
            }

            //cifre dispari
            for (int k = 0; k < 9; k += 2)
            {
                string strElem2 = CF.Substring(k, 1);
                pintTotale += int.Parse(strElem2);
            }

            //verifica sulla cifra di controllo

            pintUltimoCarattere = int.Parse(CF.Substring(10, 1));
            if (pintTotale % 10 == 0)
                return (pintTotale % 10) == pintUltimoCarattere;
            else
                return (10 - (pintTotale % 10)) == pintUltimoCarattere;
        }


        //controllo sull'ufficio
        public char controlCFuff(string CF)
        {
            char cRet;
            int codUff = int.Parse(CF.Substring(7, 3));
            if (int.Parse(CF.Substring(0, 7)) < 8000000)
                cRet = 'T';
            else if (codUff > 95)
                cRet = 'F';
            else
                cRet = 'T';

            if (controlCFnum(CF.Substring(0, 11)))// == false)
                cRet = 'F';

            return cRet;
        }

    }
}
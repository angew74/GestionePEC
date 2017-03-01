using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Delta.Web
{
    public static class RegexUtils
    {
        /*
            <string1>@<string2>

            <string1>:
            tutte le lettere maiuscole o minuscole
            tutti i numeri
            !#$%&'*+-/=?^_{|}~.
            il carattere . non deve essere ripetuto in modo consecutivo (i.e. .. oppure ...) e non deve essere né il primo né l'ultimo carattere
            il carattere - non deve essere ripetuto in modo consecutivo (i.e. -- oppure ---) e non deve essere né il primo né l'ultimo carattere
            nessuno spazio
            deve terminare con una lattera o un numero

            <string2>:
            tutte le lettere maiuscole o minuscole
            tutti i numeri
            -.
            il carattere . non deve essere ripetuto in modo consecutivo (i.e. .. oppure ...) e non deve essere né il primo né l'ultimo carattere
            il carattere - non deve essere ripetuto in modo consecutivo (i.e. -- oppure ---) e non deve essere né il primo né l'ultimo carattere
            deve esserci alemno un carattere .
            nessuno spazio 
         */

        //NUOVA
        // ^[a-zA-Z0-9][\w!#$%&'*+/=?\^_`{|}~]+((\.|\-)[\w!#$%&'*+/=?\^_`{|}~]+)*[a-zA-Z0-9]@((([\w]+(\.|\-))+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$
        //VECCHIA
        // ^[\w!#$%&'*+/=?\^_`{|}~]+((\.|\-)[\w!#$%&'*+/=?\^_`{|}~]+)*@((([\w]+(\.|\-))+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$
        public static readonly string EMAIL_REGEX = @"^([\w!#$%&'*+/=?\^_`{|}~]+(\.|\-)[\w!#$%&'*+/=?\^_`{|}~]*)*[\w]+@((([\w]+(\.|\-))*([\w]+(\.))[a-zA-Z]{2,4}))$";

        public static readonly string REGEX1 = @"^([\w!#$%&'*+/=?\^_`{|}~]+(\.|\-)[\w!#$%&'*+/=?\^_`{|}~]*)*[\w]+";
        public static readonly string REGEX2 = @"((([\w]+(\.|\-))*([\w]+(\.))[a-zA-Z]{2,4}))$";
        //public static readonly string EMAIL_REGEX = @"^[a-zA-Z0-9]*[\w!#$%&'*+/=?\^_`{|}~]+((\.|\-)[\w!#$%&'*+/=?\^_`{|}~]+)*[a-zA-Z0-9]@((([\w]+(\.|\-))+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;


namespace SendMail.Model
{
    /// <summary>
    /// Classe helper del model layer.
    /// </summary>
    public class ModelHelper
    {
        /// <summary>
        /// Effettua il parse di un valore enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(object value)    //where T : Enum
        {
            T result = default(T);
            if (Enum.IsDefined(typeof(T), value))
            {
                result = (T)(Enum.Parse(typeof(T), value.ToString()));
            }
            return result;
        }

        public static Decimal ParseDecimalPoint(string value)
        {
            Decimal dOut = default(Decimal);
            CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT");
            Decimal.TryParse(value, NumberStyles.AllowDecimalPoint, culture, out dOut);
            return dOut;
        }

        /// <summary>
        ///  Effettua il parse di un valore decimal.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Decimal ParseDecimal(string value)
        {
            Decimal dOut = default(Decimal);
            Decimal.TryParse(value, out dOut);
            return dOut;
        }

        /// <summary>
        /// Effettua il parse di un valore double.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ParseDouble(string value)
        {
            double dOut = default(double);
            double.TryParse(value, out dOut);
            return dOut;
        }

        /// <summary>
        /// Effettua il parse di un valore int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ParseInt(string value)
        {
            int iOut = default(int);
            int.TryParse(value, out iOut);
            return iOut;
        }

        /// <summary>
        /// Effettua il parse di un valore int.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="def">valore di default</param>
        /// <returns></returns>
        public static int ParseInt(string value, int def)
        {
            int iOut = def;
            int.TryParse(value, out iOut);
            return iOut;
        }

        /// <summary>
        /// Effettua il parse di un valore int nullable.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? ParseIntNullable(string value)
        {
            int iOut = default(int);
            if (int.TryParse(value, out iOut))
                return iOut;
            else
                return null;
        }

        /// <summary>
        ///  Effettua il parse di un valore datetime.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(string value)
        {
            DateTime dtOut = default(DateTime);
            DateTime.TryParse(value, out dtOut);
            return dtOut;
        }

        /// <summary>
        /// Restituisce la descrizione correlata a un valore di un enum, e.g.
        /// [Description("In lavorazione")]
        /// IN_LAVORAZIONE = 2,
        /// </summary>
        /// <param name="enumValue">Il valore dell'enum</param>
        /// <returns>Descrizione corrispondente al valore dell'enum</returns>
        public static string GetEnumDescription(Enum enumValue)
        {
            Type type = enumValue.GetType();
            MemberInfo[] memInfo = type.GetMember(enumValue.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumValue.ToString();
        }

        /// <summary>
        /// Indica se il valore di un enum è utilizzabile ai fini della ricerca.
        /// [IsSearchable(false)]
        /// IN_LAVORAZIONE = 2,
        /// </summary>
        /// <param name="enumValue">Il valore dell'enum</param>
        /// <remarks>di default è utilizzabile</remarks>
        /// <returns>true se il valore è utilizzabile come criterio di ricerca, altrimenti false</returns>
        public static bool IsEnumSearchable(Enum enumValue)
        {
            Type type = enumValue.GetType();
            MemberInfo[] memInfo = type.GetMember(enumValue.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(SearchableAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return (((SearchableAttribute)attrs[0]).IsSearchable);
                }
            }
            return true;
        }

        /// <summary>
        /// Formatta un giorno o mese in formato stringa a due cifre.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string FormatGiornoMese(string val)
        {
            if (!string.IsNullOrEmpty(val))
                return (val.Length == 1) ? "0" + val : val;
            else
                return val;
        }

        /// <summary>
        /// Verifica la validità di una data parziale
        /// sul presupposto che deve avere almeno l'anno,
        /// o l'anno e il mese, e che deve essere formalmente
        /// corretta nei dati parziali e concatenati.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="m"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool CheckDataComposta(string dayVal, string monthVal, string yearVal)
        {
            // verifiche su combinazione
            string c = (string.IsNullOrEmpty(dayVal)) ? "0" : "1";
            c += (string.IsNullOrEmpty(monthVal)) ? "0" : "1";
            c += (string.IsNullOrEmpty(yearVal)) ? "0" : "1";
            // tutto vuoto -> true
            if (c == "000")
                return true;
            // se non è completa, o anno o mese anno -> false
            if (c != "111" && c != "011" && c != "001")
                return false;
            // verifiche su regolarità data
            DateTime dt;
            string a = (string.IsNullOrEmpty(yearVal)) ? DateTime.Today.Year.ToString() : yearVal;
            string m = (string.IsNullOrEmpty(monthVal)) ? DateTime.Today.Month.ToString() : monthVal;
            string g = (string.IsNullOrEmpty(dayVal)) ? DateTime.Today.Day.ToString() : dayVal;
            string d = String.Format("{0}/{1}/{2}", g, m, a);
            if (!DateTime.TryParseExact(d, "dd/MM/yyyy",new CultureInfo("it-IT"),DateTimeStyles.None, out dt))
                return false;
            return true;
        }
    }
}

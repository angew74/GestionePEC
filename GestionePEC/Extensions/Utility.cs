using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace GestionePEC.Extensions
{
    public class Utility
    {
        public const int Ki_NASCITA_ANNOMIN = 1900;
        public const int Ki_NASCITA_ANNOMAX = -1;       // If less than 0, current year is assumed.
        public const String Ks_DATETIME_CULTURE_NAME = "it-IT";
        public static String csCultureName = Ks_DATETIME_CULTURE_NAME;

        public static bool AnnoNascitaCheck(String psValue)
        {
            int iAnno;


            // It is a number?
            if (int.TryParse(psValue, out iAnno) == false)
                return (false);

            // Bounded?
            if ((iAnno < Ki_NASCITA_ANNOMIN) || (iAnno > DateTime.Now.Year))
                return false;

            return (true);
        } // bool AnnoNascitaCheck(...


        public static String ConfigKeyValueGet(String psKey, String psDefault)
        {
            String sValue;


            try
            {
                sValue = System.Configuration.ConfigurationManager.AppSettings.Get(psKey);
                if (sValue == null)
                    sValue = psDefault;
            }
            catch
            {
                sValue = psDefault;
            }

            return (sValue);
        } // String ConfigKeyValueGet(...


        public const int Ki_RECORD_PER_PAGE = 5;
        public static int iRecordPerPage = -1;
        public static String RecordPerPageString()
        {

            if (iRecordPerPage < 0)
                RecordPerPage();
            return (iRecordPerPage.ToString());
        }

        public static int RecordPerPage()
        {
            String sRxP;
            int iRxP;

            if (iRecordPerPage < 0)
            {
                sRxP = ConfigKeyValueGet("RecordPerPage", Ki_RECORD_PER_PAGE.ToString());
                if (int.TryParse(sRxP, out iRxP) == false)
                    iRxP = Ki_RECORD_PER_PAGE;
                iRecordPerPage = iRxP;
            }
            return (iRecordPerPage);
        }


        /// <summary>
        /// Check if a date is valid.
        /// It uses a TryParse with CultureInfo( it-IT )
        /// 
        /// </summary>
        /// <param name="psDatePartial"></param>
        /// <returns></returns>
        public static bool IsDateValid(string psDatePartial)
        {
            //DateTime        dt;
            //return( DateTime.TryParse( psDatePartial, out dt ) );

            return (DateCheck(psDatePartial));
        } // bool IsDateValid(...


        // Naming style compatibility (name swapped)
        public static bool IsValidDate(string psDatePartial)
        {

            return (IsDateValid(psDatePartial));
        } // bool IsValidDate(...

        /// <summary>
        /// Return a string with the passed value packed as a DatePart ( leftpadded with zeroes, fixed length )
        /// </summary>
        /// <param name="piPart">1=day,2=month,3=year</param>
        /// <param name="psValue">A good numeric value</param>
        /// <returns>"0" either bad value or other problem encountered</returns>
        public static String DatePartFormat(Boolean pbPartialAllowed, int piPart, String psValue)
        {
            String psPart = "0";
            int iPart;
            int iValMin;
            int iValMax = -1;
            int iValLen = -1;


            if (psValue.Length >= 1)
            {
                switch (piPart)
                {
                    case 1:
                        iValMin = (pbPartialAllowed) ? 0 : 1;   // allowed if partial
                        iValMax = 31;
                        iValLen = 2;
                        break;
                    case 2:
                        iValMin = (pbPartialAllowed) ? 0 : 1;   // allowed if partial
                        iValMax = 12;
                        iValLen = 2;
                        break;
                    case 3:
                        iValMin = Ki_NASCITA_ANNOMIN;
                        if (Ki_NASCITA_ANNOMAX < 0)
                            iValMax = DateTime.Now.Year;
                        else
                            iValMax = Ki_NASCITA_ANNOMAX;
                        iValLen = 4;
                        break;
                    default:
                        iValMin = -1;
                        break;
                }

                if (iValMin >= 0)
                {

                    if (int.TryParse(psValue.Trim(), out iPart) == true)
                    {
                        if ((iPart < iValMin) || (iPart > iValMax))
                        {
                        }
                        else
                            psPart = iPart.ToString().PadLeft(iValLen, '0');
                    }
                }
            }
            return (psPart);
        } // String DatePartFormat(...


        public static String DateSeparatorGet()
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();

            return (dtfi.DateSeparator);
        } // String DateSeparatorGet()


        /// Allow a partial date string to be returned.
        /// Since DatePartFormat return a "0" if something failed, pls check the result's length to catch error(s).
        /// The result string is NOT checked.
        /// WARNING: format is fixed: dd/MM/yyyy
        public static String DateStringGet(Boolean pbPartialAllowed, String psDay, String psMonth, String psYear)
        {
            String sDate;
            String sDateSep;


            sDateSep = DateSeparatorGet();
            sDate = DatePartFormat(pbPartialAllowed, 1, psDay) + sDateSep + DatePartFormat(pbPartialAllowed, 2, psMonth) + sDateSep + DatePartFormat(pbPartialAllowed, 3, psYear);

            return (sDate);
        } // String DateStringGet(...

        internal static string FormatDataG_M_AtoMapper(string data, string v)
        {
            throw new NotImplementedException();
        }

        public static Boolean DateCheck(String psDate)
        {
            Boolean bRetVal = false;
            DateTime dt;
            CultureInfo ci;


            psDate = psDate.Trim();
            if (psDate.Length < 5)
                return (false);
            ci = new CultureInfo(csCultureName);
            bRetVal = DateTime.TryParse(psDate, ci, DateTimeStyles.None, out dt);
            return (bRetVal);
        } // Boolean DateCheck(...


        public static Boolean DateConvert(String psDate, out DateTime pdt)
        {
            CultureInfo ci;
            Boolean bRetVal;


            pdt = DateTime.MinValue;
            psDate = psDate.Trim();
            if (psDate.Length < 5)
                return (false);
            ci = new CultureInfo(csCultureName);
            bRetVal = DateTime.TryParse(psDate, ci, DateTimeStyles.None, out pdt);
            return (bRetVal);
        } // Boolean DateConvert(...


        /// <summary>
        /// Take a partial date ( i.e.: '00/01/2011' '/01/2011' '//2011' )
        ///   and do an easy check.
        /// Separators MUST always be specified.
        /// Valid separators are '-', ' ', '/', '.'
        /// </summary>
        /// <param name="psDatePartial"></param>
        /// <returns>true if the specified parts of the passed date are commonly accepted. Otherwise false.</returns>
        public static bool IsDatePartialValid(string psDatePartial)
        {
            Boolean bRetValue = false;
            char[] rgDateSep = { '-', ' ', '/', '.' };
            String[] rgDateParts;
            String sDate;
            String sDP;
            int iDP;
            String sDateSep;
            //DateTime        dt;


            try
            {
                psDatePartial = psDatePartial.Trim();
                if (psDatePartial.Length < 1)
                    return (bRetValue);
                rgDateParts = psDatePartial.Split(rgDateSep);
                sDate = "";
                sDateSep = DateSeparatorGet();

                sDP = rgDateParts[0];
                if (sDP.Length < 1)
                    sDP = "01";
                if (!int.TryParse(sDP, out iDP))
                    return (bRetValue);
                sDate += sDP;

                sDP = rgDateParts[1];
                if (sDP.Length < 1)
                    sDP = "01";
                if (!int.TryParse(sDP, out iDP))
                    return (bRetValue);
                sDate += sDateSep + sDP;

                sDP = rgDateParts[2];
                if (sDP.Length < 1)
                    sDP = "2000";
                if (!int.TryParse(sDP, out iDP))
                    return (bRetValue);
                sDate += sDateSep + sDP;
                //bRetValue           = DateTime.TryParse( sDate, out dt );
                bRetValue = DateCheck(sDate);
            }
            catch
            {
                return (bRetValue);
            }

            return (bRetValue);
        } // bool IsDatePartialValid(...       


        /// <summary>
        /// When passed as parameter to Mapper, the partial date puts zeroes instead missing date part.
        /// </summary>
        /// <param name="psDatePartial"></param>
        /// <returns></returns>
        public static String DatePartialToParam(string psDatePartial)
        {
            char[] rgDateSep = { '-', ' ', '/', '.' };
            String[] rgDateParts;
            String sRetValue = "";       // default value in case of error
            String sDate;
            String sDP;
            int iDP;
            String sDateSep;


            try
            {
                psDatePartial = psDatePartial.Trim();
                if (psDatePartial.Length < 1)
                    return (sRetValue);
                rgDateParts = psDatePartial.Split(rgDateSep);
                sDate = "";
                sDateSep = DateSeparatorGet();

                sDP = rgDateParts[0];
                if (sDP.Length < 1)
                    sDP = "00";
                if (!int.TryParse(sDP, out iDP))
                    return (sRetValue);
                sDate += sDP;

                sDP = rgDateParts[1];
                if (sDP.Length < 1)
                    sDP = "00";
                if (!int.TryParse(sDP, out iDP))
                    return (sRetValue);
                sDate += sDateSep + sDP;

                sDP = rgDateParts[2];
                if (sDP.Length < 1)
                    sDP = "0000";
                if (!int.TryParse(sDP, out iDP))
                    return (sRetValue);
                sDate += sDateSep + sDP;
                sRetValue = sDate;
            }
            catch
            {
                return (sRetValue);
            }

            return (sRetValue);
        } // String DatePartialToParam(...       


        /// <summary>
        /// Take a date ( i.e.: '00/01/2011' '/01/2011' '//2011' ) and verify that all parts of the date are specified.
        /// If a part of the date is set to zero ( '00/01/2000' ), the date is PARTIALLY specified.
        /// Separators MUST always be specified.
        /// Valid separators are '-', ' ', '/', '.'
        /// </summary>
        /// <param name="psDatePartial"></param>
        /// <returns>true if not all parts of the passed date have been specified. Otherwise false.</returns>
        public static bool IsDatePartial(string psDatePartial)
        {
            Boolean bRetValue = true;         // the worst case
            char[] rgDateSep = { '-', ' ', '/', '.' };
            String[] rgDateParts;
            String sDP;
            int iDP;


            try
            {
                psDatePartial = psDatePartial.Trim();
                if (psDatePartial.Length < 1)
                    return (bRetValue);
                rgDateParts = psDatePartial.Split(rgDateSep);

                sDP = rgDateParts[0];
                if (sDP.Length < 1)
                    return (bRetValue);
                if (!int.TryParse(sDP, out iDP))
                    return (bRetValue);
                if (iDP == 0)
                    return (bRetValue);

                sDP = rgDateParts[1];
                if (sDP.Length < 1)
                    return (bRetValue);
                if (!int.TryParse(sDP, out iDP))
                    return (bRetValue);
                if (iDP == 0)
                    return (bRetValue);

                sDP = rgDateParts[2];
                if (sDP.Length < 1)
                    return (bRetValue);
                if (!int.TryParse(sDP, out iDP))
                    return (bRetValue);
                if (iDP == 0)
                    return (bRetValue);

                bRetValue = false;
            }
            catch
            {
                return (bRetValue);
            }

            return (bRetValue);
        } // bool IsDatePartialValid(...       

        // ---------------------------------------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------------------------------------- //


        public static void Mandatory_LookAndFeel_Setup(ref Label plbl, Boolean pbMandatory)
        {
            FontInfo fi;

            fi = plbl.Font;
            fi.Bold = pbMandatory;
            plbl.Font.MergeWith(fi);
        } // void Mandatory_LookAndFeel_Setup(...
    }
}
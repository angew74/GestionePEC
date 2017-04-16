using ActiveUp.Net.Common.DeltaExt;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Mapping
{
    public class LinqExtensions

    {


        internal static string TryParseBody(ActiveUp.Net.Mail.MimeBody mimeBody)
        {
            string m = string.Empty;
            if (mimeBody != null && !string.IsNullOrEmpty(mimeBody.TextStripped))
            {
                m = mimeBody.TextStripped;
            }
            else if (m != null && !String.IsNullOrEmpty(mimeBody.TextStripped))
            {
                m = mimeBody.TextStripped;
            }
            return m;
        }

        internal static int? TryParseFollows(System.Collections.Specialized.NameValueCollection m)
        {
            int idOld = -1;
            if (!string.IsNullOrEmpty(m["X-Ricevuta"]) &&
                 !m["X-Ricevuta"].Equals("posta-certificata") &&
                 !String.IsNullOrEmpty(m["X-Riferimento-Message-ID"]))
            {
                string idOldString = m["X-Riferimento-Message-ID"];
                if (idOldString.StartsWith("<"))
                    idOldString = idOldString.Substring(1);
                if (idOldString.EndsWith(">"))
                    idOldString = idOldString.Substring(0, idOldString.Length - 1);
                string[] idOldStr = idOldString.Split('.');
                if (idOldStr.Length > 0 && int.TryParse(idOldStr[0], out idOld))
                {
                    return idOld;
                }
                else
                {
                    return null;
                }
            }
            return idOld;
        }
        public static int TryParseInt(long? id)
        {
            int val = 0;
            if (id.HasValue)
            {
                val = int.Parse(id.ToString());
            }
            return val;
        }

        public static decimal TryParseDecimal(decimal? id)
        {
            decimal val = 0;
            if (id.HasValue)
            {
                val = int.Parse(id.ToString());
            }
            return val;
        }

        internal static bool TryParseBool(string p)
        {
            bool ret = false;
            if (p == "0")
            { ret = false; }
            else if (p == "1")
            { ret = true; }
            return ret;
        }

        internal static string TryParseString(bool value)
        {
            string v = "0";
            if (value == true)
            { v = "1"; }
            return v;
        }

        internal static byte TryParseByte(string p)
        {
            byte r = byte.MinValue;
            if (string.IsNullOrEmpty(p))
            {
                r = Convert.ToByte(p);
            }
            return r;
        }

        internal static int TryParseInt(decimal? id)
        {
            int val = 0;
            if (id.HasValue)
            {
                val = int.Parse(id.ToString());
            }
            return val;
        }

        internal static Model.AllegatoProtocolloStatus TryParsEnumProt(string p)
        {
            AllegatoProtocolloStatus status = AllegatoProtocolloStatus.UNKNOWN;
            switch (p)
            {
                case "0":
                    status = AllegatoProtocolloStatus.FALSE;
                    break;
                case "1":
                    status = AllegatoProtocolloStatus.TRUE;
                    break;
                case "2":
                    status = AllegatoProtocolloStatus.DONE;
                    break;
            }
            return status;
        }


        internal static ProtocolloStatus TryParsEnumFlusso(string p)
        {
            ProtocolloStatus status = ProtocolloStatus.UNKNOWN;
            switch (p)
            {
                case "0":
                    status = ProtocolloStatus.DA_PROTOCOLLARE;
                    break;
                case "1":
                    status = ProtocolloStatus.ERRORE_PROTOCOLLAZIONE;
                    break;
                case "2":
                    status = ProtocolloStatus.PROTOCOLLATA;
                    break;
                case "3":
                    status = ProtocolloStatus.ALLEGARE_DOCUMENTI;
                    break;
                case "4":
                    status = ProtocolloStatus.ERRORE_DOCUMENTI_ALLEGATI;
                    break;
                case "5":
                    status = ProtocolloStatus.DOCUMENTI_ALLEGATI;
                    break;
            }
            return status;

        }

        internal static MailStatus TryParsEnumStatus(string p)
        {
            MailStatus status = MailStatus.UNKNOWN;
            Enum.TryParse(p, out status);
            return status;
        }

        internal static EntitaType TryParseEntitaType(string p)
        {
            EntitaType entita = EntitaType.UNKNOWN;
            Enum.TryParse(p, out entita);
            return entita;
        }

        internal static AddresseeType TryParsEnumAddressee(string p)
        {
            AddresseeType type = AddresseeType.UNDEFINED;
            Enum.TryParse(p, out type);
            return type;
        }

        internal static long TryParseLong(decimal id)
        {
            long val = 0;
            if (id != 0 )
            {
                val = int.Parse(id.ToString());
            }
            return val;
        }

        internal static decimal? TryParseLong(double id)
        {
            decimal val = 0;
            if (id != 0)
            {
                val = int.Parse(id.ToString());
            }
            return val;
        }

        internal static double TryParseDouble(decimal? id)
        {
            double val = 0;
            if (id.HasValue)
            {
                val = int.Parse(id.ToString());
            }
            return val;
        }

        internal static decimal TryParseDecimalString(string v)
        {
          return  decimal.Parse(v);
        }

        internal static int TryParseIntFromDouble(double? id)
        {
            int val = 0;
            if (id.HasValue)
            {
                val = int.Parse(id.ToString());
            }
            return val;
        }

        internal static long? TryParseLongNullable(decimal? iD_REFERRAL)
        {
            long val = 0;
           if(iD_REFERRAL != null)
            { val = long.Parse(iD_REFERRAL.ToString()); }
            return val;
        }

        internal static bool TryParseBoolDecimal(decimal? sOTTOTITOLO_ACTIVE)
        {
            bool val = false;
           if(sOTTOTITOLO_ACTIVE == 0)
            { val = false; }
           else { val = true; }
            return val;
        }

        internal static decimal? TryParseDecimalBool(bool deleted)
        {
            int val = 0;
            if (deleted)
            { val = 1; }
            return val;
        }
     
    }
}




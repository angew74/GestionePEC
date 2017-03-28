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
            throw new NotImplementedException();
        }
    }
}




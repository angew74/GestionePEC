using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

//namespace Com.Delta.Data.Utility
// rende disponibile l'estensione a tutti gli oggetti che conoscono IDataRecord 
// e nel cui contesto è presente questo assembly
namespace System.Data       
{
    /// <summary>
    /// Classe statica che estende i metodi dell'IDataRecord.
    /// </summary>
    public static class IDataRecordExtension
    {
        #region "Private methods"

        private static int GetIndex(IDataRecord dr, string field)
        {
            try
            {
                return dr.GetOrdinal(field);
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
        }

        #endregion

        #region "Extension methods"

        public static Boolean IsDBNull(this IDataRecord dr, string field)
        {
            try
            {
                int index = dr.GetOrdinal(field);
                return dr.IsDBNull(index);
            }
            catch
            {
                throw;
            }
        }

        public static Int16 GetInt16(this IDataRecord dr, string field)
        {
            Int16 result = 0;
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                    result = dr.GetInt16(index);
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }

        public static Int32 GetInt32(this IDataRecord dr, string field)
        {
            Int32 result = 0;
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                    result = dr.GetInt32(index);
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }

        public static float GetFloat(this IDataRecord dr, string field)
        {
            float result = 0;
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                    result = dr.GetFloat(index);
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }


        public static string GetString(this IDataRecord dr, string field)
        {
            string result = string.Empty;
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                    result = dr.GetString(index);
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException)
            {
                //The name specified is not a valid column name. 

            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }

        public static bool GetBoolString(this IDataRecord dr, string field)
        {
            bool result = false;
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                {
                    bool.TryParse(dr.GetString(index), out result);
                }
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }

        public static bool GetBoolChar(this IDataRecord dr, string field)
        {
            bool result = false;
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                {
                    string c = dr.GetString(index).ToLower();
                    if (c == "s" || c == "t" || c == "1")
                        result = true;
                }
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }

        public static Int64 GetInt64(this IDataRecord dr, string field)
        {
            Int64 result = default(Int64);
            try
            {

                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                    result = dr.GetInt64(index);
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }

        public static decimal GetDecimal(this IDataRecord dr, string field)
        {
            decimal result = default(decimal);
            try
            {

                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                    result = dr.GetDecimal(index);
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }

        public static double GetDouble(this IDataRecord dr, string field)
        {
            double result = default(double);
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                    result = dr.GetDouble(index);
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }

        public static DateTime GetDateTime(this IDataRecord dr, string field)
        {
            DateTime result = default(DateTime);
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                    result = dr.GetDateTime(index);
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return result;
        }

        public static Byte[] GetBytes(this IDataRecord dr, string field)
        {
            Byte[] blob = default(Byte[]);
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                {
                    blob = new Byte[(dr.GetBytes(index, 0, null, 0, int.MaxValue))];
                    dr.GetBytes(index, 0, blob, 0, blob.Length);
                }
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (IndexOutOfRangeException ie)
            {
                //The name specified is not a valid column name. 
                throw ie;
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/
            catch (InvalidCastException ice)
            {
                //The specified cast is not valid. 
                throw ice;
            }
            return blob;
        }

        public static Object GetValue(this IDataRecord dr, string field)
        {
            Object obj = default(object);
            try
            {
                int index = dr.GetOrdinal(field);
                if (!dr.IsDBNull(index))
                {
                    obj = dr.GetValue(index);
                }
            }
            catch (InvalidCastException ex)
            {
                throw ex;
            }
            return obj;
        }

        public static Nullable<T> GetNullableValue<T>(this IDataRecord dr, string field)
            where T : struct
        {
            Nullable<T> obj = null;
            if (dr.IsDBNull(field) == false)
                obj = (T)dr.GetValue(field);
            return obj;
        }
        #endregion
    }
}

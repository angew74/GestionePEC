using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using SendMail.Model;
using Oracle.DataAccess.Client;

namespace SendMail.Data.Utilities
{
    public class DataMapper
    {
        public static object FillObject(IDataRecord r, Type ItemType)
        {
            object fillObject = Activator.CreateInstance(ItemType);

            DatabaseFieldAttribute customAttr;
            int ordinal = 0;

            object[] attributes;
            PropertyInfo[] itemTypeProperties = ItemType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            PropertyInfo property;
            for (int i = 0; i < itemTypeProperties.Length; i++)
            {
                property = itemTypeProperties[i];
                attributes = property.GetCustomAttributes(typeof(DatabaseFieldAttribute), true);

                if (attributes != null && attributes.Length == 1)
                {
                    customAttr = (DatabaseFieldAttribute)attributes[0];
                    if (customAttr != null && customAttr.FieldName.Length > 0)
                    {
                        try
                        {
                            ordinal = r.GetOrdinal(customAttr.FieldName);
                        }
                        catch
                        {
                            ordinal = -1;
                        }
                        if (ordinal >= 0)
                        {
                            Type tProperty = property.PropertyType;
                            if (tProperty.IsValueType || tProperty == typeof(String) || tProperty.IsArray)
                            {
                                if (!r.IsDBNull(ordinal))
                                {
                                    object val = r.GetValue(ordinal);
                                    if (val.GetType().Equals(tProperty))
                                    {
                                        property.SetValue(fillObject, val, null);
                                    }
                                    else if (tProperty.IsEnum)
                                    {
                                        property.SetValue(fillObject, Enum.Parse(tProperty, val.ToString(), true), null);
                                    }
                                    else
                                    {
                                        if (tProperty.IsAssignableFrom(val.GetType()))
                                        {
                                            property.SetValue(fillObject, val, null);
                                        }
                                        else
                                        {
                                            try
                                            {
                                                object newVal = Convert.ChangeType(val, tProperty);
                                                property.SetValue(fillObject, newVal, null);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                }
                            }
                            else if (tProperty.IsClass)
                            {
                                if (tProperty == typeof(DateTime))
                                {
                                    ConstructorInfo constrInfo = tProperty.GetConstructor(new Type[] { typeof(DateTime) });
                                    property.SetValue(fillObject, constrInfo.Invoke(new object[] { r.GetDateTime(ordinal) }), null);
                                }
                            }
                            else
                            {
                                throw new NotImplementedException("Caso non implementato");
                            }
                        }
                    }
                }
            }

            return fillObject;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Datos.Connection
{
    public static class DataEntityMapper
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();

            try
            {
                foreach (var property in properties)
                {
                    if (row.Table.Columns.Contains(property.Name))
                    {
                        if (property.PropertyType == typeof(DayOfWeek))
                        {
                            DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                            property.SetValue(item, day, null);
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            int entero = 0;
                            int.TryParse(row[property.Name].ToString(), out entero);
                            property.SetValue(item, entero, null);
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            DateTime fecha = new DateTime();
                            DateTime.TryParse(row[property.Name].ToString(), out fecha);
                            property.SetValue(item, fecha, null);
                        }
                        else if (property.PropertyType == typeof(long))
                        {
                            long entero = 0;
                            long.TryParse(row[property.Name].ToString(), out entero);
                            property.SetValue(item, entero, null);
                        }
                        else if (property.PropertyType == typeof(Boolean))
                        {
                            bool booleano = false;
                            bool.TryParse(row[property.Name].ToString(), out booleano);
                            property.SetValue(item, booleano, null);
                        }
                        else
                        {
                            if (row[property.Name] == null || row[property.Name] == DBNull.Value)
                            {
                                property.SetValue(item, null, null);
                            }
                            else
                            {
                                property.SetValue(item, row[property.Name].ToString(), null);
                            }
                        }
                    }
                    else
                    {
                        property.SetValue(item, null, null);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error: {0}", ex.Message));
                return default(T);
            }

            return item;
        }
    }
}

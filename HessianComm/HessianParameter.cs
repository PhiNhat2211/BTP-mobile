using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HessianComm
{
    public static class HessianParameter
    {
        public static Hashtable ToHessianHashtable(this object obj)
        {
            return ToHashtableFromObj(obj);
        }

        private static Hashtable ToHashtableFromObj(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            Hashtable table = new Hashtable();

            if (obj != null)
            {
                Type type = obj.GetType();

                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (info.PropertyType.IsFundamental() == true)
                    {
                        if (info.GetValue(obj, null) != null)
                        {
                            table.Add(info.Name, info.GetValue(obj, null));
                        }
                    }
                    else
                    {   
                        table.Add(info.Name, ToHashtableFromObj(info.GetValue(obj, null)));
                    }
                }
            }

            return table;
        }

        private static bool IsFundamental(this Type type)
        {
            return type.IsPrimitive || type.Equals(typeof(string)) || type.Equals(typeof(DateTime)) ||
                type.Equals(typeof(ArrayList)) || type.Equals(typeof(string[]));
        }
    }
}

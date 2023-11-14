using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WPCommon.Util
{
    public static class Ext
    {
        [Serializable]
        public class Singleton<T> where T : class, new()
        {
            private static volatile T instance = null;
            private static object syncRoot = new Object();

            protected Singleton() { }

            public static T Instance
            {
                get
                {
                    if (instance == null)
                    {
                        lock (syncRoot)
                        {
                            if (instance == null)
                                instance = new T();
                        }
                    }

                    return instance;
                }
            }
        }

        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                if (table == null)
                    return new List<T>();

                List<T> list = new List<T>();
                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            if (row.Table.Columns.Contains(prop.Name))
                            {
                                if (row[prop.Name].GetType() == typeof(System.DBNull))
                                {
                                    propertyInfo.SetValue(obj,
                                        propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null,
                                        null);
                                }
                                else
                                {
                                    if (propertyInfo.PropertyType.FullName.Equals("ObjectsMgr.Date"))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        propertyInfo.SetValue(obj,
                                            Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType),
                                            null);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Logger.WriteErrorLog(ex.GetBaseException());
                            continue;
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }

        public delegate bool CompareValue<in T1, in T2>(T1 value1, T2 value2);
        public static bool CompareTwoArrays<T1, T2>(this IEnumerable<T1> array1, IEnumerable<T2> array2, CompareValue<T1, T2> compareValue)
        {
            return array1.Select(item1 => array2.Any(item2 => compareValue(item1, item2))).All(search => search) &&
                   array2.Select(item2 => array1.Any(item1 => compareValue(item1, item2))).All(search => search);
        }
    }
}
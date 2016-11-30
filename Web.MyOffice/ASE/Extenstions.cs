using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace ASE
{
    public static class Extenstions
    {
        public static string ToStringLower(this bool b)
        {
            return b.ToString().ToLower();
        }
    }

    public static class ExtenstionsDateTime
    {

        public static string DateTimeToStringDT = "dd/MM/yyyy HH:mm:ss";
        public static string ToStringDT(this DateTime dt)
        {
            return dt.ToString(DateTimeToStringDT);
        }

        public static string DateTimeToStringD = "dd/MM/yyyy";
        public static string ToStringD(this DateTime dt)
        {
            return dt.ToString(DateTimeToStringD);
        }

        public static string DateTimeToStringT = "HH:mm:ss";
        public static string ToStringT(this DateTime dt)
        {
            return dt.ToString(DateTimeToStringT);
        }

        public static DateTime DateTimeFrom(this DateTime? dt, DateTime value)
        {
            if (dt.HasValue)
                return dt.Value;

            return value;
        }

        public static DateTime DateTimeTo(this DateTime? dt, DateTime value)
        {
            if (dt.HasValue)
                return dt.Value.Date.AddDays(1).AddMilliseconds(-1);

            return value.Date.AddDays(1).AddMilliseconds(-1);
        }
    }

    public static class ExtStrings
    {
        public static string Right(this string str, int count)
        {
            if ((str == null) || (str.Length <= count))
                return str;

            return str.Substring(str.Length - count, count);
        }

        public static string Left(this string str, int count)
        {
            if ((str == null) || (str.Length <= count))
                return str;

            return str.Substring(0, count);
        }
    }

    public static class ExtGuid 
    {
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        public static bool IsNotEmpty(this Guid guid)
        {
            return guid != Guid.Empty;
        }
    }

    public static class ExtensionsOject
    {
        public static string NullToString(this Object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        public static string NullToString(this Object obj, string str)
        {
            return obj == null ? str : obj.ToString();
        }

        public static decimal AsDecimal(this Object obj)
        {
            return obj == null ? 0 : (decimal) obj;
        }

        public static TClass As<TClass>(this Object obj) where TClass: class 
        {
            return (obj as TClass);
        }
        public static IDictionary<string, object> AddProperty(this object obj, string name, object value)
        {
            var dictionary = obj.ToDictionary();
            dictionary.Add(name, value);
            return dictionary;
        }

        // helper
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor property in properties)
            {
                result.Add(property.Name, property.GetValue(obj));
            }
            return result;
        }

        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            return expando as ExpandoObject;
        }
    }
}
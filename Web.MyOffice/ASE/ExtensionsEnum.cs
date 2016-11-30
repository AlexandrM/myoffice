using ASE.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASE
{
    public static class ExtensionsEnum
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
                    where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e, Name = e.ToDisplayName() };

            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static SelectList ToSelectListIntId<TEnum>(this TEnum enumObj)
                    where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = Convert.ChangeType(e, TypeCode.Int32), Name = e.ToDisplayName() };

            return new SelectList(values, "Id", "Name", enumObj);
        }
    }
}
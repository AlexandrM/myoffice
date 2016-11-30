using ASE.EF;
using ASE.ToolsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Data.Entity;

using ASE.MVC;

using Web.MyOffice.Models;

namespace Web.MyOffice.Data
{
    public static class CurrencyExtensions
    {
        public static IEnumerable<SelectListItem> SelectListItems(this List<Currency> currencies)
        {
            return new SelectList(currencies, "Id", "Name");
        }

        public static CurrencyRate CurrencyRate(this Currency currency)
        {
            return ContextPerRequest<DB>.Db.CurrencyRates.Where(x => x.CurrencyId == currency.Id).OrderByDescending(x => x.DateTime).Take(1).FirstOrDefault() ?? new CurrencyRate { Value = 1 };
        }

        public static Currency CurrencyByUser(this Currency currency, DB db)
        {
            var userId = HttpContext.Current.UserId();
            return db.Currencies.FirstOrDefault(x => x.UserId == userId & x.CurrencyType == currency.CurrencyType);
        }

        public static Currency LocalCurrency(this Currency currency, DB db)
        {
            var userId = HttpContext.Current.UserId();
            if (currency.UserId == userId)
                return currency;

            return db.Currencies.FirstOrDefault(x => x.UserId == userId & x.CurrencyType == currency.CurrencyType);
        }
    }

    public static class MemberRateExtensions
    {
    }

    /*public static class MyMemberExtensions
    {
        public static Currency DefaultCurrency(this Member meber, DB db)
        {
            var userId = HttpContext.Current.UserId();
            return db.Currencies.FirstOrDefault(x => x.UserId == userId & x.CurrencyRates.Count() == 0) ?? new Currency();
        }
    }*/

    public static class ImplementerExtensions
    {
    }

    public static class MeberExtensions
    {
        /*public static List<MemberRate> MemberRates(this Member member, DB db) 
        {
            return db.MemberRates
                .AsNoTracking()
                .Include(x => x.Currency)
                .Where(x => x.MemberId == member.Id & x.UserId == member.UserId).ToList() ?? new List<MemberRate>();
        }

        public static MemberRate MemberRate(this Member member, DB db)
        {
            return MemberRate(member, db, DateTime.Now);
        }

        public static MemberRate MemberRate(this Member member, DB db, Currency currency)
        {
            return MemberRate(member, db, DateTime.Now, currency);
        }
z
        public static MemberRate MemberRate(this Member member, DB db, DateTime date)
        {
            return db.MemberRates.Where(x => x.MemberId == member.Id & x.DateTime < date).OrderBy(x => x.DateTime).Take(1).FirstOrDefault();
        }

        public static MemberRate MemberRate(this Member member, DB db, DateTime date, Currency currency)
        {
            return db.MemberRates.Where(x => x.MemberId == member.Id & x.CurrencyId == currency.Id & x.DateTime < date).OrderBy(x => x.DateTime).Take(1).FirstOrDefault();
        }*/

        public static Member LocalMember(this Member member, DB db, Guid userId)
        {
            if (member.UserId != userId)
                return db.Members.FirstOrDefault(x => x.UserId == userId & x.MainMemberId == member.MainMemberId);
 
            return member;
        }

        /*public static Member MyMember(this Member member, DB db)
        {
            var userId = HttpContext.Current.UserId();
            return db.Members.Where(x => x.MemberId == member.Id & x.UserId == userId).FirstOrDefault();
        }*/
    }

    public static class EFModelExtensions
    {
        public static ToolModelIdValue SelectResult(this EFModel model)
        {
            object[] attrs = model.GetType().GetCustomAttributes(typeof(DisplaybleAttribute), true);
            var prop = (attrs[0] as DisplaybleAttribute).Displayble;

            return new ToolModelIdValue
            {
                Id = model.Id.ToString(),
                Value = model.GetType().GetProperty(prop).GetValue(model).ToString()
            };
        }
    }

    public static class AttachmentExtensions
    {
        public static string FilePath(this Attachment model)
        {
            FileInfo fi = new FileInfo(model.OriginalName);
            return String.Format(@"\{0}\{1}\{2}\", 
                model.CreateDate.Year, 
                model.CreateDate.Month, 
                model.CreateDate.Day);
        }
        public static string FileName(this Attachment model)
        {
            FileInfo fi = new FileInfo(model.OriginalName);
            return String.Format(@"{0}{1}",
                model.Id,
                fi.Extension);
        }
    }
}
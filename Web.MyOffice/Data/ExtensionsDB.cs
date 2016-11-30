using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using Web.MyOffice.Models;
using System.Data.Entity;
using System.Globalization;

namespace Web.MyOffice.Data
{
    public static class ExtensionsDB
    {
        public static TEntity AddDefault<TEntity>(this DbSet<TEntity> dbset, TEntity value) where TEntity : class
        {
            var prop = value.GetType().GetProperty("UserId");
            if (prop != null)
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                    prop.SetValue(value, Guid.Parse(System.Web.HttpContext.Current.User.Identity.GetUserId()));

            dbset.Add(value);

            return value;
        }

        public static Currency DefaultCurrency(this DB db, Guid? userId = null)
        {
            Guid UserId;
            if (userId.HasValue)
                UserId = userId.Value;
            else
                UserId = Guid.Parse(System.Web.HttpContext.Current.User.Identity.GetUserId());

            var curr = db.Currencies.FirstOrDefault(x => x.UserId == UserId & x.CurrencyRates.Count() == 0);
            if (curr == null)
            {
                if (db.Currencies.Count(x => x.UserId == UserId) == 0)
                {
                    CultureInfo cu = null;
                    if (System.Web.HttpContext.Current.Request.UserLanguages.Count() != 0)
                        cu = new CultureInfo(System.Web.HttpContext.Current.Request.UserLanguages[0]);

                    var c = db.Currencies.AddDefault(new Currency { Name = "UAH", ShortName = "UAH", CurrencyType = CurrencyType.UAH, UserId = UserId, Value = 1 });
                    if ((cu != null) && (cu.Name != "uk"))
                        db.CurrencyRates.Add(new CurrencyRate { CurrencyId = c.Id, DateTime = DateTime.Now, Value = 1 });
                    else
                        curr = c;

                    c = db.Currencies.AddDefault(new Currency { Name = "RUR", ShortName = "RUR", CurrencyType = CurrencyType.RUR, UserId = UserId, Value = 1 });
                    if ((cu != null) && ((cu.Name != "ru") | (cu.Name != "ru-RU")))
                        db.CurrencyRates.Add(new CurrencyRate { CurrencyId = c.Id, DateTime = DateTime.Now, Value = 1 });
                    else
                        curr = c;

                    c = db.Currencies.AddDefault(new Currency { Name = "USD", ShortName = "USD", CurrencyType = CurrencyType.USD, UserId = UserId, Value = 1 });
                    if ((cu != null) && (cu.Name != "en-US"))
                        db.CurrencyRates.Add(new CurrencyRate { CurrencyId = c.Id, DateTime = DateTime.Now, Value = 1 });
                    else
                        curr = c;

                    c = db.Currencies.AddDefault(new Currency { Name = "EUR", ShortName = "EUR", CurrencyType = CurrencyType.EUR, UserId = UserId, Value = 1 });
                    if (curr != null)
                        db.CurrencyRates.Add(new CurrencyRate { CurrencyId = c.Id, DateTime = DateTime.Now, Value = 1 });

                    if (curr != null)
                        curr.MyCurrency = true;
                    db.SaveChanges();
                }
            }

            return curr;
        }

        public static Member IAm(this DB db, Guid? userId = null)
        {
            Guid UserId;
            if (userId.HasValue)
                UserId = userId.Value;
            else
                UserId = Guid.Parse(System.Web.HttpContext.Current.User.Identity.GetUserId());

            return db.Members.FirstOrDefault(x => x.Id == UserId & x.UserId == UserId);
        }
    }
}
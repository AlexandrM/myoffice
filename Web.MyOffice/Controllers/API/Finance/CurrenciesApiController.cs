using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data.Entity;

using ASE;
using ASE.MVC;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using MyBank.Models;
using System.Web.Http;

namespace Web.MyOffice.Controllers.API
{
    public class CurrenciesController: ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage CurrencyListGet()
        {
            var model = new {
                currencies = db.Currencies
                    .Where(currency => currency.UserId == UserId)
                    .OrderBy(x => x.Name)
                    .ToList(),

                types = Enum.GetValues(typeof(CurrencyType))
                    .Cast<CurrencyType>()
                    .Select(x => new { Id = x, Name = Enum.GetName(typeof(CurrencyType), x) }),

                warnings = new List<string>() {
                    R.R.WarningExists,
                    R.R.CurrencyWarning
                },
            };

            return ResponseObject2Json(model);
        }

        [HttpGet]
        public HttpResponseMessage CurrenciesUpdate(string sourceName)
        {
            /*var rateLoader = new CurrencyRateLoader(db);
            if (rateLoader.UpdateRates(sourceName))
            {
                return ResponseObject2Json(HttpStatusCode.OK);
            }
            else
            {
                return ResponseObject2Json(HttpStatusCode.NotModified);
            }*/
            return ResponseObject2Json(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage CurrencyPost(Currency newCurrency)
        {
            using (db)
            {
                newCurrency.UserId = UserId;
                if (db.Currencies.Where(currency => currency.Id == newCurrency.Id && currency.UserId == UserId).Any())
                {
                    if (newCurrency.MyCurrency)
                    {
                        foreach (var currency in db.Currencies.Where(x => x.Id != newCurrency.Id && x.UserId == UserId && x.MyCurrency))
                        {
                            currency.MyCurrency = false;
                        }
                    }
                    db.Entry(newCurrency).State = EntityState.Modified;
                }
                else
                {
                    db.Entry(newCurrency).State = EntityState.Added;
                }
                db.SaveChanges();

                return ResponseObject2Json(HttpStatusCode.Accepted);
            }
        }

        [HttpGet]
        public HttpResponseMessage CurrencyArchive(Guid currencyId, bool deleted)
        {
            var delcurrency = db.Currencies.FirstOrDefault(x => x.Id == currencyId && x.UserId == UserId);
            delcurrency.IsArchive = deleted;
            db.SaveChanges();

            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        [HttpDelete]
        public HttpResponseMessage CurrencyDelete(Guid currencyId)
        {
            var delAccounts = db.Accounts
                .Include(acc => acc.Currency)
                .Where(acc => acc.CurrencyId == currencyId & acc.Budget.OwnerId == UserId)
                .ToList();

            if (delAccounts.Any())
            {
                return ResponseObject2Json(new {
                    result = false,
                    message = R.R.CantDeleteCurrency1
                });
            }

            var m = db.Currencies.FirstOrDefault(x => x.Id == currencyId && x.UserId == UserId);
            db.Currencies.Remove(m);
            db.SaveChanges();

            return ResponseObject2Json(new {
                result = true
            });
        }
    }
}
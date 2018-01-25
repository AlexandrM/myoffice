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
                    .Select(x => new { Id = x, Name = Enum.GetName(typeof(CurrencyType), x) }).OrderBy(x=>x.Id),

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
            var rateLoader = new CurrencyRateLoader(db);
            if (rateLoader.UpdateRates(sourceName, UserId))
            {
                return ResponseObject2Json(HttpStatusCode.OK);
            }
            else
            {
                return ResponseObject2Json(HttpStatusCode.NotModified);
            }
        }

        [HttpPost]
        [Route("api/currencies/postCurrency")]
        public HttpResponseMessage PostCurrency(Currency currency)
        {
            var model = db.Currencies.FirstOrDefault(x => x.UserId == UserId && x.Id == currency.Id);
            if (model == null)
            {
                currency.Id = Guid.NewGuid();
                currency.UserId = UserId;
                db.Currencies.Add(currency);
            }
            else
            {
                model.Name = currency.Name;
                model.ShortName = currency.ShortName;
                model.CurrencyType = currency.CurrencyType;
                model.MyCurrency = currency.MyCurrency;
                if (currency.MyCurrency)
                {
                    foreach(var item in db.Currencies.Where(x => x.UserId == UserId))
                    {
                        item.MyCurrency = false;
                    }
                    db.SaveChanges();
                }
            }
            db.SaveChanges();

            return ResponseObject2Json(new {
                Currency = currency,
            });
        }

        [HttpPost]
        [Route("api/currencies/postCrrencyRate")]
        public HttpResponseMessage PostCrrencyRate(CurrencyRate rate) {
            var currency = db.Currencies.FirstOrDefault(x => x.Id == rate.CurrencyId);

            var lastRate = db.CurrencyRates
                .AsNoTracking()
                .OrderByDescending(x => x.DateTime)
                .Where(x => x.CurrencyId == currency.Id & x.DateTime <= rate.DateTime)
                .FirstOrDefault();

            if (lastRate == null || lastRate.Value != rate.Value)
            {
                rate.CurrencyId = currency.Id;
                db.CurrencyRates.Add(rate);
                db.SaveChanges();
            }

            lastRate = db.CurrencyRates
                .AsNoTracking()
                .OrderByDescending(x => x.DateTime)
                .Where(x => x.CurrencyId == currency.Id)
                .FirstOrDefault();
            currency.Value = lastRate.Value;
            db.SaveChanges();


            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        [HttpDelete]
        [Route("api/currencies/deleteCurrency")]
        public object DeleteCurrency([FromUri]Guid currencyId)
        {
            var currency = db.Currencies.FirstOrDefault(x => x.Id == currencyId && x.UserId == UserId);

            var acc = db.Accounts.FirstOrDefault(x => x.CurrencyId == currencyId);
            if (acc != null)
            {
                return new
                {
                    ok = false,
                    message = $"There are account '{acc.Name}' whith currency '{currency.Name}'",
                };
            }
            var mr = db.MemberRates.Include(x => x.Member).FirstOrDefault(x => x.CurrencyId == currencyId);
            if (mr != null)
            {
                return new
                {
                    ok = false,
                    message = $"There are member '{mr.Member.FullName}' whith currency '{currency.Name}'",
                };
            }

            db.CurrencyRates.RemoveRange(db.CurrencyRates.Where(x => x.CurrencyId == currency.Id));
            db.Currencies.Remove(currency);
            db.SaveChanges();

            return new
            {
                ok = true,
                message = "",
            };
        }


        [HttpGet]
        [Route("api/currencies/ratesUpdate")]
        public object RatesUpdate([FromUri] string name)
        {
            var cl = new CurrencyRateLoader(db);
            cl.UpdateRates(name, UserId);

            return new
            {
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Data.Entity;
using System.Dynamic;

using ASE;
using ASE.EF;
using ASE.MVC;
using ASE.Json;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MyBank.Models;
using MVC = Web.MyOffice.Controllers.MyBank;
using Method = System.Web.Http;

namespace Web.MyOffice.Controllers.API
{
    public class CurrenciesController: ControllerApiAdv<DB>
    {
        [Method.HttpGet]
        public HttpResponseMessage CurrencyListGet()
        {
            var model = (new object()).ToDynamic();
            model.currencies = db.Currencies.ToList();
            model.types = Enum.GetNames(typeof(CurrencyType));
            model.warnings = new List<string>() {R.R.WarningExists };
            return ResponseObject2Json(model);
        }

        [Method.HttpGet]
        public HttpResponseMessage CurrenciesUpdate(string sourceName)
        {
            var rateLoader = new CurrencyRateLoader(db);
            if (rateLoader.UpdateRates(sourceName))
            {
                return ResponseObject2Json(HttpStatusCode.OK);
            }
            else
            {
                return ResponseObject2Json(HttpStatusCode.NotModified);
            }
        }

        [Method.HttpPost]
        public HttpResponseMessage CurrencyPost(Currency newCurrency)
        {
            using (db)
            {
                newCurrency.UserId = UserId;
                var currencies = db.Currencies.Where(currency => currency.Id == newCurrency.Id);
                if (currencies.Count() == 1)
                {
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

        [Method.HttpDelete]
        public HttpResponseMessage CurrencyDelete(Guid delCurrencyId)
        {
            using (db) {
                var deletedCurrency = db.Currencies.Where(currency => currency.Id == delCurrencyId).FirstOrDefault();

                db.Entry(deletedCurrency).State = EntityState.Deleted;
                try
                {
                    db.SaveChanges();
                    return ResponseObject2Json(HttpStatusCode.Accepted);
                }
                catch
                {
                    return ResponseObject2Json(HttpStatusCode.NotModified);
                }
            }
        }
    }
}
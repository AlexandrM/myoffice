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
            model.currencies = db.Currencies.Where(currency=>currency.UserId == UserId).ToList();
            model.types = Enum.GetNames(typeof(CurrencyType));
            model.warnings = new List<string>() { R.R.WarningExists, R.R.CurrencyWarning };
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

        [Method.HttpGet]
        public HttpResponseMessage CurrencyArchive(Guid currencyId, bool deleted)
        {
            CurrencyModify(currencyId, EntityState.Modified, deleted);
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        [Method.HttpDelete]
        public HttpResponseMessage CurrencyDelete(Guid currencyId)
        {
            CurrencyModify(currencyId, EntityState.Deleted, true);
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        public void CurrencyModify(Guid currencyId, EntityState state, bool deleted)
        {
            var delAccounts = db.Accounts.Where(acc => acc.CurrencyId == currencyId).Include(acc => acc.Currency).ToList();
            var delMotions = new List<Motion>();
            List<Motion> delMotionsAcc = null;
            foreach (var acc in delAccounts)
            {
                delMotionsAcc = db.Motions.Where(motion => acc.Id == motion.AccountId).ToList();
                delMotions.AddRange(delMotionsAcc);
            }

            foreach (var motion in delMotions)
            {
                motion.Deleted = deleted;
                db.Entry(motion).State = state;
            }
            db.SaveChanges();

            foreach (var acc in delAccounts)
            {
                acc.Deleted = deleted;
                db.Entry(acc).State = state;
            }
            db.SaveChanges();

            var delcurrency = db.Currencies.Find(currencyId);
            delcurrency.IsArchive = deleted;
            db.Entry(delcurrency).State = state;

            db.SaveChanges();
        }
    }
}
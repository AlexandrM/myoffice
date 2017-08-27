using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using ASE.MVC;
using ASE.EF;

using Web.MyOffice.Data;
using Web.MyOffice.Models;


namespace Web.MyOffice.Controllers.References
{
    [Authorize]
    //[RequireHttps]
    [ViewEngineAdv("References/")]
    public class CurrencyRateController : ControllerAdv<DB>
    {
        public ActionResult Index(Guid? currencyId, int? page, string searchTB_Filter)
        {
            if (!currencyId.HasValue)
                return RedirectToAction("Index", "Currency");

            ViewBag.currencyId = currencyId;

            IQueryable<CurrencyRate> list = db.CurrencyRates.Include(x => x.Currency).Where(x => x.Currency.UserId == UserId);

            if (!String.IsNullOrEmpty(searchTB_Filter))
            {
                string[] filters = searchTB_Filter.Split(' ');
                foreach (var filter in filters)
                    list = list.ApplyFilterPatern(filter,
                        x => x.Currency.Name,
                        x => x.Currency.ShortName,
                        x => x.DateTime
                        );
            }

            list = list.Where(x => x.CurrencyId == currencyId.Value);

            list = list.OrderByDescending(x => x.DateTime);
            var model = SearchablePagedList<CurrencyRate>.Create(list, page, 25, searchTB_Filter);
            if (Request.IsAjaxRequest())
                return new JsonResult
                {
                    Data = new
                    {
                        html = this.RenderPartialView("IndexList", model)
                    }
                };
            else
                return View("Index", model);
        }

        public ActionResult Create(Guid currencyId)
        {
            return View(new CurrencyRate(db.Currencies.Find(currencyId)));
        }

        private void UpdateRate(Guid currencyId)
        {
            var cur = db.Currencies.FirstOrDefault(x => x.Id == currencyId);
            var rate = db.CurrencyRates.Where(x => x.CurrencyId == currencyId).OrderByDescending(x => x.DateTime).Take(1).FirstOrDefault();
            cur.Value = rate.Value;
            db.SaveChanges();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CurrencyRate model)
        {
            if (ModelState.IsValid)
            {
                model.CurrencyId = model.CurrencyId;
                db.CurrencyRates.Add(model);
                db.SaveChanges();
                UpdateRate(model.CurrencyId);

                return RedirectToAction("Index", new { currencyId = model.CurrencyId });
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = db.CurrencyRates.FirstOrDefault(x => x.Id == id & x.Currency.UserId == UserId);
            UpdateRate(model.CurrencyId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CurrencyRate model)
        {
            if (ModelState.IsValid)
            {
                var dbModel = db.CurrencyRates.FirstOrDefault(x => x.Id == model.Id & x.Currency.UserId == UserId);
                dbModel.DateTime = model.DateTime;
                dbModel.Value = model.Value;
                db.SaveChanges();

                return RedirectToAction("Index", new { currencyId = model.CurrencyId });
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = db.CurrencyRates.FirstOrDefault(x => x.Id == id & x.Currency.UserId == UserId);
            db.CurrencyRates.Remove(model);
            db.SaveChanges();

            return RedirectToAction("Index", new { currencyId = model.CurrencyId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

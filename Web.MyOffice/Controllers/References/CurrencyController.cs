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
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Xml;


namespace Web.MyOffice.Controllers.References
{
    [Authorize]
    //[RequireHttps]
    [ViewEngineAdv("References/")]
    public class CurrencyController : ControllerAdv<DB>
    {
        public ActionResult Index(int? page, string searchTB_Filter)
        {
            IQueryable<Currency> list = db.Currencies.Where(x => x.UserId == UserId);

            if (!String.IsNullOrEmpty(searchTB_Filter))
            {
                string[] filters = searchTB_Filter.Split(' ');
                foreach (var filter in filters)
                    list = list.ApplyFilterPatern(filter,
                        x => x.Name,
                        x => x.ShortName);
            }

            list = list.OrderBy(x => x.Name);
            var model = SearchablePagedList<Currency>.Create(list, page, 25, searchTB_Filter);
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

        private void FillViewBag()
        {

        }

        public ActionResult Create()
        {
            return View(new Currency());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Currency currency)
        {
            if (ModelState.IsValid)
            {
                currency.Id = Guid.NewGuid();
                currency.UserId = UserId;
                currency.Value = currency.Value == 0 ? 1 : currency.Value;
                db.Currencies.Add(currency);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(currency);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currencies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Currency model)
        {
            if (ModelState.IsValid)
            {
                Currency currency = db.Currencies.Find(model.Id);

                currency.UserId = UserId;
                currency.Value = model.Value == 0 ? 1 : model.Value;
                currency.Name = model.Name;
                currency.ShortName = model.ShortName;
                currency.CurrencyType = model.CurrencyType;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Delete(Guid id)
        {
            Currency currency = db.Currencies.Find(id);
            db.Currencies.Remove(currency);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ImportRates(int mode)
        {
            try
            {
                if (mode < 3) //Privatbank
                {
                    using (WebClient wc = new WebClient())
                    {
                        var ret = wc.DownloadString("https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5");
                        JArray ja = JArray.Parse(ret);
                        foreach (JObject item in ja)
                        {
                            string value = mode == 1 ? item.GetValue("buy").ToString() : item.GetValue("sale").ToString();
                            value = value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                            value = value.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                            CurrencyType currencyType = CurrencyType.UAH;
                            if (item.GetValue("ccy").ToString() == "RUR")
                                currencyType = CurrencyType.RUR;
                            else if (item.GetValue("ccy").ToString() == "EUR")
                                currencyType = CurrencyType.EUR;
                            else if (item.GetValue("ccy").ToString() == "USD")
                                currencyType = CurrencyType.USD;
                            else
                                continue;

                            var currency = db.Currencies.FirstOrDefault(x => x.UserId == UserId && x.CurrencyType == currencyType);
                            if (currency == null)
                                continue;
                            if (currency.Value == decimal.Parse(value))
                                continue;
                            db.CurrencyRates.Add(new CurrencyRate
                            {
                                DateTime = DateTime.Now,
                                CurrencyId = currency.Id,
                                Value = decimal.Parse(value)
                            });
                            currency.Value = decimal.Parse(value);
                            db.SaveChanges();
                        }
                    }
                }
                if ((mode == 3) | (mode == 4)) //NBU & CBR
                {
                    using (WebClient wc = new WebClient())
                    {
                        var ret = wc.DownloadString(String.Format("https://privat24.privatbank.ua/p24/accountorder?oper=prp&PUREXML&apicour&country={0}&full", mode == 3 ? "ua" : "ru"));
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(ret);
                        foreach (XmlNode item in doc["exchangerate"].ChildNodes)
                        {
                            string value = item.Attributes["buy"].Value;
                            value = value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                            value = value.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                            string unit = item.Attributes["unit"].Value;
                            unit = unit.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                            unit = unit.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                            CurrencyType currencyType = CurrencyType.UAH;
                            if (item.Attributes["ccy"].Value == "RUR")
                                currencyType = CurrencyType.RUR;
                            else if (item.Attributes["ccy"].Value == "EUR")
                                currencyType = CurrencyType.EUR;
                            else if (item.Attributes["ccy"].Value == "USD")
                                currencyType = CurrencyType.USD;
                            else
                                continue;

                            db.CurrencyRates.Add(new CurrencyRate
                            {
                                DateTime = DateTime.Now,
                                CurrencyId = db.Currencies.FirstOrDefault(x => x.UserId == UserId && x.CurrencyType == currencyType).Id,
                                Value = decimal.Parse(value) / 100 / 100 / decimal.Parse(unit)
                            });
                            db.SaveChanges();
                        }
                    }
                }
                
            }
            catch
            {

            }

            return RedirectToAction("Index");
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

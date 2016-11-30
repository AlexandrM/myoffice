using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ASE;
using ASE.MVC;
using Web.MyOffice.Data;
using System.Data.Entity.SqlServer;

namespace Web.MyOffice.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TimeLoggerController : ControllerAdv<DB>
    {
        //
        // GET: /TimeLogger/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View(db.TimeLoggerSettingWorkStations.Where(x => x.UserId == this.UserId).OrderBy(x => x.Name).ToList());
        }

        public ActionResult EditWorkStation(Guid id)
        {
            return View(db.TimeLoggerSettingWorkStations.Find(id) ?? new TimeLoggerSettingWorkStation { WorkStationId = Guid.NewGuid() });
        }

        [HttpPost]
        public ActionResult EditWorkStation(TimeLoggerSettingWorkStation model)
        {
            if (model.Id.IsEmpty())
            {
                model.Id = Guid.NewGuid();
                //model.Settings = db.TimeLoggerSettingGet(this.UserId);
                model.UserId = UserId;
                db.TimeLoggerSettingWorkStations.Add(model);
            }
            else
                AttachModel(model);
            db.SaveChanges();

            return RedirectToAction("Settings");
        }

        public ActionResult DeleteWorkStation(Guid id, bool? delete)
        {
            var item = db.TimeLoggerSettingWorkStations.Find(id);
            if (delete.GetValueOrDefault(false))
            {
                db.TimeLoggerSettingWorkStations.Remove(item);
                db.SaveChanges();

                return RedirectToAction("Settings");
            }

            return View(item);
        }

        public ActionResult Journal()
        {
            return View(db.TimeLoggerSettingWorkStations.Where(x => x.UserId == this.UserId).OrderBy(x => x.Name).ToList());
        }

        public class LogItemData
        {
            public int? Time { get; set; }

            public TimeLoggerSettingWorkStation WorkStation { get; set; }

            public TimeLoggerApplicationArgument ApplicationArgument { get; set; }
        }

        [HttpPost]
        public JsonResult JournalData(DateTime dateFrom, DateTime dateTo, List<Guid> dataBases)
        {
            dateTo = dateTo.Date.AddDays(1).AddMilliseconds(-1);

            List<LogItemData> list = new List<LogItemData>();
            var q = from items in db.TimeLoggerLogItems.Where(x => x.DTActivate >= dateFrom & x.DTDeActivate <= dateTo & dataBases.Contains(x.ApplicationArgument.WorkStationId))
                    group items by items.ApplicationArgument into g1
                    select new { ApplicationArgument = g1.Key, Time = g1.Sum(x => SqlFunctions.DateDiff("second", x.DTActivate, x.DTDeActivate)) } into result

                    join stations in db.TimeLoggerSettingWorkStations.Where(x => x.UserId == UserId & dataBases.Contains(x.WorkStationId))
                    on result.ApplicationArgument.WorkStationId equals stations.WorkStationId into w
                    from station in w.DefaultIfEmpty()

                    select new LogItemData { WorkStation = station, ApplicationArgument = result.ApplicationArgument, Time = result.Time };


            list.AddRange(q.ToList());

            return new JsonResult
            {
                Data = new
                {
                    dateFrom = dateFrom.ToStringD(),
                    dateTo = dateTo.ToStringD(),
                    table = this.RenderPartialView("JournalData", list)
                }
            };
        }

        public ActionResult Applications()
        {
            var q = from items in db.TimeLoggerSettingWorkStations
                    where items.UserId == UserId
                    join aa in db.TimeLoggerApplicationArguments on items.Id equals aa.WorkStationId
                    select aa;

            return View(q.ToList());
        }

        [HttpPost]
        public JsonResult AddCategory(string categoryName, Guid aaItem)
        {
            var itemAA = db.TimeLoggerApplicationArguments.Find(aaItem);
            if (!String.IsNullOrWhiteSpace(categoryName))
                if (itemAA != null)
                {
                    var item = db.TimeLoggerApplicationCategories.Create();
                    item.Id = Guid.NewGuid();
                    item.Name = categoryName;
                    item.UserId = UserId;

                    db.TimeLoggerApplicationCategories.Add(item);
                    db.SaveChanges();

                    itemAA.Category = item;
                    db.SaveChanges();
                }

            return new JsonResult
            {
                Data = new
                {
                    data = this.RenderPartialView("ApplicationsData")
                }
            };
        }

        [HttpPost]
        public JsonResult SelectCategory(Guid category, Guid aaItem)
        {
            var item = db.TimeLoggerApplicationArguments.Find(aaItem);
            item.Category = db.TimeLoggerApplicationCategories.Find(category);
            db.SaveChanges();

            return new JsonResult
            {
                Data = new
                {
                    data = this.RenderPartialView("ApplicationsData")
                }
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult SyncDbStartStop(Guid dbid, TimeLoggerStartStop model)
        {
            bool result = true;
            try
            {
                db.TimeLoggerStartStop.Add(model);
                db.SaveChanges();
            }
            catch
            {
                result = false;
            }

            return new JsonResult
            {
                Data = new
                {
                    result = result
                }
            };
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public JsonResult SyncDbApplicationArgument(Guid dbid, TimeLoggerApplicationArgument model)
        {
            bool result = true;
            try
            {
                db.TimeLoggerApplicationArguments.Add(model);
                db.SaveChanges();
            }
            catch
            {
                result = false;
            }

            return new JsonResult
            {
                Data = new
                {
                    result = result
                }
            };
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public JsonResult DbLogItem(Guid dbid, TimeLoggerLogItem model)
        {
            bool result = true;
            try
            {
                db.TimeLoggerLogItems.Add(model);
                db.SaveChanges();
            }
            catch
            {
                result = false;
            }

            return new JsonResult
            {
                Data = new
                {
                    result = result
                }
            };
        }
       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ASE;
using ASE.MVC;
using Web.MyOffice.Data;
using Web.MyOffice.Models;

namespace Web.MyOffice.Controllers
{
    /*[Authorize]
    //[RequireHttps]
    public class QuickRecordsController : ControllerAdv<DB>
    {
        //
        // GET: /QuickRecord/
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddCaregory(string category, Guid? parent)
        {
            var item = db.QuickRecordCategories.Create();
            item.Id = Guid.NewGuid();
            item.UserId = UserId;
            item.ParentId = parent;
            item.Name = category;
            db.QuickRecordCategories.Add(item);
            db.SaveChanges();

            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("SettingsTree")
                }
            };
        }

        [HttpPost]
        public JsonResult RemoveCategory(Guid category)
        {
            var item = db.QuickRecordCategories.Find(category, UserId);
            db.QuickRecordCategories.Remove(item);
            db.SaveChanges();

            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("SettingsTree")
                }
            };
        }

        public ActionResult QuickRecords(Guid category)
        {
            return View(category);
        }

        [HttpPost]
        public JsonResult AddRecord(string text, Guid category)
        {
            if (!String.IsNullOrEmpty(text))
            {
                var item = db.QuickRecords.Create();
                item.Id = Guid.NewGuid();
                //item.UserId = UserId;
                item.Record = text;
                item.DateTime = DateTime.Now;
                item.Priority = 0;
                item.Category = db.QuickRecordCategories.Find(category, UserId);
                db.QuickRecords.Add(item);
                db.SaveChanges();
            }

            return new JsonResult
            {
                Data = new
                {
                    text = ""
                }
            };
        }

        public JsonResult Filter(Guid category, string filter)
        {
            var list = new List<QuickRecord>();

            if (!String.IsNullOrEmpty(filter))
            {
                string[] filters = filter.Split(',');
                var q = from items in db.QuickRecords
                   where (items.CategoryId == category && items.CategoryUserId == UserId)
                   && (filters.Any(f => items.Record.Contains(f)))
                   orderby items.Priority descending, items.Record
                   select items;

                list.AddRange(q.ToList());
            }
            

            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("QuickRecordsList", list)
                }
            };
        }

        [HttpPost]
        public JsonResult RemoveRecord(Guid id)
        {
            var item = db.QuickRecords.Find(id);
            if (item.CategoryUserId == UserId)
            {
                db.QuickRecords.Remove(item);
                item.Record += "1";
                db.SaveChanges();
            }

            return new JsonResult
            {
                Data = new
                {
                }
            };
        }
    }*/
}
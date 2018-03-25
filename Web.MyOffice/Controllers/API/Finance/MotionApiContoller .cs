using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;
using System.Data.Entity;
using System.Dynamic;

using ASE;
using ASE.EF;
using ASE.MVC;
using ASE.Json;

using Web.MyOffice.Data;
using MyBank.Models;
using Web.MyOffice.Models;
using System.Data.SqlClient;
using Method = System.Web.Http;

namespace Web.MyOffice.Controllers.API
{
    public class MotionsController : ControllerApiAdv<DB>
    {
        [Method.HttpGet]
        public HttpResponseMessage MotionsUpdate(Guid itemId)
        {
            using (db)
            {
                var motions = db.Motions.Where(motion => motion.ItemId == itemId).Include(motion=>motion.Account).ToList();
                return ResponseObject2Json(motions);
            }
        }
        [Method.HttpPut]
        public HttpResponseMessage MotionsUpdate(List<Motion> motions)
        {
            using (db)
            {
                Motion updateMotion = null;
                foreach (var motion in motions)
                {
                    updateMotion = db.Motions.Find(motion.Id);
                    updateMotion.Deleted = motion.Deleted;
                    db.Entry(updateMotion).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        public class MotionDeleteModel
        {
            public Guid Id { get; set; }

            public bool Delete { get; set; }
        }

        [HttpPost]
        [Route("api/Motions/Delete")]
        public HttpResponseMessage MotionDelete(MotionDeleteModel model)
        {
            using (db)
            {
                var deletedMotion = db.Motions.Find(model.Id);
                if (model.Delete)
                {
                    db.Motions.Remove(deletedMotion);
                }
                else
                {
                    deletedMotion.Deleted = !deletedMotion.Deleted;
                }
                db.SaveChanges();
            }
            return ResponseObject2Json(HttpStatusCode.Accepted);
        }

        public class MotionMerge
        {
            public Item mainItem { set; get; }
            public Item selectedItem { set; get; }
        }

        [HttpPost]
        [Route("api/Motions/Merge")]
        public HttpResponseMessage ItemMerge(MotionMerge motionMerge)
        {
            var mainItem = motionMerge.mainItem;
            var selectedItem = motionMerge.selectedItem;

            mainItem.Motions = db.Motions.Where(motion => motion.ItemId == mainItem.Id).ToList();
            selectedItem.Motions = db.Motions.Where(motion => motion.ItemId == selectedItem.Id).ToList();

            using (db)
            {
                    foreach (var motion in selectedItem.Motions)
                    {
                        motion.ItemId = mainItem.Id;
                        db.Entry(motion).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    db.Entry(db.Items.Find(selectedItem.Id)).State = EntityState.Deleted;
                    db.SaveChanges();
            }
                return ResponseObject2Json(HttpStatusCode.Moved);
        }
    }
}
using ASE.MVC;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web.MyOffice.Data;

namespace Web.MyOffice.Controllers.API
{
    public class RecordController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public List<Record> List(Guid sid)
        {
            return db.BankRecords
                .Where(x => x.Owner.APISessionId == sid)
                .OrderByDescending(x => x.DateTime).Take(40).ToList();
        }

        [HttpPost]
        public HttpResponseMessage List(Record record)
        {
            var m = db.Members.Where(x => x.APISessionId == record.OwnerId).Select(x => x.Id).FirstOrDefault();
            if ((m != null) & (record.Sum != 0) & (!String.IsNullOrEmpty(record.Name)))
            {
                record.OwnerId = m;
                record.Id = Guid.NewGuid();
                db.BankRecords.Add(record);
                db.SaveChanges();
            }

            return ResponseObject2Json(record);
        }

        [HttpDelete]
        public HttpResponseMessage Delete(Guid id)
        {
            var m = db.BankRecords.FirstOrDefault(x => x.Id == id);
            if (m != null)
            {
                db.BankRecords.Remove(m);
                db.SaveChanges();
            }

            return ResponseObject2Json(id);
        }
    }
}

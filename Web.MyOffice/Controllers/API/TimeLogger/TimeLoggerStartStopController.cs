using ASE.MVC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

using Web.MyOffice.Data;

namespace Web.MyOffice.Controllers.API
{
    /*public class TimeLoggerStartStopController : ControllerApiAdv<DB>
    {
        public static List<Guid> KnownClients = new List<Guid>();

        public static bool Validate(DB db, Guid id)
        {
            lock (KnownClients)
            {
                if (KnownClients.FirstOrDefault(x => x == id) != Guid.Empty)
                    return true;

                var item = db.TimeLoggerSettingWorkStations.FirstOrDefault(x => x.WorkStationId == id);
                if (item == null)
                    return false;

                KnownClients.Add(id);
                return true;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [HttpPut]
        public HttpResponseMessage Post([FromUri]Guid dbid, [FromBody]TimeLoggerStartStop model)
        {
            if (TimeLoggerStartStopController.Validate(db, dbid))
            {
                if (db.TimeLoggerStartStop.Find(model.Id) == null)
                {
                    db.TimeLoggerStartStop.Add(model);
                    db.SaveChanges();
                }

                return new HttpResponseMessage() { Content = new StringContent("true", Encoding.UTF8, "application/json") };
            }

            return new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.InternalServerError, Content = new StringContent("false", Encoding.UTF8, "application/json") };
        }
    }*/
}
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
    public class TimeLoggerLogItemController : ControllerApiAdv<DB>
    {
        [AllowAnonymous]
        [HttpPost]
        [HttpPut]
        public HttpResponseMessage Post([FromUri]Guid dbid, [FromBody]TimeLoggerLogItem model)
        {
            if (TimeLoggerStartStopController.Validate(db, dbid))
            {
                db.TimeLoggerLogItems.Add(model);
                db.SaveChanges();

                return new HttpResponseMessage() { Content = new StringContent("true", Encoding.UTF8, "application/json") };
            }

            return new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.InternalServerError, Content = new StringContent("false", Encoding.UTF8, "application/json") };
        }
    }
}
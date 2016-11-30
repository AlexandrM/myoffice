using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using ASE.EF;
using ASE.ToolsModels;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using ASE.Json;
using System.Linq.Expressions;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace ASE.MVC
{
    public class ControllerApiAdv<DBType> : ApiController where DBType : DbContext, new()
    {
        internal DBType db;

        public ControllerApiAdv()
        {
            db = new DBType();
        }

        public Guid UserId
        {
            get
            {
                return Guid.Parse(this.User.Identity.GetUserId());
            }
        }

        public Guid? UserIdAPI
        {
            get
            {
                if (!Request.Headers.Contains("UID"))
                {
                    return null;
                }

                Guid uid = Guid.Empty;
                if (Guid.TryParse(Request.Headers.FirstOrDefault(x => x.Key == "UID").Value.FirstOrDefault(), out uid))
                {
                    return uid;
                }


                return null;
            }
        }

        public string UserIdS
        {
            get
            {
                return UserId.ToString();
            }
        }

        internal HttpResponseMessage ResponseEmpty()
        {
            return new HttpResponseMessage() { Content = new StringContent("", Encoding.UTF8, "application/json") };
        }

        internal HttpResponseMessage ResponseObject2Json(object obj, Func<JsonProperty, MemberInfo, MemberSerialization, JsonProperty> processor)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, 
                new JsonSerializerSettings {
                    ContractResolver = new ContractResolverWithProcessor(processor),
                    //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    //PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });

            return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        }

        internal HttpResponseMessage ResponseObject2Json(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    //PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });

            return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
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
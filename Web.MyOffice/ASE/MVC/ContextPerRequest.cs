using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using System.Data.Entity;

namespace ASE.MVC
{
    public static class ContextPerRequest<DBType> where DBType : DbContext, new()
    {
        private const string myDbPerRequestContext = "MDPRC";

        public static DBType Db
        {
            get
            {
                if (!HttpContext.Current.Items.Contains(myDbPerRequestContext))
                {
                    HttpContext.Current.Items.Add(myDbPerRequestContext, new DBType());
                }

                return HttpContext.Current.Items[myDbPerRequestContext] as DBType;
            }
        }

        public static Guid UserId
        { 
            get
            {
                return HttpContext.Current.UserId();
            }
        }

        public static void DisposeDbContextPerRequest()
        {
            var entityContext = HttpContext.Current.Items[myDbPerRequestContext] as DBType;
            if (entityContext != null)
            {
                entityContext.Dispose();
                HttpContext.Current.Items.Remove(myDbPerRequestContext);
            }
        }
    }
}

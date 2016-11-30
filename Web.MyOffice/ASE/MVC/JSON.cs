using ASE.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASE.MVC
{
    public class JSON
    {
        public static JsonNetResult EmptyJsonNetResult()
        {
            return new JsonNetResult { Data = new { } };
        }
    }

    public static class JsonNetResultExtension
    {
    }
    
}
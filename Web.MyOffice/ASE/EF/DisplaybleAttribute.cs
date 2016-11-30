using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASE.EF
{
    public class DisplaybleAttribute: Attribute
    {
        public string Displayble { get; set; }

        public string Format { get; set; }
    }
}
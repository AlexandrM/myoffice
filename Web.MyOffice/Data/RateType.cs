using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;

namespace Web.MyOffice.Models
{
    public enum RateType
    {
        [LocalizedDisplayAttribute("Unknown")]
        Unknown = 0,
        [LocalizedDisplayAttribute("Hour")]
        Hour = 1,
        [LocalizedDisplayAttribute("Day")]
        Day = 2,
        [LocalizedDisplayAttribute("Month")]
        Month = 3
    }
}
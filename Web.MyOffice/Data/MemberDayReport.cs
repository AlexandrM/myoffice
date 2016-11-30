using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ASE;
using ASE.MVC;

namespace Web.MyOffice.Models
{
    public class MemberDayReport : MemberMotion
    {
        public MemberDayReport()
        {
            DateTime = DateTime.Now;
        }

        [DataType(DataType.DateTime)]
        [LocalizedRequired]
        [LocalizedDisplay("DateTime")]
        new public DateTime DateTime { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Rate")]
        public decimal Value { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Type")]
        public RateType RateType { get; set; }

        [NotMapped]
        public string RateTypeS
        {
            get
            {
                return RateType.ToDisplayName();
            }
        }
    }
}
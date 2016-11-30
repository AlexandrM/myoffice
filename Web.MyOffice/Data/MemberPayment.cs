using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;
using System.ComponentModel.DataAnnotations;

namespace Web.MyOffice.Models
{
    public class MemberPayment : MemberMotion
    {
        public MemberPayment()
        {
            DateTime = DateTime.Now;
        }

        [DataType(DataType.DateTime)]
        [LocalizedRequired]
        [LocalizedDisplay("DateTime")]
        new public DateTime DateTime { get; set; }

    }
}
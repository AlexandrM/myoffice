using System;
using System.Linq;
using System.Collections.Generic;
namespace Web.MyOffice.Models
{
    public class MemberDebt
    {
        public Member Member { get; set; }

        public Currency Currency { get; set; }

        public decimal Debt { get; set; }
    }
}
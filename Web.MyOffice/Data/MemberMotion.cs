using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;

namespace Web.MyOffice.Models
{
    public class MemberMotion : AutoGuidIdOwner
    {
        public DateTime DateTime { get; set; }

        public Guid? MemberId { get; set; }

        [LocalizedDisplayAttribute("Member")]
        public Member Member { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Amount")]
        public decimal Amount { get; set; }

        [LocalizedDisplayAttribute("Description")]
        public string Description { get; set; }

        public Guid? ProjectId { get; set; }

        [LocalizedDisplayAttribute("Project")]
        public Project Project { get; set; }
    }
}
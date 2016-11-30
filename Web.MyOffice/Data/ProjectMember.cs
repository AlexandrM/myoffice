using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ASE;
using ASE.MVC;

namespace Web.MyOffice.Models
{
    public class ProjectMember// : AutoGuidId
    {
        [Key, Column(Order = 0)]
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        [Key, Column(Order = 1)]
        public Guid MemberId { get; set; }

        [LocalizedDisplayAttribute("Member")]
        public Member Member { get; set; }

        [LocalizedDisplayAttribute("MemberType")]
        public ProjectMemberType MemberType { get; set; }

        [NotMapped]
        public string MemberTypeS {
            get
            {
                return MemberType.ToDisplayName();
            }
        }
    }
}
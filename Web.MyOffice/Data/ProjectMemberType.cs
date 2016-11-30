using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;

namespace Web.MyOffice.Models
{
    public enum ProjectMemberType
    {
        [LocalizedDisplayAttribute("Implementer")]
        Implementer = 0,

        [LocalizedDisplayAttribute("Customer")]
        Customer = 1,

        [LocalizedDisplayAttribute("Member")]
        Member = 2
    }
}
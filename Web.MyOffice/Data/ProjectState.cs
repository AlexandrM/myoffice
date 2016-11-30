using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;

namespace Web.MyOffice.Models
{
    public enum ProjectState
    {
        [LocalizedDisplayAttribute("ProjectStateNew")]
        New = 0,

        [LocalizedDisplayAttribute("ProjectStateInProgress")]
        InProgress = 1,

        [LocalizedDisplayAttribute("ProjectStateComplete")]
        Complete = 2,

        [LocalizedDisplayAttribute("ProjectStateApproved")]
        Approved = 3
    }
}
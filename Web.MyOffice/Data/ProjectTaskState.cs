using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;

namespace Web.MyOffice.Models
{
    public enum ProjectTaskState
    {
        [LocalizedDisplayAttribute("ProjectTaskStateNew")]
        New = 0,

        [LocalizedDisplayAttribute("ProjectTaskStateInProgress")]
        InProgress = 1,

        [LocalizedDisplayAttribute("ProjectTaskStateComplete")]
        Complete = 2,

        [LocalizedDisplayAttribute("ProjectTaskStateApproved")]
        Approved = 3
    }
}
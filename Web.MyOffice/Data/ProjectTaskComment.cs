using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;
using System.ComponentModel.DataAnnotations;

namespace Web.MyOffice.Models
{
    public partial class ProjectTaskComment : AutoGuidId
    {
        public ProjectTaskComment()
        {
            DateTime = DateTime.Now;
            Updated = DateTime.Now;
        }

        [LocalizedRequired]
        public Guid TaskId { get; set; }

        public ProjectTask Task { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Comment")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        public string History { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Date")]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }

        public DateTime? Updated { get; set; }

        [LocalizedRequired]
        public Guid AuthorId { get; set; }

        [LocalizedDisplayAttribute("Author")]
        public Member Author { get; set; }
    }
}
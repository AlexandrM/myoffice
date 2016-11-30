using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;
using System.ComponentModel.DataAnnotations;

namespace Web.MyOffice.Models
{
    public class ProjectTask : AutoGuidId
    {
        public ProjectTask()
        {
            DateTime = DateTime.Now.Date;
            Updated = DateTime.Now;
            State = ProjectTaskState.New;
            Limitation = DateTime.Now.Date.AddDays(3);
        }

        [LocalizedRequired]
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Name")]
        public string Name { get; set; }

        [LocalizedDisplayAttribute("Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string History { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Date")]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }

        public DateTime? Updated { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Limitation")]
        [DataType(DataType.Date)]
        public DateTime Limitation { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("State")]
        public ProjectTaskState State { get; set; }

        public List<ProjectTaskComment> Comments { get; set; }

        [LocalizedRequired]
        public Guid AuthorId { get; set; }

        [LocalizedDisplayAttribute("Author")]
        public Member Author { get; set; }
    }
}
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ASE;
using ASE.MVC;

namespace Web.MyOffice.Models
{
    public class Project : AutoGuidId
    {
        public Project()
        {
            DateTime = DateTime.Now.Date;
            State = ProjectState.New;
        }

        [LocalizedRequired]
        public Guid AuthorId { get; set; }

        [LocalizedDisplayAttribute("Author")]
        public Member Author { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Name")]
        public string Name { get; set; }

        [LocalizedDisplayAttribute("Description")]
        public string Description { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Date")]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Date")]
        public ProjectState State { get; set; }

        [NotMapped]
        public string StateS {
            get
            {
                return State.ToDisplayName();
            }
        }

        [LocalizedDisplayAttribute("Currency")]
        public CurrencyType RateCurrencyType { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Rate")]
        public decimal RateValue { get; set; }

        [LocalizedRequired]
        //[JsonConverter(typeof(StringEnumConverter))]
        public RateType RateType { get; set; }

        public List<ProjectMember> Members { get; set; }

        public List<ProjectTask> Tasks { get; set; }

        public List<MemberDayReport> DayReports { get; set; }

        public List<MemberPayment> Payments { get; set; }

        public string BotId { get; set; }

        public int BotUTC { get; set; }
    }
}
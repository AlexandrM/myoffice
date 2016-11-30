using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.MyOffice.Models;

namespace MyBank.Models
{
    public class BudgetUser : AutoGuidId
    {
        public Guid BudgetId { get; set; }
        [Display(ResourceType = typeof(R.R), Name = "Account")]
        public Budget Budget { get; set; }

        public Guid UserId { get; set; }
        [Display(ResourceType = typeof(R.R), Name = "User")]
        public Member User { get; set; }
    }
}
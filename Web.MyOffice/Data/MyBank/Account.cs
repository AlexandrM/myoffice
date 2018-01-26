using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.MyOffice.Models;

namespace MyBank.Models
{
    public class Account : AutoGuidId
    {
        //public Guid BudgetId { get; set; }
        //[Display(ResourceType = typeof(R.R), Name = "Budget")]
        //public Budget Budget { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Deleted")]
        public bool Deleted { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Account")]
        public string Name { get; set; }

        public Guid CategoryId { get; set; }
        [Display(ResourceType = typeof(R.R), Name = "Category")]
        public CategoryAccount Category { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Description")]
        public string Description { get; set; }

        public Guid CurrencyId { get; set; }
        [Display(ResourceType = typeof(R.R), Name = "Currency")]
        public Currency Currency { get; set; }

        public List<Motion> Motions { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "CreditLimit")]
        public decimal CreditLimit { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "ShowInRest")]
        public bool ShowInRest { get; set; }
    }
}
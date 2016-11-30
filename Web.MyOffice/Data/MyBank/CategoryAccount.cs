using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.MyOffice.Models;

namespace MyBank.Models
{
    public class CategoryAccount : AutoGuidId, IComparable
    {
        public Guid BudgetId { get; set; }
        public Budget Budget { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Description")]
        public string Description { get; set; }

        public int CompareTo(object obj)
        {
            return String.Compare(this.Name, (obj as Category).Name);
        }

        public List<Account> Accounts { get; set; }
    }
}
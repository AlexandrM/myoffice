using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.MyOffice.Models;

namespace MyBank.Models
{
    public class Item : AutoGuidId
    {
        public Guid BudgetId { get; set; }
        public Budget Budget { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Deleted")]
        public bool Deleted { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Description")]
        public string Description { get; set; }

        public Guid CategoryId { get; set; }
        [Display(ResourceType = typeof(R.R), Name = "Category")]
        public CategoryItem Category { get; set; }

        public List<Motion> Motions { get; set; }
    }
}
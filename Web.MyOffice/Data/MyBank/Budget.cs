using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.MyOffice.Models;

namespace MyBank.Models
{
    public class Budget : AutoGuidId
    {
        public Guid OwnerId { get; set; }
        [Display(ResourceType = typeof(R.R), Name = "Account")]
        public Member Owner { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Name")]
        public string Name { get; set; }

        public List<BudgetUser> Users { get; set; }

        public List<CategoryAccount> CategoryAccounts { get; set; }

        public List<CategoryItem> CategoryItems { get; set; }

        public List<Item> Items { get; set; }
    }
}
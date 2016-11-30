using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.MyOffice.Models;

namespace MyBank.Models
{
    public class Motion : AutoGuidId
    {
        [Display(ResourceType = typeof(R.R), Name = "Deleted")]
        public bool Deleted { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "DateTime")]
        public DateTime DateTime { get; set; }

        public Guid AccountId { get; set; }
        [Display(ResourceType = typeof(R.R), Name = "Account")]
        public Account Account { get; set; }

        public Guid ItemId { get; set; }
        [Display(ResourceType = typeof(R.R), Name = "Item")]
        public Item Item { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Sum")]
        public decimal SumP { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Sum")]
        public decimal SumM { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "Note")]
        public string Note { get; set; }

        [Display(ResourceType = typeof(R.R), Name = "DateTimeAdded")]
        public DateTime Added { get; set; }
    }
}
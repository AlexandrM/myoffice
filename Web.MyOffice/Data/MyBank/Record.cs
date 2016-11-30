using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Web.MyOffice.Models;

namespace MyBank.Models
{
    public class Record
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public Member Owner { get; set; }

        public string Name { get; set; }

        public decimal Sum { get; set; }

        public DateTime DateTime { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
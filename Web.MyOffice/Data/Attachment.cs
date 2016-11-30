using System;
using System.Linq;
using System.Collections.Generic;
namespace Web.MyOffice.Models
{
    public class Attachment : AutoGuidId
    {
        public string Name { get; set; }

        public string OriginalName { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
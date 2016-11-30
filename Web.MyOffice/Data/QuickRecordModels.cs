using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.MyOffice.Models
{
    public class QuickRecordCategory
    {
        [Key, Column(Order=0)]
        public Guid Id { get; set; }

        [Key, Column(Order = 1)]
        public Guid UserId { get; set; }
        
        public Guid? ParentId { get; set; }
        public Guid? ParentUserId { get; set; }

        public virtual QuickRecordCategory Parent { get; set; }

        public string Name { get; set; }
    }

    public class QuickRecord
    {
        //[Key, Column(Order = 0)]
        [Key]
        public Guid Id { get; set; }
        
        //[Key, Column(Order = 1)]
        //public Guid UserId { get; set; }

        public DateTime DateTime { get; set; }

        public Guid? CategoryId { get; set; }
        public Guid? CategoryUserId { get; set; }

        public virtual QuickRecordCategory Category { get; set; }

        public string Record { get; set; }

        public string Tags { get; set; }

        public int Priority { get; set; }
    }
}
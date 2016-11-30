using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.MyOffice.Models
{
    /// <summary>
    /// Basic model
    /// With Guid Id
    /// </summary>
    public class EFModel
    {

        [Key]
        public Guid Id { get; set; }

    }
}
using System;
using System.Linq;
using System.Collections.Generic;
namespace Web.MyOffice.Models
{
    public class AutoGuidIdOwner : AutoGuidId
    {
        /// <summary>
        /// Budget UserId
        /// </summary>
        public Guid UserId { get; set; }
    }
}
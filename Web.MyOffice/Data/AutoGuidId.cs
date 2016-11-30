using System;
using System.Linq;
using System.Collections.Generic;
namespace Web.MyOffice.Models
{
    /// <summary>
    /// Auto fill id
    /// </summary>
    public class AutoGuidId : EFModel
    {
        public AutoGuidId()
        {
            Id = Guid.NewGuid();
        }
    }
}
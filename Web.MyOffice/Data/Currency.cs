using System;
using System.Linq;
using System.Collections.Generic;
using ASE.EF;
using ASE.MVC;
using System.Web.Script.Serialization;
using Web.MyOffice.Res;
using System.ComponentModel.DataAnnotations;

namespace Web.MyOffice.Models
{
    [Displayble(Displayble = "ShortName")]
    [SelectAttribute(Action = "Index", Controller = "Currency", ResourceType = typeof(S), ResourceName = "Currencies")]
    public partial class Currency : AutoGuidId
    {
        public Guid UserId { get; set; }

        public Member User { get; set; }

        [LocalizedDisplayAttribute("Name")]
        public string Name { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("ShortName")]
        public string ShortName { get; set; }

        [ScriptIgnore(ApplyToOverrides = true)]
        public List<CurrencyRate> CurrencyRates { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("CurrencyType")]
        public CurrencyType CurrencyType { get; set; }

        [DataType(DataType.Currency)]
        [LocalizedDisplayAttribute("CurrencyRate")]
        public decimal Value { get; set; }
    }
}
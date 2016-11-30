using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Web.MyOffice.Models
{
    public class MemberRate : AutoGuidIdOwner
    {
        public MemberRate()
        {
            DateTime = DateTime.Now;
            RateType = RateType.Hour;
        }

        public MemberRate(Member member, Currency currency)
        : this()
        {
            Member = member;
            MemberId = member.Id;
            Currency = currency;
            CurrencyId = currency.Id;
        }

        [LocalizedRequired]
        public Guid MemberId { get; set; }

        public Member Member { get; set; }

        [LocalizedRequired]
        public Guid CurrencyId { get; set; }

        [LocalizedDisplayAttribute("Currency")]
        public Currency Currency { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Date")]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Rate")]
        public decimal Value { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayAttribute("Type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RateType RateType { get; set; }
    }
}
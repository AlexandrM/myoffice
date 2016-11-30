using System;
using System.Linq;
using System.Collections.Generic;
using ASE.MVC;
using System.ComponentModel.DataAnnotations;

namespace Web.MyOffice.Models
{
    public class CurrencyRate
    {
        public CurrencyRate()
        {
            DateTime = DateTime.Now;
        }

        public CurrencyRate(Currency currency)
        : this()
        {
            Currency = currency;
            CurrencyId = currency.Id;
        }

        [Key]
        public int Id { get; set; }

        public Guid CurrencyId { get; set; }

        [LocalizedDisplayAttribute("Currency")]
        public Currency Currency { get; set; }

        [LocalizedDisplayAttribute("Date")]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }

        [DataType(DataType.Currency)]
        [LocalizedDisplayAttribute("CurrencyRate")]
        public decimal Value { get; set; }
    }
}
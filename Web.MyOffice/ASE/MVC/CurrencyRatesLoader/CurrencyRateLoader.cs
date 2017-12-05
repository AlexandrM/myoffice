using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.MyOffice.Models;
using Web.MyOffice.Data;
using System.Data.Entity;

namespace ASE
{
    public class CurrencyRateLoader: ICurrencyRateLoader
    {
        #region ICurrencyRateLoader implementing
            public List<ICurrencyRateSource> Sources { set; get; }
            public void AddSource(ICurrencyRateSource source)
            {
                Sources.Add(source);
            }
            public void RemoveSource(ICurrencyRateSource source)
            {
                Sources.Remove(source);
            }
            public ICurrencyRateSource FindSource(Guid Id)
            {
                return Sources.Find(source=> source.Id == Id);
            }
            public ICurrencyRateSource FindSource(string name)
            {
                return Sources.Find(source => source.Name == name);
            }
            public bool UpdateRates(string name)
            {
                var source = Sources.Find(src => src.Name == name);
                if (source == null) return false;
                if (source.Load())
                {
                    Db.CurrencyRates.RemoveRange(Db.CurrencyRates);
                    var currencies = Db.Currencies.ToList();
                    CurrencyRate currencyRate = null;
                    var rates = new List<CurrencyRate>();
                    decimal value = 0;

                    foreach (var currency in currencies)
                    {
                        value = source.LoadedRates.FirstOrDefault(rate => rate.Key == currency.CurrencyType).Value;
                        currency.Value = value==0?1:value;
                        currencyRate = new CurrencyRate();
                        currencyRate.CurrencyId = currency.Id;
                        currencyRate.Currency = currency;
                        currencyRate.Value = currency.Value;
                        currencyRate.DateTime = DateTime.Now;
                        rates.Add(currencyRate);
                    }
                Db.CurrencyRates.AddRange(rates);
                Db.SaveChanges();
            }
                return true;
            }
        #endregion
        private DB Db { set; get; }
        public CurrencyRateLoader(DB db)
        {
            Db = db;
            Sources = new List<ICurrencyRateSource>();
            Initialize();
        }
        private void Initialize()
        {
            var privatBankSource1 = new PrivatBankRatesSource(
                "RatesPrivatbank1", 
                new Uri("https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5", UriKind.Absolute), 
                null);

            /*var privatBankSource2 = new PrivatBankRatesSource(
                "RatesPrivatbank2",
                new Uri("https://privat24.privatbank.ua/p24/accountorder?oper=prp&PUREXML&apicour&country={0}&full", UriKind.Absolute),
                null);*/

            var NBUSource = new NBURatesSource(
                "RatesNBU",
                new Uri("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json", UriKind.Absolute),
                null);
            var CBRSource = new CBRRatesSource(
                "RatesCBR",
                new Uri("https://www.cbr-xml-daily.ru/daily_utf8.xml", UriKind.Absolute),
                null);

            AddSource(privatBankSource1);
            AddSource(NBUSource);
            AddSource(CBRSource);
        }
    }
}
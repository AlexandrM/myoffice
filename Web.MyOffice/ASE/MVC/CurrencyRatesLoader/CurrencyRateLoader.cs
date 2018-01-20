using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.MyOffice.Models;
using Web.MyOffice.Data;
using System.Data.Entity;
using System.Drawing.Drawing2D;

using WebGrease.Css.Extensions;

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
            public CurrencyType BaseCurrency { set; get;}

            private List<Currency> UserCurrenciesInit(Guid UserId, CurrencyType BaseCurrency)
            {
                var userCurrencies = Db.Currencies.Where(currency => currency.UserId == UserId).ToList();
                if (userCurrencies.Count == 0)
                {
                    Db.Currencies.Add(new Currency() { CurrencyType = BaseCurrency, UserId = UserId, Value = 1, MyCurrency = true, IsArchive = false });
                }
                else if (!Db.Currencies.Any(currency => currency.UserId == UserId && currency.MyCurrency))
                {
                    var baseCurrency = Db.Currencies.FirstOrDefault(currency => currency.CurrencyType == BaseCurrency);
                    if (baseCurrency != null)
                    {
                        baseCurrency.MyCurrency = true;
                    }
                    else
                    {
                        var currency = userCurrencies.First();
                        currency.MyCurrency = true;
                        Db.Entry(currency).State = EntityState.Modified;
                    }
                }
                Db.SaveChanges();
                return userCurrencies;
            }

            public bool UpdateRates(string name, Guid UserId)
            {
                var source = Sources.Find(src => src.Name == name);
                if (source == null) return false;
                BaseCurrency = source.BaseCurrency;
                var userCurrencies = UserCurrenciesInit(UserId, BaseCurrency);
                if (source.Load(userCurrencies.Select(currency => currency.CurrencyType.ToString()).ToList()))
                {
                    var loadedTypes = source.LoadedRates.Select(currency => currency.Key).ToList();
                    Currency currentCurrency = null;
                    CurrencyRate rate = null;

                    foreach (var loadedRate in source.LoadedRates)
                    {
                        currentCurrency = userCurrencies.FirstOrDefault(crncy => crncy.CurrencyType == loadedRate.Key);
                        
                        if (currentCurrency != null && decimal.Round(currentCurrency.Value, 4) != decimal.Round(loadedRate.Value,4))
                        {
                            currentCurrency.Value = loadedRate.Value;
                            Db.Entry(currentCurrency).State = EntityState.Modified;
                            rate = new CurrencyRate()
                            {
                                CurrencyId = currentCurrency.Id,
                                DateTime = DateTime.Now,
                                Value = decimal.Round(loadedRate.Value, 4)
                            };
                            Db.CurrencyRates.Add(rate);
                        }
                    }
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
                AddSource(privatBankSource1);

            var NBUSource = new NBURatesSource(
                "RatesNBU",
                new Uri("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json", UriKind.Absolute),
                null);
                AddSource(NBUSource);

            var CBRSource = new CBRRatesSource(
                "RatesCBR",
                new Uri("https://www.cbr-xml-daily.ru/daily_utf8.xml", UriKind.Absolute),
                null);
                AddSource(CBRSource);
        }
    }
}
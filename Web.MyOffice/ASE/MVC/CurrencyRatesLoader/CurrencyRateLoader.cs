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
            public bool UpdateRates(string name, Guid UserId)
            {
                var source = Sources.Find(src => src.Name == name);
                if (source == null) return false;

                var userCurrencies = Db.Currencies.Where(currency => currency.UserId == UserId).ToList();

                if (source.Load(UserCurrencyTypeString(userCurrencies)))
                {
                    var Idgroups = Db.CurrencyRates.Where(rate => rate.Currency.UserId == UserId)
                        .GroupBy(rate => rate.CurrencyId);
                    var lastDbRates = new List<CurrencyRate>();
                    foreach (var group in Idgroups)
                    {
                        lastDbRates.Add(group.FirstOrDefault(
                            rate => rate.DateTime == group.Max(grp => grp.DateTime)));
                    }

                   var  newDbRates = source.LoadedRates.Join(lastDbRates,
                            outer => outer.Key,
                            inner => inner.Currency.CurrencyType,
                            (outer, inner) => new CurrencyRate()
                            {
                                Id = inner.Id,
                                Currency = inner.Currency,
                                CurrencyId = inner.CurrencyId,
                                DateTime = DateTime.Now,
                                Value = Math.Round(inner.Value,4) != Math.Round(outer.Value, 4) ? outer.Value : 0
                            }).Where(rate => rate.Value != 0).Distinct().ToList();

                    var DbCurTypes = lastDbRates.Select(x => x.Currency.CurrencyType).ToList();
                    var LoadedRatesCurTypes = source.LoadedRates.Keys.ToList();
                    var newCurTypes = LoadedRatesCurTypes.Except(DbCurTypes).ToList();

                    newDbRates.AddRange(newCurTypes.Select(type => new CurrencyRate()
                    { 

                        Currency = userCurrencies.Find(cur => cur.CurrencyType == type),
                        CurrencyId = userCurrencies.Find(cur => cur.CurrencyType == type).Id,
                        DateTime=DateTime.Now,
                        Value = source.LoadedRates.ToList().Find(pair=>pair.Key == type).Value
                    }));

                   Db.CurrencyRates.AddRange(newDbRates);
                   newDbRates.ForEach(rate=> userCurrencies.Find(cur=>cur.Id == rate.CurrencyId).Value = rate.Value);
                   userCurrencies.ForEach(currency=> Db.Entry(currency).State = EntityState.Modified);
                   Db.SaveChanges();
                }
                return true;
            }

        private List<string> UserCurrencyTypeString(List<Currency> userCurrencies)
        {
            var userCurrenciesType = new List<string>();
            foreach (var currency in userCurrencies)
            {
                userCurrenciesType.Add(currency.CurrencyType.ToString());
            }
            return userCurrenciesType;
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
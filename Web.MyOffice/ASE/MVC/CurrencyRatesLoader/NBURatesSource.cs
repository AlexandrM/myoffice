using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using Web.MyOffice.Data;
using Web.MyOffice.Models;
using System.Web;

namespace ASE
{
    public class NBURatesSource : ICurrencyRateSource
    {
        private class NBUCurrency
        {
            double r030 { set; get; }
            public string txt { set; get; }
            public string cc { set; get; }
            public decimal rate { set; get; }
            public string exchangedate { set; get; }
        }
        public Uri BaseSource { set; get; }
        public Guid Id { set; get; }
        public string Name { set; get; }
        public Dictionary<string, string> RouteParams { set; get; }
        public Dictionary<CurrencyType, decimal> LoadedRates { set; get; }
        private bool isInited = false;
        public NBURatesSource(string name, Uri baseSource, Dictionary<string, string> routeParams)
        {
            BaseSource = baseSource;
            RouteParams = routeParams;
            Name = name;
            isInited = true;
            Id = Guid.NewGuid();
        }

        private string GetRouteParamString()
        {
            if (RouteParams == null) return string.Empty;
            var paramString = string.Empty;
            foreach (var param in RouteParams)
            {
                paramString = param.Key + "=" + param.Value + "&";
            }
            return paramString;
        }
        public bool Load()
        {
            if (isInited)
            {
                using (var wc = new WebClient())
                {
                    var stringRawData = wc.DownloadString(BaseSource.AbsoluteUri + GetRouteParamString()).Replace("RUB","RUR");
                    var loadedRates = JsonConvert.DeserializeObject<NBUCurrency[]>(stringRawData).ToList();
                    var currencyTypes = Enum.GetNames(typeof(CurrencyType)).ToList();

                    LoadedRates = loadedRates.Join(currencyTypes,
                        rate => rate.cc,
                        type => type,
                        (rate, type) =>
                        new KeyValuePair<string, decimal>(type, rate.rate))
                        .ToDictionary(KeyValuePair => (CurrencyType)Enum.Parse(typeof(CurrencyType), KeyValuePair.Key),
                                      KeyValuePair => KeyValuePair.Value);
                }
                return true;
            }
            else return false;
        }
    }
}
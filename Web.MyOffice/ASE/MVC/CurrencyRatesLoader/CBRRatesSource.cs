using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using Web.MyOffice.Data;
using Web.MyOffice.Models;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ASE
{
    public class CBRRatesSource:ICurrencyRateSource
    {
        private class CBRCurrency
        {
            public double NumCode { set; get; }
            public string Name { set; get; }
            public string CharCode { set; get; }
            public decimal Value { set; get; }
            public int Nominal { set; get; }
        }

        private class ValCurs
        {
           public  string @Date { set; get; }
           public string @Name { set; get; }
           public CBRCurrency[] Valute { set; get; }
        }

        private class CBRCurrencyContainer
        {
            public ValCurs ValCurs;
        }

        public Uri BaseSource { set; get; }
        public Guid Id { set; get; }
        public string Name { set; get; }
        public Dictionary<string, string> RouteParams { set; get; }
        public Dictionary<CurrencyType, decimal> LoadedRates { set; get; }
        private bool isInited = false;
        public CBRRatesSource(string name, Uri baseSource, Dictionary<string, string> routeParams)
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
        public bool Load(List<string> userTypes)
        {
            if (isInited)
            {
                using (var wc = new WebClient())
                {
                    var stringRawData = wc.DownloadString(BaseSource.AbsoluteUri + GetRouteParamString()).Replace(',','.');                    
                    var loadedRates = JsonConvert.DeserializeObject<CBRCurrencyContainer>(JsonConvert.SerializeXNode(XDocument.Parse(stringRawData).Root)).ValCurs.Valute.ToList(); 
                    LoadedRates = loadedRates.Join(userTypes.Distinct(),
                        rate => rate.CharCode,
                        type => type,
                        (rate, type) =>
                        new KeyValuePair<string, decimal>(type, rate.Value/ rate.Nominal))
                        .ToDictionary(KeyValuePair => (CurrencyType)Enum.Parse(typeof(CurrencyType), KeyValuePair.Key),
                                      KeyValuePair => KeyValuePair.Value);
                }
                return true;
            }
            else return false;
        }
    }
}
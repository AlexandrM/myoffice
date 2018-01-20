using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.MyOffice.Models;

namespace ASE
{
    public interface ICurrencyRateSource
    {
        Guid Id { set; get; }
        string Name { set; get; }
        Uri BaseSource { set; get; }
        Dictionary<string, string> RouteParams { set; get; }
        bool Load(List<string> userTypes);
        Dictionary<CurrencyType, decimal> LoadedRates { set; get; }
        CurrencyType BaseCurrency { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASE
{
    public interface ICurrencyRateLoader
    {
        List<ICurrencyRateSource> Sources { set; get; }
        void AddSource(ICurrencyRateSource source);
        void RemoveSource(ICurrencyRateSource source);
        ICurrencyRateSource FindSource(Guid Id);
        bool UpdateRates(string sourceName, Guid UserId);
    }
}

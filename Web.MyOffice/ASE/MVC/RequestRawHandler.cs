using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ASE.MVC
{
    public class RequestRawHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            HttpMessageContent requestContent = new HttpMessageContent(request);
            string requestMessage = requestContent.ReadAsStringAsync().Result;

            return await base.SendAsync(request, token);
        }
    }
}
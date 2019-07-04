using Microsoft.Extensions.Configuration;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Client.HttpHandlers
{
    public class PollyMessageHandler : DelegatingHandler
    {
        private readonly string serviceUrl;

        public PollyMessageHandler(IConfiguration configuration)
        {
            serviceUrl = configuration["CustomerService:Url"];
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => await SendAsync(request, new Uri(serviceUrl), cancellationToken);

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Uri uri, CancellationToken cancellationToken)
             => await Policy.Handle<Exception>()
                 .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                 .ExecuteAsync(async () =>
                 {
                     request.RequestUri = uri;
                     return await base.SendAsync(request, cancellationToken);
                 });
    }
}

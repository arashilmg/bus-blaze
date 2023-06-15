﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BizCover.Api.Renewals.IntegrationTests
{
    internal class ResponseVersionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            response.Version = request.Version;

            return response;
        }
    }
}

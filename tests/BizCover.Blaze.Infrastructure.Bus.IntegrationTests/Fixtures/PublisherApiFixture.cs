using System;
using System.Collections.Generic;
using System.Text;
using BizCover.Blaze.Infrastructure.Bus.Sample.Publisher;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace BizCover.Blaze.Infrastructure.Bus.IntegrationTests.Fixtures
{
    public class PublisherApiFixture : WebApplicationFactory<Startup>
    {
        public PublisherApiFixture():base()
        {
            using (CreateDefaultClient())
            {
            }
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            builder.ConfigureLogging((p) => p.AddXUnit());
            return base.CreateServer(builder);
        }
    }
}

using BizCover.Blaze.Infrastructure.Bus.Sample.Consumer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

namespace BizCover.Blaze.Infrastructure.Bus.IntegrationTests.Fixtures
{
    public class ConsumerApiFixture : WebApplicationFactory<Startup>
    {
        public ConsumerApiFixture():base()
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

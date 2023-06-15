using System;
using System.Linq;
using System.Threading.Tasks;
using BizCover.Application.Renewals.Rules.AutoEligibility;
using BizCover.Application.Renewals.Services;
using Grpc.Net.Client;
using MassTransit.AspNetCoreIntegration;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace BizCover.Api.Renewals.IntegrationTests.SubmitAutoRenewalOrder
{
    public class SubmitAutoRenewalOrderFixture<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly GrpcChannel _channel;
        
        public SubmitAutoRenewalOrderFixture()
        {
            Environment.SetEnvironmentVariable("SkipBus", "true");
            var client = CreateDefaultClient(new ResponseVersionHandler());
            _channel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions { HttpClient = client });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            builder.ConfigureTestServices(async services =>
            {
                services.AddSingleton<IOrderService, OrderServiceStub>();
                services.AddSingleton<IPolicyService, PolicyServiceStub>();
                services.AddSingleton<IRenewalEligibility, RenewalEligibilityStub>();
                services.AddSingleton<IPaymentService, PaymentServiceStub>();
                services.AddSingleton<IQuotationService, QuotationServiceStub>();
            });

        protected override async void Dispose(bool disposing)
        {
            _channel?.Dispose();
            base.Dispose(disposing);
        }
    }
}

using BizCover.Blaze.Infrastructure.Bus.IntegrationTests.Fixtures;
using BizCover.Blaze.Infrastructure.Bus.Sample.Publisher;
using BizCover.Blaze.Infrastructure.Bus.Sample.Publisher.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BizCover.Blaze.Infrastructure.Bus.IntegrationTests.Controllers
{
    public class BlazeBusIntegrationTests : IClassFixture<PublisherApiFixture>, IClassFixture<ConsumerApiFixture>
    {
        private readonly PublisherApiFixture _publisherApiFixture;
        private readonly ConsumerApiFixture _consumerApiFixture;

        public BlazeBusIntegrationTests(PublisherApiFixture publisherApiFixture, ConsumerApiFixture consumerApiFixture)
        {
            _publisherApiFixture = publisherApiFixture;
            _consumerApiFixture = consumerApiFixture;
        }

        [Fact]
        public async Task BlazeBus_PublishAndConsumeOrder_ReturnsSuccess()
        {
            var consumerClient = _consumerApiFixture.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost:5002")
            });

            await Task.Delay(5000);

            var publisherClient = _publisherApiFixture.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost:5001")
            });

            var orderId = "123";
            var productName = "abce";

            var orderEventModel = new OrderEventModel()
            {
                ProductName = productName,
                OrderId = orderId
            };

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(orderEventModel), Encoding.UTF8, "application/json");

            var publisherResponse = await publisherClient.PostAsync("buspublisher/publish", httpContent);
            var consumerResponse = await consumerClient.GetAsync($"busreceiver/get/{orderId}");
            var consumerResponseString = await consumerResponse.Content.ReadAsStringAsync();

            publisherResponse.IsSuccessStatusCode.Should().BeTrue();

            consumerResponse.IsSuccessStatusCode.Should().BeTrue();
            consumerResponseString.Should().NotBeNullOrEmpty();
        }
    }
}

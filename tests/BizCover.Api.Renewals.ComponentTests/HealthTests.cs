using BizCover.Api.Renewals.ComponentTests.Fixtures;
using Grpc.Net.Client;
using Xunit;

namespace BizCover.Api.Renewals.ComponentTests
{
    [Trait("Component", "Local")]

    public class HealthTests : IClassFixture<ComponentTestFixture>
    {
        private readonly HttpClient _client = new();
        private readonly GrpcChannel _renewalsGrpcChannel;

        public HealthTests(ComponentTestFixture fixture)
        {
            _client.BaseAddress = new Uri(fixture.SvcHttpUrl);
            _renewalsGrpcChannel = GrpcChannel.ForAddress(new Uri(fixture.SvcGrpcUrl));
        }

        [Fact]
        public async Task When_Check_Health_Then_Get_Healthy_Response()
        {
            //Act
            var result = await _client.GetStringAsync($"health/alive");

            //Assert
            Assert.Equal("Healthy", result);
        }
    }
}

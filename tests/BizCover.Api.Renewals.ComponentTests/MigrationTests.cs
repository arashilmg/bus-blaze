using BizCover.Api.Renewals.ComponentTests.Fixtures;
using Grpc.Net.Client;
using Xunit;

namespace BizCover.Api.Renewals.ComponentTests;

[Trait("Component", "Local")]

public class MigrationTests : IClassFixture<ComponentTestFixture>
{
    private readonly GrpcChannel _renewalsGrpcChannel;
    private readonly ComponentTestFixture _fixture;

    public MigrationTests(ComponentTestFixture fixture)
    {
        _fixture = fixture;
        _renewalsGrpcChannel = GrpcChannel.ForAddress(new Uri(fixture.SvcGrpcUrl));
    }

    [Fact(Skip = "LocalOnly")]
    public async Task Can_Perform_PolicyBoundEventMigration()
    {
        var httpClient = new HttpClient();
        var migrationUri = $"{_fixture.SvcHttpUrl}/migration";

        // act
        await httpClient.PostAsync(migrationUri, null);
    }
}

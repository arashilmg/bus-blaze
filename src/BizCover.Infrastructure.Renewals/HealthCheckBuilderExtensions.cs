using BizCover.Framework.Persistence.Mongo.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BizCover.Infrastructure.Renewals
{
    public static class HealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddRepositoryHealthCheck(this IHealthChecksBuilder checks, IConfiguration configuration) =>
            checks
                .AddMongoDb(
                    GetSection(configuration, "Url"),
                    mongoDatabaseName: GetSection(configuration, "DatabaseName"),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { HealthCheckTags.Database },
                    timeout: TimeSpan.FromSeconds(5));

        private static string GetSection(IConfiguration configuration, string key) =>
            configuration.GetSection(nameof(MongoOptions))[key];
    }
}

using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Configuration;
using System;

namespace BizCover.Blaze.Infrastructure.Bus.Internals
{
    public interface IResolveEndpoint
    {
        Uri ResolveDefaultEndpoint(string serviceName);
        Uri ResolveDefaultEndpointForSelf();
    }

    public class ResolveEndPoint : IResolveEndpoint
    {
        private readonly IConfiguration _configuration;

        public BusOptions BusOptions { get; set; }

        public AWSOptions AwsOptions { get; set; }

        public ResolveEndPoint(IConfiguration configuration)
        {
            _configuration = configuration;
            BusOptions = _configuration.GetBusOptions();
            AwsOptions = _configuration.GetAWSOptions();
        }

        public Uri ResolveDefaultEndpoint(string serviceName)
            => new Uri($"amazonsqs://{AwsOptions.Region.SystemName}/{BusOptions.Prefix}-{serviceName}");

        public Uri ResolveDefaultEndpointForSelf()
            => new Uri($"amazonsqs://{AwsOptions.Region.SystemName}/{BusOptions.QueuePrefix}");
    }
}

using BizCover.Blaze.Infrastructure.Bus.Internals;
using Microsoft.Extensions.Configuration;

namespace BizCover.Blaze.Infrastructure.Bus
{
    public static class ConfigurationExtensions
    {
        public static BusOptions GetBusOptions(this IConfiguration configuration)
        {
            BusOptions busOptions = new BusOptions();

            if (!string.IsNullOrEmpty(configuration[Constants.BlazeRegion]))
            {
                busOptions.BlazeRegion = configuration[Constants.BlazeRegion];
            }

            if (!string.IsNullOrEmpty(configuration[Constants.BlazeEnvironment]))
            {
                busOptions.BlazeEnvironment = configuration[Constants.BlazeEnvironment];
            }

            if (!string.IsNullOrEmpty(configuration[Constants.BlazeService]))
            {
                busOptions.BlazeService = configuration[Constants.BlazeService];
            }

            return busOptions;
        }
    }
}

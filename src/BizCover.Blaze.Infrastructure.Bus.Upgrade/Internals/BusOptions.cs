using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizCover.Blaze.Infrastructure.Bus.Upgrade.Internals
{
    public class BusOptions
    {
        public string Prefix => $"{BlazeEnvironment}-{BlazeRegion}-{Constants.PlatformName}";
        public string QueuePrefix => $"{Prefix}-{BlazeService}";
        public string BlazeRegion { get; set; } = Constants.DefaultBlazeRegion;
        public string BlazeEnvironment { get; set; } = Constants.DefaultBlazeEnvironment;
        public string BlazeService { get; set; } = Constants.DefaultBlazeService;
    }
}

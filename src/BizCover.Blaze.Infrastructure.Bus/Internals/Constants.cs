using System;
using System.Collections.Generic;
using System.Text;

namespace BizCover.Blaze.Infrastructure.Bus.Internals
{
    public class Constants
    {
        public static readonly string QueueTopicNameRegExString = @"[^\w\-$]";
        public static readonly int MaxTopicNameLength = 250;
        public static readonly int MaxQueueNameLength = 75;
        public static readonly string FaultTopicSuffix = "fault";
        public static readonly string DefaultBlazeRegion = "unknown-region";
        public static readonly string DefaultBlazeEnvironment = "unknown-environment";
        public static readonly string DefaultBlazeService = "unknown-service";
        public static readonly string BlazeRegion = "BLAZE_REGION";
        public static readonly string BlazeEnvironment = "BLAZE_ENVIRONMENT";
        public static readonly string BlazeService = "BLAZE_SERVICE";
        public static readonly string PlatformName = "blaze";
        public static readonly string MessagesNamespacePrefix = "BizCover.Messages.";

    }
}

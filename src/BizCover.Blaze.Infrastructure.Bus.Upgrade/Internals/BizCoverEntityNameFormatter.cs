using MassTransit;

namespace BizCover.Blaze.Infrastructure.Bus.Upgrade.Internals
{
    internal class BizCoverEntityNameFormatter : IEntityNameFormatter
    {
        private readonly BusOptions _busOptions;
        public BizCoverEntityNameFormatter(BusOptions busOptions)
        {
            _busOptions = busOptions;
        }
        public string FormatEntityName<T>()
        {
            if (typeof(T).GetInterfaces().Any(x => x.Name == nameof(Fault)))
            {
                return ConstructFaultTopicSuffix(typeof(T));
            }

            return ConstructTopicSuffix(typeof(T));
        }

        private string ConstructFaultTopicSuffix(Type type)
        {
            var topicSuffixFromType = type.GenericTypeArguments.FirstOrDefault().FullName.ToLowerInvariant()
                .StartsWith(Constants.MessagesNamespacePrefix.ToLowerInvariant())
                ? type.GenericTypeArguments.FirstOrDefault().FullName
                    .Replace(Constants.MessagesNamespacePrefix, string.Empty)
                    .Replace(type.GenericTypeArguments.FirstOrDefault()?.Name, string.Empty)
                    .TrimEnd('.')
                : string.Empty;
            var faultTopicSuffix = (!string.IsNullOrEmpty(topicSuffixFromType))
                ? $"{topicSuffixFromType}-{type.GenericTypeArguments.FirstOrDefault()?.Name}"
                : type.GenericTypeArguments.FirstOrDefault()?.Name;

            return (string.IsNullOrEmpty(faultTopicSuffix))
                 ? Constants.FaultTopicSuffix.ToAwsTopicName(_busOptions.Prefix)
                 : $"{faultTopicSuffix}-{Constants.FaultTopicSuffix}".ToAwsTopicName(_busOptions.Prefix);
        }

        private string ConstructTopicSuffix(Type type)
        {
            var topicSuffixFromType = type.FullName.ToLowerInvariant()
                .StartsWith(Constants.MessagesNamespacePrefix.ToLowerInvariant())
                ? type.FullName
                    .Replace(Constants.MessagesNamespacePrefix, string.Empty)
                    .Replace(type.Name, string.Empty)
                    .TrimEnd('.')
                : string.Empty;
            var topicSuffix= (!string.IsNullOrEmpty(topicSuffixFromType))
                ? $"{topicSuffixFromType}-{type.Name}"
                : type.Name;

            return topicSuffix.ToAwsTopicName(_busOptions.Prefix);
        }
    }
}

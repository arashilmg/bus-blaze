namespace BizCover.Blaze.Infrastructure.Bus.Upgrade.Internals
{
    public static class ErrorMessages
    {
        public static readonly string InvalidTopicName = $"Topic name should be of maximum of {Constants.MaxTopicNameLength} characters and will allow alphanumeric characters with hyphen(-) and underscore(_)";
        public static readonly string InvalidQueueName = $"Queue name should be of maximum of {Constants.MaxQueueNameLength} characters and will allow alphanumeric characters with hyphen(-) and underscore(_)";
    }
}

using System;
using System.Text.RegularExpressions;

namespace BizCover.Blaze.Infrastructure.Bus.Internals
{
    public static class StringExtenstions
    {
        public static string ToAwsTopicName(this string typeName, string prefix)
            => ConstructMessageExchangerName(typeName, Constants.MaxTopicNameLength, ErrorMessages.InvalidTopicName, prefix);

        public static string ToAwsQueueName(this string typeName)
            => ConstructMessageExchangerName(typeName,  Constants.MaxQueueNameLength, ErrorMessages.InvalidQueueName);

        public static string ToAwsDeadLetterQueueName(this string typeName)
            => $"{ToAwsQueueName(typeName)}_error";

        private static string ConstructMessageExchangerName(string typeName, int maxLength, string errorMessage, string namePrefix = null)
        {
            string name = string.Empty;

            if (!string.IsNullOrEmpty(namePrefix))
            {
                name = $"{namePrefix}-{typeName}";
            }
            else
            {
                name = typeName;
            }

            if (name.Length > maxLength || Regex.IsMatch(name, Constants.QueueTopicNameRegExString))
            {
                throw new ArgumentException(errorMessage);
            }

            return name;
        }
    }
}

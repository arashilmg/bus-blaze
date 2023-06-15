using BizCover.Blaze.Infrastructure.Bus.Internals;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace BizCover.Blaze.Infrastructure.Bus.Tests.Internals
{
    public class StringExtensionsTests
    {
        [Theory]
        [MemberData(nameof(ValidTopicNames))]
        public void ToAwsTopicName_ValidName_ReturnsTopicName(string prefix, string typeName, string expectedResult)
        {
            var actual = typeName.ToAwsTopicName(prefix);
            actual.Should().Be(expectedResult);
        }

        [Theory]
        [MemberData(nameof(InValidTopicNames))]
        public void ToAwsTopicName_InValidName_ThrowsArgumentException(string prefix, string typeName)
        {
            Func<string> actual = () => typeName.ToAwsTopicName(prefix);

            actual.Should().Throw<ArgumentException>().WithMessage(ErrorMessages.InvalidTopicName);
        }

        [Theory]
        [MemberData(nameof(ValidQueueNames))]
        public void ToAwsQueueName_ValidName_ReturnsTopicName(string prefix, string expectedResult)
        {
            var actual = prefix.ToAwsQueueName();
            actual.Should().Be(expectedResult);
        }

        [Theory]
        [MemberData(nameof(InValidQueueNames))]
        public void ToAwsQueueName_InValidName_ThrowsArgumentException(string prefix)
        {
            Func<string> actual = () => prefix.ToAwsQueueName();

            actual.Should().Throw<ArgumentException>().WithMessage(ErrorMessages.InvalidQueueName);
        }

        public static IEnumerable<object[]> ValidTopicNames =>
            new List<object[]>
            {
                // Prefix, TypeName, Expected Result
                new object[] {"au-dev-documents", "Order", "au-dev-documents-Order"},
                new object[] {"au-dev-documents", "Policy", "au-dev-documents-Policy"},
            };

        public static IEnumerable<object[]> ValidQueueNames =>
            new List<object[]>
            {
                // Prefix, Expected Result
                new object[] {"au-dev-documents",  "au-dev-documents"},
                new object[] {"au-dev-policy", "au-dev-policy"},
            };

        public static IEnumerable<object[]> InValidTopicNames =>
            new List<object[]>
            {
                // Prefix, TypeName
                new object[] {"au-dev-documents", "Order`1"},
                new object[] {new string('a',Constants.MaxTopicNameLength +1), "Policy"},
            };

        public static IEnumerable<object[]> InValidQueueNames =>
            new List<object[]>
            {
                // Prefix, TypeName
                new object[] {"au-dev-documents`1"},
                new object[] {new string('a',Constants.MaxQueueNameLength +1)},
            };
    }
}

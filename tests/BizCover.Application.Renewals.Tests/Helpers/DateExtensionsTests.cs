using BizCover.Application.Renewals.Helpers;
using FluentAssertions;
using Xunit;

namespace BizCover.Application.Renewals.Tests.Helpers
{
    public class DateExtensionsTests
    {
        [Theory]
        [MemberData(nameof(CalculateDateTestData))]
        public void CalculateDate_Should_Return_Substracted_Date_When_days_Are_Passed(DateTime date, int days,
            DateTime expectedDate)
        {
            date.CalculateDate(days).Should().Be(expectedDate);
        }

        public static IEnumerable<object[]> CalculateDateTestData =>
            new List<object[]>
            {
                new object[] { new DateTime(2022, 1, 10), 2, new DateTime(2022, 1, 8) },
                new object[] { new DateTime(2022, 1, 10), 3, new DateTime(2022, 1, 7) },
                new object[] { new DateTime(2022, 1, 10), 4, new DateTime(2022, 1, 6) },
                new object[] { new DateTime(2022, 1, 10), 6, new DateTime(2022, 1, 4) },
                new object[] { new DateTime(2022, 1, 10), 0, new DateTime(2022, 1, 10) }
            };
    }
}

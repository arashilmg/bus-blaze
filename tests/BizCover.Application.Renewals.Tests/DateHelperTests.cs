using System.Globalization;
using BizCover.Application.Renewals.Helpers;
using Xunit;

namespace BizCover.Application.Renewals.Tests;

public class DateHelperTests
{
    [Theory]
    [InlineData("19/01/2022 5:00:00 AM", "19/01/2022 4:00:00 PM", "19/01/2023 4:00:00 PM")]
    [InlineData("29/02/2000 5:00:00 AM", "29/02/2000 4:00:00 PM", "28/02/2001 4:00:00 PM")] 
    public void CalculateExpiryDateTests(string oldExpiryDateTimeValue, string expectedInceptionDate, string expectedExpiryDate)
    {
        var oldExpiryDateTime = DateTime.ParseExact(oldExpiryDateTimeValue, "dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

        var actualInceptionDate = DateHelper.CalculateInceptionDate(oldExpiryDateTime);
        var actualExpiryDate = DateHelper.CalculateExpiryDates(oldExpiryDateTime);

        Assert.Equal(expectedInceptionDate, actualInceptionDate);
        Assert.Equal(expectedExpiryDate, actualExpiryDate);
    }
}

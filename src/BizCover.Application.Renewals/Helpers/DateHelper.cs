using BizCover.Application.Renewals.Configuration;

namespace BizCover.Application.Renewals.Helpers;

internal static class DateHelper
{
    internal static string CalculateInceptionDate(DateTime expiryDateTime) => 
        DateOnly.FromDateTime(expiryDateTime)
            .WithCutOffTime()
            .ToString(DateTimeConfiguration.DateTimeFormat);

    internal static string CalculateExpiryDates(DateTime expiryDateTime) => 
        DateOnly.FromDateTime(expiryDateTime).AddYears(DateTimeConfiguration.DefaultRenewalYears)
            .WithCutOffTime()
            .ToString(DateTimeConfiguration.DateTimeFormat);

    private static DateTime WithCutOffTime(this DateOnly date) =>
        new(date.Year, date.Month, date.Day, DateTimeConfiguration.DefaultRenewalHour,
            DateTimeConfiguration.DefaultRenewalMinute, DateTimeConfiguration.DefaultRenewalSecond);
}

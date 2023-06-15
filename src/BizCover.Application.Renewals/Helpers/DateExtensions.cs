namespace BizCover.Application.Renewals.Helpers
{
    internal static class DateExtensions
    {
        internal static DateTime CalculateDate(this DateTime policyExpiryDate, int days) =>
            policyExpiryDate.AddDays(-days);
    }
}

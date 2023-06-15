namespace BizCover.Application.Renewals.Helpers;

internal static class ParametersExtension
{
    internal static string GetSection(this IDictionary<string, string> parameters, string code, int section) =>
        parameters.First(x => x.Key == code).Value.Split(":")[section];
}

using Polly;

namespace BizCover.Api.Renewals.ComponentTests.Fixtures;

internal static class HttpClientExtenstion
{
    internal static void BlockGetTillAvailable(this HttpClient client, string url)
    {
        const int retryIntervalInSeconds = 1;
        const int retryAttempts = 30;

        var polly = Policy
            .HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                retryAttempts,
                retryAttempt => TimeSpan.FromSeconds(retryIntervalInSeconds));

        Task.Run(async () => await polly.ExecuteAsync(async () => await client.GetAsync(url))).Wait();
    }
}

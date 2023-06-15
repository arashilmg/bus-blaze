using BizCover.Application.Renewals.UseCases.WordingChanges;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BizCover.Application.Renewals.Tests.UseCases
{
    public class WordingChangesConfigTests
    {
        [Theory, MemberData(nameof(TestData))]
        public void Can_get_wording_document_url_for_configured_product(string productCode, DateTime effectiveDate, string? expectedWordingUrl)
        {
            // setup
            IServiceCollection services = new ServiceCollection();
            services.AddWordingChangesConfig();

            // act
            using var sp = services.BuildServiceProvider();
            var wordingConfig = sp.GetRequiredService<WordingChangesConfig>();

            // assert
            var wordingUrl = wordingConfig.GetWordingConfigUrl(productCode, effectiveDate);
            wordingUrl.Should().Be(expectedWordingUrl);
        }

        public static IEnumerable<object[]> TestData =>
            new List<object[]>
            {
                new object[] { "PI-PL-VERO", new DateTime(2021, 3, 3), string.Empty},
                new object[] { "PI-PL-DUAL", new DateTime(2022, 1, 10), string.Empty},
                new object[] { "PI-PL-DUAL", new DateTime(2022,1,11), "https://insure.bizcover.com.au/WebForms/Public/documents/USA%20and%20Canada%20Cover%20Notice%20for%20Auto-renewals%20-%20DUAL%20123%20Pitt%20Street.pdf" },
                new object[] { "PI-PL-DUAL", new DateTime(2024, 4, 17),"https://insure.bizcover.com.au/WebForms/Public/documents/USA%20and%20Canada%20Cover%20Notice%20for%20Auto-renewals%20-%20DUAL%20123%20Pitt%20Street.pdf" },
            };
    }
}

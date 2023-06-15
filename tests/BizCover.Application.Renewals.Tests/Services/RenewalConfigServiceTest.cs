using BizCover.Application.Renewals.Configuration;
using BizCover.Application.Renewals.Exception;
using BizCover.Application.Renewals.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BizCover.Application.Renewals.Tests.Services
{
    public class RenewalConfigServiceTest
    {
        [Fact]
        public void CanAutoRenew_Return_True()
        {
            //Arrange
            var autoRenewalProductConfig = new CanAutoRenewProductConfig()
            {
                Products = new List<CanAutoRenewProduct>()
                {
                    new()
                    {
                        ProductCode = "PI-PL-DUAL",
                        CanAutoRenew = true,
                        EffectiveFrom = new DateTime(2022, 02, 01),
                    }
                }
            };

            var autoRenewalConfigService =
                new RenewalConfigService(autoRenewalProductConfig, new RenewalStepTriggerDayConfig());

            //Act and Assert
            Assert.True(autoRenewalConfigService.CanAutoRenew("PI-PL-DUAL", new DateTime(2022, 02, 01)));
        }

        [Fact]
        public void CanAutoRenew_Return_False()
        {
            //Arrange
            var autoRenewalProductConfig = new CanAutoRenewProductConfig()
            {
                Products = new List<CanAutoRenewProduct>()
                {
                    new()
                    {
                        ProductCode = "PI-PL-DUAL",
                        CanAutoRenew = true,
                        EffectiveFrom = new DateTime(2021, 02, 01),
                        EffectiveTo = new DateTime(2021, 12, 31)
                    },
                    new()
                    {
                        ProductCode = "PI-PL-DUAL",
                        CanAutoRenew = false,
                        EffectiveFrom = new DateTime(2022, 01, 01),
                        EffectiveTo = new DateTime(2022, 03, 31),
                    },
                    new()
                    {
                        ProductCode = "PI-PL-DUAL",
                        CanAutoRenew = true,
                        EffectiveFrom = new DateTime(2022, 04, 01),
                    },
                }
            };
            var autoRenewalConfigService =
                new RenewalConfigService(autoRenewalProductConfig, new RenewalStepTriggerDayConfig());

            //Act and Assert
            Assert.False(autoRenewalConfigService.CanAutoRenew("PI-PL-DUAL", new DateTime(2022, 01, 01)));
        }

        [Fact]
        public void CanAutoRenew_Throws_NotImplementedException()
        {
            var autoRenewalProductConfig = new CanAutoRenewProductConfig()
            {
                Products = new List<CanAutoRenewProduct>()
                {
                    new()
                    {
                        ProductCode = "PI-PL-DUAL",
                        CanAutoRenew = true,
                        EffectiveFrom = new DateTime(2022, 02, 01),
                    }
                }
            };
            var autoRenewalConfigService =
                new RenewalConfigService(autoRenewalProductConfig, new RenewalStepTriggerDayConfig());

            //Act and Assert
            Assert.Throws<ProductConfigMissingException>(() =>
                autoRenewalConfigService.CanAutoRenew("PI-PL-VERO", new DateTime(2021, 01, 01)));
        }

        [Fact]
        public void GetAutoRenewalStepTriggerDay_Should_Return_AutoRenewalStepTriggerDay()
        {
            var autoRenewalProductConfig = new RenewalStepTriggerDayConfig
            {
                Products = new List<RenewalStepTriggerDay>
                {
                    new()
                    {
                        ProductCode = "PI-PL-DUAL",
                        EffectiveFrom = DateTime.UtcNow.AddDays(-10),
                        Initiation = 1,
                        OrderGeneration = 20,
                        OrderSubmission = 100
                    },
                    new()
                    {
                        ProductCode = "PI-PL-VIRO",
                        EffectiveFrom = DateTime.UtcNow.AddDays(-10),
                        Initiation = 10,
                        OrderGeneration = 200,
                        OrderSubmission = 10
                    },
                }
            };
            var autoRenewalConfigService =
                new RenewalConfigService(new CanAutoRenewProductConfig(), autoRenewalProductConfig);

            var result =
                autoRenewalConfigService.GetRenewalStepTriggerDay("PI-PL-DUAL", DateTime.UtcNow.AddDays(-10));

            result.Initiation.Should().Be(1);
            result.OrderGeneration.Should().Be(20);
            result.OrderSubmission.Should().Be(100);
        }
    }
}

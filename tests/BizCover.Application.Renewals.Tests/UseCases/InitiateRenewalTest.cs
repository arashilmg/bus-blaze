using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.Application.Renewals.UseCases.WordingChanges;
using BizCover.Entity.Renewals;
using BizCover.Messages.Renewals;
using Moq;
using Xunit;

namespace BizCover.Application.Renewals.Tests.UseCases
{
    public class InitiateRenewalTest
    {
        private readonly Mock<IQueuePublisher> _mockQueuePublisher = new();
        private readonly Mock<IRenewalConfigService> _mockRenewalConfigService = new();
        private readonly Mock<IRenewalService> _renewalService = new();

        private readonly InitiateRenewal _initiateRenewal;

        public InitiateRenewalTest()
        {
            _initiateRenewal = new InitiateRenewal(
                _mockQueuePublisher.Object, 
                _renewalService.Object,
                _mockRenewalConfigService.Object, 
                new WordingChangesConfig());
        }

        [Fact]
        public async Task Initiate_Renewal_For_Auto()
        {
            var expiringPolicyId = Guid.NewGuid();

            _renewalService.Setup(x => x.GetRenewalDetailsForExpiringPolicy(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(new Renewal()
                {
                    ExpiringPolicyId = expiringPolicyId,
                    OrderId = null,
                    OptIn = true,
                    RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.AddHours(1) },
                    AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
                    PolicyStatus = PolicyStatus.Active
                }));

            _renewalService.Setup(x => x.HasArrears(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(false));

            _mockRenewalConfigService.Setup(x => x.CanAutoRenew(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(() => true);

            await _initiateRenewal.Initiate(expiringPolicyId, CancellationToken.None);

            _renewalService.Verify(x => x.SetInitiationDetails(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _mockQueuePublisher.Verify(x => x.Publish(It.IsAny<RenewalInitializedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Initiate_Renewal_For_SelfServe()
        {
            var expiringPolicyId = Guid.NewGuid();

            _renewalService.Setup(x => x.GetRenewalDetailsForExpiringPolicy(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(new Renewal()
                {
                    ExpiringPolicyId = expiringPolicyId,
                    OrderId = null,
                    OptIn = false,
                    RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.AddHours(1) },
                    AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
                    PolicyStatus = PolicyStatus.Active
                }));

            _renewalService.Setup(x => x.HasArrears(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(false));

            _mockRenewalConfigService.Setup(x => x.CanAutoRenew(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(() => true);

            await _initiateRenewal.Initiate(expiringPolicyId, CancellationToken.None);

            _renewalService.Verify(x => x.SetInitiationDetails(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

            _mockQueuePublisher.Verify(x => x.Publish(It.IsAny<RenewalInitializedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Initiate_SelfServe_Renewal_When_Payment_In_Arrears()
        {
            var expiringPolicyId = Guid.NewGuid();

            _renewalService.Setup(x => x.GetRenewalDetailsForExpiringPolicy(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(new Renewal()
                {
                    ExpiringPolicyId = expiringPolicyId,
                    OrderId = null,
                    OptIn = true,
                    RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.AddHours(1) },
                    AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
                    PolicyStatus = PolicyStatus.Active
                }));

            _renewalService.Setup(x => x.HasArrears(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(true));

            _mockRenewalConfigService.Setup(x => x.CanAutoRenew(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(() => true);

            await _initiateRenewal.Initiate(expiringPolicyId, CancellationToken.None);

            _renewalService.Verify(x => x.SetInitiationDetails(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

            _mockQueuePublisher.Verify(x => x.Publish(It.IsAny<RenewalInitializedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Initiate_SelfServe_Renewal_When_Product_Is_Not_Allowed_To_Do_AutoRenewal()
        {
            var expiringPolicyId = Guid.NewGuid();

            _renewalService.Setup(x => x.GetRenewalDetailsForExpiringPolicy(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(new Renewal()
                {
                    ExpiringPolicyId = expiringPolicyId,
                    OrderId = null,
                    OptIn = true,
                    RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.AddHours(1) },
                    AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
                    PolicyStatus = PolicyStatus.Active
                }));

            _renewalService.Setup(x => x.HasArrears(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(false));

            _mockRenewalConfigService.Setup(x => x.CanAutoRenew(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(() => false);

            await _initiateRenewal.Initiate(expiringPolicyId, CancellationToken.None);

            _renewalService.Verify(x => x.SetInitiationDetails(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

            _mockQueuePublisher.Verify(x => x.Publish(It.IsAny<RenewalInitializedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

using System.Linq.Expressions;
using BizCover.Application.Renewals.Services;
using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Exceptions;
using BizCover.Framework.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BizCover.Application.Renewals.Tests.Services
{
    public class RenewalServiceTests
    {
        [Fact]
        public void GetRenewalDetails_NotFound()
        {
            // Arrange
            var repository = new Mock<IRepository<Renewal>>();
            var paymentService = new Mock<IPaymentService>();

            var renewalService = new RenewalService(repository.Object, paymentService.Object, null);

            repository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Renewal, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult((new List<Renewal>()).AsEnumerable()));

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException<Renewal>>(() =>
                  renewalService.GetRenewalDetailsByOrderId(It.IsAny<Guid>(), CancellationToken.None));
        }

        [Fact]
        public async Task GetRenewalDetails_Found()
        {
            // Arrange
            var expiringPolicyId = Guid.Parse("A30A54DB-9763-4546-A2CA-A9F53C205D3A");
            var repository = new Mock<IRepository<Renewal>>();
            var paymentService = new Mock<IPaymentService>();
            var renewalService = new RenewalService(repository.Object, paymentService.Object, null);

            repository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Renewal, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult((new List<Renewal>()
                {
                    new Renewal()
                    {
                        ExpiringPolicyId = expiringPolicyId
                    }
                }).AsEnumerable()));

            // Act
            var response = await renewalService.GetRenewalDetailsByOrderId(It.IsAny<Guid>(), CancellationToken.None);
            
            // Assert
            Assert.NotNull(response);
            Assert.Equal(expiringPolicyId, response.ExpiringPolicyId);
        }
    }
}

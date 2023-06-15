using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Exceptions;
using BizCover.Framework.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BizCover.Application.Renewals.Services;

internal class RenewalService : IRenewalService
{
    private readonly IRepository<Renewal> _renewalRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<RenewalService> _logger;

    public RenewalService(IRepository<Renewal> renewalRepository, IPaymentService paymentService, ILogger<RenewalService> logger)
    {
        _renewalRepository = renewalRepository;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<Renewal> GetRenewalDetailsByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var renewal = (await _renewalRepository.FindAsync(x => x.OrderId == orderId, cancellationToken)).SingleOrDefault();
        return renewal ?? throw new NotFoundException<Renewal>(orderId);
    }

    public async Task<Renewal> GetRenewalDetailsForExpiringPolicy(Guid expiringPolicyId, CancellationToken cancellationToken)
    {
        var renewal = (await _renewalRepository.FindAsync(x => x.ExpiringPolicyId == expiringPolicyId, cancellationToken)).FirstOrDefault();
        return renewal ?? throw new NotFoundException<Renewal>(expiringPolicyId);
    }

    public async Task<Renewal> GetRenewalDetailsForRenewedPolicy(Guid renewedPolicyId, CancellationToken cancellationToken)
        => (await _renewalRepository.FindAsync(x => x.RenewedPolicyId == renewedPolicyId, cancellationToken)).FirstOrDefault();


    public async Task<bool> HasArrears(Guid expiringPolicyId, CancellationToken cancellationToken)
    {
        var renewal = await GetRenewal(expiringPolicyId, cancellationToken);
        
        if (!await _paymentService.HasArrears(expiringPolicyId)) return false;

        renewal.HasArrears = true;
        await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);

        return true;
    }

    public async Task SetInitiationDetails(Guid expiringPolicyId, CancellationToken cancellationToken)
    {
        var renewal = await GetRenewal(expiringPolicyId, cancellationToken);
        renewal.RenewalDates!.Initiated = DateTime.UtcNow;
        await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
    }

    public async Task SetOrderGeneratedDetails(Guid expiringPolicyId, Guid orderId, CancellationToken cancellationToken)
    {
        var renewal = await GetRenewal(expiringPolicyId, cancellationToken);
        renewal.OrderId = orderId;
        renewal.RenewalDates!.OrderGenerated = DateTime.UtcNow;
        await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
    }

    public async Task SetOrderSubmissionDetails(Guid expiringPolicyId, CancellationToken cancellationToken)
    {
        var renewal = await GetRenewal(expiringPolicyId, cancellationToken);
        renewal.RenewalDates!.OrderSubmitted = DateTime.UtcNow;
        await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
    }

    private async Task<Renewal> GetRenewal(Guid expiringPolicyId, CancellationToken cancellationToken)
    {
        var renewal = (await _renewalRepository.FindAsync(x => x.ExpiringPolicyId == expiringPolicyId, cancellationToken)).SingleOrDefault();
        return renewal ?? throw new NotFoundException<Renewal>(expiringPolicyId);
    }

    public async Task UpdateAllRenewalsFlagToTrueFromNull(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating all renewals flags from null to true started");
        var stringBuilder = new StringBuilder();
        var renewals = await _renewalRepository.FindAsync(x => x.AllRenewalsEnabled == null, cancellationToken);

        foreach(var renewal in renewals)
        {
            stringBuilder.AppendLine($"updating expiring policy {renewal.ExpiringPolicyId} all renewal to true");
            renewal.AllRenewalsEnabled = new RenewalsEnabled
            {
                IsEnabled = true,
                UpdatedAt = DateTime.UtcNow
            };
            await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
        }
        _logger.LogInformation(stringBuilder.ToString());
        _logger.LogInformation("finished updating all renewals flags");
    }
}

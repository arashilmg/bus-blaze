using System.Runtime.CompilerServices;
using BizCover.Application.Policies;
using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Messages.Renewals;
using Microsoft.Extensions.Logging;

namespace BizCover.Application.Renewals.UseCases;

public class PolicyBoundEventMigration
{
    private readonly ILogger _logger;
    private readonly IQueuePublisher _queuePublisher;
    private readonly IPolicyService _policyService;

    public PolicyBoundEventMigration(
        ILogger<PolicyBoundEventMigration> logger,
        IQueuePublisher queuePublisher,
        IPolicyService policyService)
    {
        _logger = logger;
        _queuePublisher = queuePublisher;
        _policyService = policyService;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        await foreach (var policies in GetAllPolicies(cancellationToken))
        {
            var policyList = policies.ToList();
            await HandleActivePolicies(policyList, cancellationToken);
        }
    }

    private async Task HandleActivePolicies(IEnumerable<PolicyDto> policies, CancellationToken cancellationToken)
    {
        foreach (var policy in policies)
        {
            await _queuePublisher.Send(new MigratePolicyCommand
            {
                PolicyId = policy.PolicyId,
                ExpiryDate = policy.ExpiryDate.ToDateTime(),
                InceptionDate = policy.InceptionDate.ToDateTime(),
                ProductCode = policy.ProductCode,
                Status = policy.Status
            }, cancellationToken);
        }
    }

    private async IAsyncEnumerable<IEnumerable<PolicyDto>> GetAllPolicies([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var offset = 0;
        const int fetch = 100;

        _logger.LogInformation("Begin Migration");

        while (true)
        {
            var policies = (await GetPolicies(offset, fetch, cancellationToken)).ToList();
            if (!policies.Any())
            {
                break;
            }

            yield return policies;
            offset += fetch;
        }

        _logger.LogInformation($"End Migration");
    }

    private async Task<IEnumerable<PolicyDto>> GetPolicies(int offset, int fetch, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Finding Policies for Migration - {offset} - {offset + fetch}");
        return await _policyService.FindPolicies(offset, fetch, cancellationToken);
    }
}

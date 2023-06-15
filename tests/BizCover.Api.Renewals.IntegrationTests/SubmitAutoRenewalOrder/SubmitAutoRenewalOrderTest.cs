using System;
using System.Threading;
using System.Threading.Tasks;
using BizCover.Application.Renewals.Services;
using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Interfaces;
using BizCover.Messages.Policies;
using BizCover.Messages.Scheduler;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Xunit;

namespace BizCover.Api.Renewals.IntegrationTests.SubmitAutoRenewalOrder
{
    public class SubmitAutoRenewalOrderTest : IClassFixture<SubmitAutoRenewalOrderFixture<Program>>
    {
        private readonly SubmitAutoRenewalOrderFixture<Program> _submitAutoRenewalOrderFixture;
        public SubmitAutoRenewalOrderTest(SubmitAutoRenewalOrderFixture<Program> submitAutoRenewalOrderFixture)
        {
            _submitAutoRenewalOrderFixture = submitAutoRenewalOrderFixture;
        }

        [Fact]
        public async Task PI_PL_Should_Generate_Submit_AutoRenewalOrder()
        {
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = Guid.NewGuid();
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy_PI_PL(expiringPolicyId.ToString(), policyBoundDate);

            using var scope = _submitAutoRenewalOrderFixture.Services.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            var renewalService = scope.ServiceProvider.GetService<IRenewalService>();
            var renewalRepository = scope.ServiceProvider.GetService<IRepository<Renewal>>();
            
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(1 * 5));

            // Step 1: Prepare renewal data by publishing policy bound event
            await publishEndpoint.Publish<PolicyBoundEvent>(expiringPolicyBoundEvent);
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.Equal(expiringPolicyId, renewalRecord.ExpiringPolicyId);
            });

            // Update Initiation, Order Generation and Order Submission date to date to perform renewal 
            var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
            renewalRecord.RenewalDates!.Initiation = DateTime.UtcNow;
            renewalRecord.RenewalDates!.OrderGeneration = DateTime.UtcNow;
            renewalRecord.RenewalDates!.OrderSubmission = DateTime.UtcNow;
            await renewalRepository.UpdateAsync(renewalRecord.Id, renewalRecord);

            // Step 2: Initialize Renewal
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.True(renewalRecord.RenewalDates!.Initiated.HasValue);
            });

            // Step 3: Generate AutoRenewal Order
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.True(renewalRecord.RenewalDates!.OrderGenerated.HasValue);
            });

            // Step 4: Submit Auto renewal order 
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.True(renewalRecord.RenewalDates!.OrderSubmitted.HasValue);
            });
        }

        [Fact]
        public async Task PA_Should_Not_Generate_Submit_AutoRenewalOrder()
        {
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = Guid.NewGuid();
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy_PA(expiringPolicyId.ToString(), policyBoundDate);

            using var scope = _submitAutoRenewalOrderFixture.Services.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            var renewalService = scope.ServiceProvider.GetService<IRenewalService>();
            var renewalRepository = scope.ServiceProvider.GetService<IRepository<Renewal>>();

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(1 * 5));

            // Step 1: Prepare renewal data by publishing policy bound event
            await publishEndpoint.Publish<PolicyBoundEvent>(expiringPolicyBoundEvent);
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.Equal(expiringPolicyId, renewalRecord.ExpiringPolicyId);
            });

            // Update Initiation, Order Generation and Order Submission date to date to perform renewal 
            var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
            renewalRecord.RenewalDates!.Initiation = DateTime.UtcNow;
            renewalRecord.RenewalDates!.OrderGeneration = DateTime.UtcNow;
            renewalRecord.RenewalDates!.OrderSubmission = DateTime.UtcNow;
            await renewalRepository.UpdateAsync(renewalRecord.Id, renewalRecord);

            // Step 2: Initialize Renewal
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.True(renewalRecord.RenewalDates!.Initiated.HasValue);
            });

            // Step 3: Generate AutoRenewal Order
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.False(renewalRecord.RenewalDates!.OrderGenerated.HasValue);
            });

            // Step 4: Submit Auto renewal order 
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.False(renewalRecord.RenewalDates!.OrderSubmitted.HasValue);
            });
        }

        [Fact]
        public async Task WhenRenewalNotEligible_DoNot_InitiateRenewal_Test()
        {
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = Guid.NewGuid();
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy_PI_PL(expiringPolicyId.ToString(), policyBoundDate);

            using var scope = _submitAutoRenewalOrderFixture.Services.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            var renewalService = scope.ServiceProvider.GetService<IRenewalService>();
            var renewalRepository = scope.ServiceProvider.GetService<IRepository<Renewal>>();

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(1 * 5));

            // Step 1: Prepare renewal data by publishing policy bound event
            await publishEndpoint.Publish<PolicyBoundEvent>(expiringPolicyBoundEvent);
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.Equal(expiringPolicyId, renewalRecord.ExpiringPolicyId);
            });

            // Update Initiation and AutoRenewalEligibility to false
            var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
            renewalRecord.RenewalDates!.Initiation = DateTime.UtcNow;
            renewalRecord.AutoRenewalEligibility = new AutoRenewalEligibility() { IsEligible = false };
            await renewalRepository.UpdateAsync(renewalRecord.Id, renewalRecord);

            // Step 2: Raise status change and it should not initiate 
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.False(renewalRecord.RenewalDates!.Initiated.HasValue);
            });
        }

        [Fact]
        public async Task WhenOptIn_Flag_False_DoNot_InitiateRenewal_Test()
        {
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = Guid.NewGuid();
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy_PI_PL(expiringPolicyId.ToString(), policyBoundDate);

            using var scope = _submitAutoRenewalOrderFixture.Services.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            var renewalService = scope.ServiceProvider.GetService<IRenewalService>();
            var renewalRepository = scope.ServiceProvider.GetService<IRepository<Renewal>>();

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(1 * 5));

            // Step 1: Prepare renewal data by publishing policy bound event
            await publishEndpoint.Publish<PolicyBoundEvent>(expiringPolicyBoundEvent);
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.Equal(expiringPolicyId, renewalRecord.ExpiringPolicyId);
            });

            // Update Initiation and OptIn to false
            var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
            renewalRecord.RenewalDates!.Initiation = DateTime.UtcNow;
            renewalRecord.OptIn = false;
            await renewalRepository.UpdateAsync(renewalRecord.Id, renewalRecord);

            // Step 2: Raise status change and it should not initiate 
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.False(renewalRecord.RenewalDates!.Initiated.HasValue);
            });

            // Step 3: Raise status change and it should not generate order 
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.False(renewalRecord.RenewalDates!.OrderGenerated.HasValue);
            });
        }

        [Fact]
        public async Task WhenOptIn_Flag_False_DoNot_SubmitOrder_Test()
        {
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = Guid.NewGuid();
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy_PI_PL(expiringPolicyId.ToString(), policyBoundDate);

            using var scope = _submitAutoRenewalOrderFixture.Services.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            var renewalService = scope.ServiceProvider.GetService<IRenewalService>();
            var renewalRepository = scope.ServiceProvider.GetService<IRepository<Renewal>>();
            
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(1 * 5));

            // Step 1: Prepare renewal data by publishing policy bound event
            await publishEndpoint.Publish<PolicyBoundEvent>(expiringPolicyBoundEvent);
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.Equal(expiringPolicyId, renewalRecord.ExpiringPolicyId);
            });

            // Update Initiation, Order Generation and Order Submission date to date to perform renewal 
            var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
            renewalRecord.RenewalDates!.Initiation = DateTime.UtcNow;
            renewalRecord.RenewalDates!.OrderGeneration = DateTime.UtcNow;
            renewalRecord.RenewalDates!.OrderSubmission = DateTime.UtcNow;
            await renewalRepository.UpdateAsync(renewalRecord.Id, renewalRecord);

            // Step 2: Initialize Renewal
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.True(renewalRecord.RenewalDates!.Initiated.HasValue);
            });

            // Step 3: Generate AutoRenewal Order
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.True(renewalRecord.RenewalDates!.OrderGenerated.HasValue);
            });

            // Step 4: Raise status change and it should not submit order 
            await publishEndpoint.Publish(new StatusChange());
            await retryPolicy.ExecuteAsync(async () =>
            {
                var renewalRecord = await renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, new CancellationToken());
                Assert.False(renewalRecord.RenewalDates!.OrderSubmitted.HasValue);
            });
        }

        private object GetPolicyBoundEventForExpiringPolicy_PI_PL(string expiringPolicyId, DateTime policyBoundDate)
        {
            var offerId = Guid.Parse(expiringPolicyId);
            return new 
            {
                PolicyId = expiringPolicyId,
                PolicyNumber = "ABC-123",
                Status = "Active",
                OrderId = Guid.NewGuid().ToString(),
                QuotationId = Guid.NewGuid().ToString(),
                ContactId = Guid.NewGuid().ToString(),
                ProductCode = "PI-PL-DUAL",
                ProductTypeCode = "PI-PL",
                PolicyBoundDate = policyBoundDate,
                InceptionDate = policyBoundDate,
                ExpiryDate = policyBoundDate.AddYears(1),
                OfferId = offerId
            };
        }

        private object GetPolicyBoundEventForExpiringPolicy_PA(string expiringPolicyId, DateTime policyBoundDate)
        {
            var offerId = Guid.Parse(expiringPolicyId);
            return new
            {
                PolicyId = expiringPolicyId,
                PolicyNumber = "ABC-123",
                Status = "Active",
                OrderId = Guid.NewGuid().ToString(),
                QuotationId = Guid.NewGuid().ToString(),
                ContactId = Guid.NewGuid().ToString(),
                ProductCode = "PA-DUAL",
                ProductTypeCode = "PA",
                PolicyBoundDate = policyBoundDate,
                InceptionDate = policyBoundDate,
                ExpiryDate = policyBoundDate.AddYears(1),
                OfferId = offerId
            };
        }
    }
}

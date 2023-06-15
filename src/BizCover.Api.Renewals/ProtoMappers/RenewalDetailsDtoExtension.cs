using BizCover.Application.Renewals;
using BizCover.Application.Renewals.UseCases.RenewalDetails;
using AutoRenewalEligibilityDto = BizCover.Application.Renewals.AutoRenewalEligibilityDto;
using RenewalDatesDto = BizCover.Application.Renewals.RenewalDatesDto;
using SpecialCircumstancesDto = BizCover.Application.Renewals.SpecialCircumstancesDto;

namespace BizCover.Api.Renewals.ProtoMappers;

internal static class RenewalDetailsDtoExtension
{
    internal static GetRenewalDetailsForExpiringPolicyResponse ToGrpcResponse(this RenewalDetailsDto renewal)
    {
        var response = new GetRenewalDetailsForExpiringPolicyResponse
        {
            ExpiringPolicyId = renewal.ExpiringPolicyId.ToString(),
            ProductCode = renewal.ProductCode,
            PolicyStatus = renewal.PolicyStatus,
            OrderId = renewal.OrderId.ToString(),
            PolicyExpiryDate = renewal.PolicyExpiryDate.ToGrpcTimestamp(),
            PolicyInceptionDate = renewal.PolicyInceptionDate.ToGrpcTimestamp(),
            OptIn = renewal.OptIn,
            HasArrears = renewal.HasArrears,
            RenewedPolicyId = renewal.RenewedPolicyId.ToString(),
            EnableAllRenewal = renewal.AllEnabledFlag
        };
      

        if (!string.IsNullOrWhiteSpace(renewal.RenewalType))
            response.RenewalType = renewal.RenewalType;

        if (renewal.RenewedDate.HasValue)
            response.RenewedDate = renewal.RenewedDate.Value.ToGrpcTimestamp();

        if (renewal.RenewalDates != null)
        {
            response.RenewalDates = new RenewalDatesDto
            {
                Initiation = renewal.RenewalDates.Initiation.ToGrpcTimestamp(),
                OrderGeneration = renewal.RenewalDates.OrderGeneration.ToGrpcTimestamp(),
                OrderSubmission = renewal.RenewalDates.OrderSubmission.ToGrpcTimestamp(),
            };

            if (renewal.RenewalDates.Initiated.HasValue)
                response.RenewalDates.Initiated = renewal.RenewalDates.Initiated.Value.ToGrpcTimestamp();

            if (renewal.RenewalDates.OrderGenerated.HasValue)
                response.RenewalDates.OrderGenerated = renewal.RenewalDates.OrderGenerated.Value.ToGrpcTimestamp();

            if (renewal.RenewalDates.OrderSubmitted.HasValue)
                response.RenewalDates.OrderSubmitted = renewal.RenewalDates.OrderSubmitted.Value.ToGrpcTimestamp();
        }

        if (renewal.AutoRenewalEligibility != null)
        {
            response.AutoRenewalEligibility = new AutoRenewalEligibilityDto
            {
                IsEligible = renewal.AutoRenewalEligibility.IsEligible,
                Comments = string.IsNullOrEmpty(renewal.AutoRenewalEligibility.Comments)
                    ? string.Empty : renewal.AutoRenewalEligibility.Comments,
                UpdatedAt = renewal.AutoRenewalEligibility.UpdatedAt.ToGrpcTimestamp()
            };
        }

        if (renewal.SpecialCircumstances != null)
        {
            response.SpecialCircumstances = new SpecialCircumstancesDto
            {
                IsApplied = renewal.SpecialCircumstances.IsApplied,
                Comments = renewal.SpecialCircumstances.Comments ?? String.Empty,
                UpdatedAt = renewal.SpecialCircumstances.UpdatedAt.ToGrpcTimestamp(),
                Reason = renewal.SpecialCircumstances.Reason ?? String.Empty,
                SecondLevelReason = renewal.SpecialCircumstances.SecondLevelReason ?? String.Empty
            };
        }

        return response;
    }

    internal static RenewalDto ToRenewalDto(this RenewalDetailsDto renewal)
    {
        var response = new RenewalDto
        {
            ExpiringPolicyId = renewal.ExpiringPolicyId.ToString(),
            ProductCode = renewal.ProductCode,
            PolicyStatus = renewal.PolicyStatus,
            OrderId = renewal.OrderId.ToString(),
            PolicyExpiryDate = renewal.PolicyExpiryDate.ToGrpcTimestamp(),
            PolicyInceptionDate = renewal.PolicyInceptionDate.ToGrpcTimestamp(),
            OptIn = renewal.OptIn,
            HasArrears = renewal.HasArrears,
            RenewedPolicyId = renewal.RenewedPolicyId.ToString(),
            EnableAllRenewal = renewal.AllEnabledFlag
        };


        if (!string.IsNullOrWhiteSpace(renewal.RenewalType))
            response.RenewalType = renewal.RenewalType;

        if (renewal.RenewedDate.HasValue)
            response.RenewedDate = renewal.RenewedDate.Value.ToGrpcTimestamp();

        if (renewal.RenewalDates != null)
        {
            response.RenewalDates = new RenewalDatesDto
            {
                Initiation = renewal.RenewalDates.Initiation.ToGrpcTimestamp(),
                OrderGeneration = renewal.RenewalDates.OrderGeneration.ToGrpcTimestamp(),
                OrderSubmission = renewal.RenewalDates.OrderSubmission.ToGrpcTimestamp(),
            };

            if (renewal.RenewalDates.Initiated.HasValue)
                response.RenewalDates.Initiated = renewal.RenewalDates.Initiated.Value.ToGrpcTimestamp();

            if (renewal.RenewalDates.OrderGenerated.HasValue)
                response.RenewalDates.OrderGenerated = renewal.RenewalDates.OrderGenerated.Value.ToGrpcTimestamp();

            if (renewal.RenewalDates.OrderSubmitted.HasValue)
                response.RenewalDates.OrderSubmitted = renewal.RenewalDates.OrderSubmitted.Value.ToGrpcTimestamp();
        }

        if (renewal.AutoRenewalEligibility != null)
        {
            response.AutoRenewalEligibility = new AutoRenewalEligibilityDto
            {
                IsEligible = renewal.AutoRenewalEligibility.IsEligible,
                Comments = string.IsNullOrEmpty(renewal.AutoRenewalEligibility.Comments)
                    ? string.Empty : renewal.AutoRenewalEligibility.Comments,
                UpdatedAt = renewal.AutoRenewalEligibility.UpdatedAt.ToGrpcTimestamp()
            };
        }

        if (renewal.SpecialCircumstances != null)
        {
            response.SpecialCircumstances = new SpecialCircumstancesDto
            {
                IsApplied = renewal.SpecialCircumstances.IsApplied,
                Comments = renewal.SpecialCircumstances.Comments ?? String.Empty,
                UpdatedAt = renewal.SpecialCircumstances.UpdatedAt.ToGrpcTimestamp(),
                Reason = renewal.SpecialCircumstances.Reason ?? String.Empty,
                SecondLevelReason = renewal.SpecialCircumstances.SecondLevelReason ?? String.Empty
            };
        }


        return response;
    }
}

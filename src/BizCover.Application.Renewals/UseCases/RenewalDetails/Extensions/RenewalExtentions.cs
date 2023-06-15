using BizCover.Application.Renewals.DTO;
using BizCover.Entity.Renewals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizCover.Application.Renewals.UseCases.RenewalDetails.Extensions
{
    public static  class RenewalExtentions
    {
        public static RenewalDetailsDto ToRenewalDetailsDto (this Renewal renewal)
        {
            var renewalDetailsDto = new RenewalDetailsDto
            {
                ExpiringPolicyId = renewal.ExpiringPolicyId,
                ProductCode = renewal.ProductCode,
                PolicyStatus = renewal.PolicyStatus,
                OrderId = renewal.OrderId,
                PolicyExpiryDate = renewal.PolicyExpiryDate,
                PolicyInceptionDate = renewal.PolicyInceptionDate,
                OptIn = renewal.OptIn,
                RenewedPolicyId = renewal.RenewedPolicyId,
                HasArrears = renewal.HasArrears,
                RenewedDate = renewal.RenewedDate,
                AllEnabledFlag = renewal.AllRenewalsEnabled?.IsEnabled ?? true
            };


            if (renewal.AllRenewalsEnabled != null)
            {
                renewalDetailsDto.AllEnabledFlag = renewal.AllRenewalsEnabled.IsEnabled;
            }

            if (renewal.RenewalDates != null)
            {
                renewalDetailsDto.RenewalDates = new RenewalDatesDto
                {
                    Initiation = renewal.RenewalDates.Initiation,
                    OrderGeneration = renewal.RenewalDates.OrderGeneration,
                    OrderSubmission = renewal.RenewalDates.OrderSubmission,
                    Initiated = renewal.RenewalDates.Initiated,
                    OrderGenerated = renewal.RenewalDates.OrderGenerated,
                    OrderSubmitted = renewal.RenewalDates.OrderSubmitted,
                };
            }

            if (renewal.AutoRenewalEligibility != null)
            {
                renewalDetailsDto.AutoRenewalEligibility = new AutoRenewalEligibilityDto
                {
                    IsEligible = renewal.AutoRenewalEligibility.IsEligible,
                    Comments = renewal.AutoRenewalEligibility.Comments,
                    UpdatedAt = renewal.AutoRenewalEligibility.UpdatedAt
                };
            }

            if (renewal.SpecialCircumstances != null)
            {
                renewalDetailsDto.SpecialCircumstances = new SpecialCircumstancesDto
                {
                    IsApplied = renewal.SpecialCircumstances.IsApplied,
                    Comments = renewal.SpecialCircumstances.Comments,
                    UpdatedAt = renewal.SpecialCircumstances.UpdatedAt,
                    Reason = renewal.SpecialCircumstances.Reason,
                    SecondLevelReason = renewal.SpecialCircumstances.SecondLevelReason
                };
            }
            else
            {
                renewalDetailsDto.SpecialCircumstances = new SpecialCircumstancesDto { IsApplied = false, UpdatedAt = DateTime.UtcNow};
            }

            return renewalDetailsDto;
        }
    }
}

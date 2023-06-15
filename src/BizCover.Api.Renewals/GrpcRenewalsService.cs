using BizCover.Api.Renewals.ProtoMappers;
using BizCover.Application.Renewals;
using BizCover.Application.Renewals.Rules.AutoEligibility;
using BizCover.Application.Renewals.Rules.ReQuoteEligibility;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.Application.Renewals.UseCases.GenerateRenewalOrder;
using BizCover.Application.Renewals.UseCases.RenewalDetails;
using BizCover.Application.Renewals.UseCases.WordingChanges;
using Google.Protobuf.WellKnownTypes;
using Google.Type;
using Grpc.Core;
using GenerateOrderResponse = BizCover.Application.Renewals.GenerateOrderResponse;
using GetRenewalDetailsResponse = BizCover.Application.Renewals.GetRenewalDetailsResponse;

namespace BizCover.Api.Renewals
{
    public class GrpcRenewalsService : RenewalsService.RenewalsServiceBase
    {
        private readonly GenerateRenewalOrder _generateRenewalOrder;
        private readonly GetRenewalDetails _getRenewalDetails;
        private readonly UpdateAutoRenewalEligibility _updateAutoRenewalEligibility;
        private readonly UpdateAutoRenewalOptInFlag _updateAutoRenewalOptInFlag;

        private readonly UpdateEnableAllRenewalFlag _updateEnableAllRenewalFlag;
        private readonly ISubmitRenewalOrder _submitRenewalOrder;
        private readonly UpdateSpecialCircumstances _updateSpecialCircumstances;
        private readonly IRenewalEligibility _renewalEligibility;
        private readonly IReQuoteRenewalEligibility _reQuoteRenewalEligibility;

        private readonly IPolicyService _policyService;
        private readonly WordingChangesConfig _wordingChangesConfig;

        public GrpcRenewalsService(
            GenerateRenewalOrder generateRenewalOrder,
            GetRenewalDetails getRenewalDetails,
            UpdateAutoRenewalEligibility updateAutoRenewalEligibility,
            UpdateAutoRenewalOptInFlag updateAutoRenewalOptInFlag,
            UpdateEnableAllRenewalFlag updateEnableAllRenewalFlag,
            ISubmitRenewalOrder submitRenewalOrder,
            UpdateSpecialCircumstances updateSpecialCircumstances, 
            IRenewalEligibility renewalEligibility,
            IPolicyService policyService, 
            WordingChangesConfig wordingChangesConfig, 
            IReQuoteRenewalEligibility reQuoteRenewalEligibility)
        {
            _generateRenewalOrder = generateRenewalOrder;
            _getRenewalDetails = getRenewalDetails;
            _updateAutoRenewalEligibility = updateAutoRenewalEligibility;
            _updateAutoRenewalOptInFlag = updateAutoRenewalOptInFlag;
            _submitRenewalOrder = submitRenewalOrder;
            _updateSpecialCircumstances = updateSpecialCircumstances;
            _renewalEligibility = renewalEligibility;
            _policyService = policyService;
            _wordingChangesConfig = wordingChangesConfig;
            _reQuoteRenewalEligibility = reQuoteRenewalEligibility;
            _updateEnableAllRenewalFlag = updateEnableAllRenewalFlag;
        }

        public override async Task<GenerateOrderResponse> GenerateOrder(GenerateOrderRequest request,
            ServerCallContext context)
        {
            var response = await _generateRenewalOrder.Generate(new Guid(request.ExpiringPolicyId), context.CancellationToken);

            return response.Success
                ? new GenerateOrderResponse() { Success = new SuccessDto()
                {
                    OrderId = response.Result.OrderId.ToString()
                }}
                : new GenerateOrderResponse() { Failed = new FailedDto() { Message = response.Result.FailedReason } };
        }
        
        public override async Task<SubmitRenewalOrderResponse> SubmitRenewalOrder(SubmitRenewalOrderRequest request,
            ServerCallContext context)
        {
            var response = await _submitRenewalOrder.Submit(new Guid(request.ExpiringPolicyId), new Guid(request.OrderId), context.CancellationToken);

            if (!response.Success)
            {
                return new SubmitRenewalOrderResponse()
                {
                    Failed = new FailedDto()
                    {
                        Message = response.Result.FailReason
                    }
                };
            }

            return new SubmitRenewalOrderResponse()
            {
                Success = new SuccessDto()
                {
                    OrderId = response.Result.OrderId.ToString()
                }
            };
        }

        public override async Task<IsEligibleForRenewalResponse> IsEligibleForRenewal(IsEligibleForRenewalRequest request,
            ServerCallContext context)
        {
            var policy = await _policyService.GetPolicy(request.ExpiringPolicyId);

            var result=   await _renewalEligibility.CheckEligibility(new Guid(request.ExpiringPolicyId), policy.PaymentFrequency,
                context.CancellationToken);

            return new IsEligibleForRenewalResponse
            {
                IsEligible = result.IsEligible,
                Reason = result.Reason
            };
        }

        public override async Task<IsEligibleForReQuoteRenewalResponse> IsEligibleForReQuoteRenewal(
            IsEligibleForReQuoteRenewalRequest request,
            ServerCallContext context)
        {
             var policy = await _policyService.GetPolicy(request.ExpiringPolicyId);

             var result = await _reQuoteRenewalEligibility.CheckEligibility(new Guid(request.ExpiringPolicyId),
                 policy.PaymentFrequency,
                 context.CancellationToken);

            return new IsEligibleForReQuoteRenewalResponse
            {
                IsEligible = result.IsEligible,
                Reason = result.Reason ?? string.Empty
            };
        }

        public override async Task<GetRenewalEligibilityDetailsResponse> GetRenewalEligibilityDetails(GetRenewalEligibilityDetailsRequest request,
            ServerCallContext context)
        {
            var policy = await _policyService.GetPolicy(request.ExpiringPolicyId);

            var result = await _renewalEligibility.CheckEligibility(new Guid(request.ExpiringPolicyId), policy.PaymentFrequency,
                context.CancellationToken);

            return new GetRenewalEligibilityDetailsResponse
            {
                IsEligible = result.IsEligible,
                Reason = result.Reason,
                Flags = new RenewalFlags
                {
                    HasAlreadyRenewed = result.HasAlreadyRenewed,
                    AutoRenewalOptIn = result.AutoRenewalOptIn
                }
            };
        }

        public override async Task<GetRenewalDetailsResponse> GetRenewalDetails(GetRenewalDetailsRequest request,
            ServerCallContext context)
        {
            var response = await _getRenewalDetails.Get(new Guid(request.OrderId), context.CancellationToken);

            return new GetRenewalDetailsResponse
            {
                ExpiringPolicyId = response.ExpiringPolicyId.ToString(),
                OrderId = response.OrderId.ToString()
            };
        }

        public override async Task<GetRenewalDetailsForExpiringPolicyResponse> GetRenewalDetailsForExpiringPolicy(
            GetRenewalDetailsForExpiringPolicyRequest request, ServerCallContext context)
        {
            var renewalDto = await _getRenewalDetails
                .GetRenewalDetailsForExpiringPolicy(new Guid(request.ExpiringPolicyId), context.CancellationToken);

            return renewalDto.ToGrpcResponse();
        }

        public override async Task<Empty> UpdateAutoRenewalEligibility(
            UpdateAutoRenewalEligibilityRequest request, ServerCallContext context)
        {
            await _updateAutoRenewalEligibility.Update(
                new Guid(request.ExpiringPolicyId), request.IsEligible, request.Comments, context.CancellationToken);

            return new Empty();
        }

        public override async Task<Empty> UpdateAutoRenewalOptInFlag(UpdateAutoRenewalOptInFlagRequest request, ServerCallContext context)
        {
            await _updateAutoRenewalOptInFlag.Update(Guid.Parse(request.ExpiringPolicyId), request.OptIn,
                context.CancellationToken);

            return new Empty();
        }

        public override async Task<Empty> UpdateEnableAllRenewalFlag(UpdateEnableAllRenewalFlagRequest request, ServerCallContext context)
        {
            await _updateEnableAllRenewalFlag.Update(Guid.Parse(request.ExpiringPolicyId), request.Enable,
                context.CancellationToken);

            return new Empty();
        }
      
        public override async Task<Empty> UpdateSpecialCircumstances(UpdateSpecialCircumstancesRequest request, ServerCallContext context)
        {
            ValidateGrpcRequest(request);

            await _updateSpecialCircumstances.Update(Guid.Parse(request.ExpiringPolicyId), request.IsApplied,
                request.Comments, request.Reason, request.SecondLevelReason, context.CancellationToken);

            return new Empty();
        }

        public override async Task<GetRenewalPolicyDetailsResponse> GetRenewalPolicyDetails(GetRenewalPolicyDetailsRequest request, ServerCallContext context)
        {
            var (expiringPolicy, renewedPolicy) = await _getRenewalDetails.GetRenewalPolicyDetails(new Guid(request.PolicyId), context.CancellationToken);

            return new GetRenewalPolicyDetailsResponse()
            {
                OldPolicy = expiringPolicy?.ToRenewalDto(),
                NewPolicy = renewedPolicy?.ToRenewalDto()
            };
        }

        public override async Task<GetWordingChangeUrlResponse> GetWordingChangeUrl(GetWordingChangeUrlRequest request, ServerCallContext context) =>
            await Task.FromResult(new GetWordingChangeUrlResponse { Url = _wordingChangesConfig.GetWordingConfigUrl(request.ProductCode, request.EffectiveDate.ToDateTime()) });

        private static void ValidateGrpcRequest(UpdateSpecialCircumstancesRequest request)
        {
            const string InsurerRequest = "insurer request";

            if (!request.IsApplied) return;

            var validReasons = new[] { InsurerRequest, "claim", "it bug" };
            if (validReasons.Contains(request.Reason.ToLower()) == false)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Argument"), $"Invalid Reason {request.Reason} valid reasons are {string.Join("'", validReasons) }") { };
            }

            if (request.Reason == InsurerRequest && string.IsNullOrWhiteSpace(request.SecondLevelReason))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "insurer request must have second level reason")) { };
            }
        }
    }
}

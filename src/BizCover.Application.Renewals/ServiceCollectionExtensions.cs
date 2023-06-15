using BizCover.Application.Renewals.Configuration;
using BizCover.Application.Renewals.Rules;
using BizCover.Application.Renewals.Rules.AutoEligibility;
using BizCover.Application.Renewals.Rules.Criteria;
using BizCover.Application.Renewals.Rules.ReQuoteEligibility;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.Application.Renewals.UseCases.GenerateRenewalOrder;
using BizCover.Application.Renewals.UseCases.RenewalDetails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService = BizCover.Application.Renewals.Services.OrderService;

namespace BizCover.Application.Renewals
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRenewalsApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<IOfferService, OfferService>()
                .AddScoped<ICartService, CartService>()
                .AddScoped<IPolicyService, PolicyService>()
                .AddScoped<IOrderService, OrderService>()
                .AddScoped<IQuotationService, QuotationService>()
                .AddScoped<IRenewalService, RenewalService>()
                .AddScoped<IRenewalConfigService, RenewalConfigService>()
                .AddScoped<IPaymentService, PaymentService>()
                .AddScoped<IRenewalEligibility, RenewalEligibility>()
                .AddScoped<IReQuoteRenewalEligibility, ReQuoteRenewalEligibility>();

            services
                .AddScoped<InitiateRenewal>()
                .AddScoped<GenerateRenewalOrder>()
                .AddScoped<ISubmitRenewalOrder, SubmitRenewalOrder>()
                .AddScoped<GetRenewalDetails>()
                .AddScoped<UpdateRenewalDetails>()
                .AddScoped<UpdateAutoRenewalEligibility>()
                .AddScoped<UpdateSpecialCircumstances>()
                .AddScoped<UpdateEnableAllRenewalFlag>()
                .AddScoped<IAddOrUpdatePolicyRenewalDetails, AddOrUpdatePolicyRenewalDetails>()
                .AddScoped<UpdateAutoRenewalOptInFlag>()
                .AddScoped<StartAutoRenewalOrderGeneration>()
                .AddScoped<StartAutoRenewalOrderSubmission>()
                .AddScoped<StartRenewalInitiation>()
                .AddScoped<UpdatePolicyStatus>()
                .AddScoped<PolicyBoundEventMigration>()
                .AddScoped<ISetPolicyAsRenewed, SetPolicyAsRenewed>()
                .AddScoped<PublishDueForRenewalEvents>()
                .AddScoped<IPublishSpecialCircumstancesUpdatedEvent, PublishSpecialCircumstancesUpdatedEvent>()
                .AddScoped<IPublishAutoRenewalOptInFlagUpdatedEvent, PublishAutoRenewalOptInFlagUpdatedEvent>();

            // Eligibility Validations Criteria
            services
                .AddScoped<IRenewalEligibilityCriteriaFactory, RenewalEligibilityCriteriaFactory>()
                .AddScoped<IAutoRenewalEligibilityService, AutoRenewalEligibilityService>()
                .AddScoped<IReQuoteRenewalEligibilityService, ReQuoteRenewalEligibilityService>()

                .AddScoped<IRenewalEligibilityCriteria, PolicyAlreadyRenewedCriteria>()
                .AddScoped<IRenewalEligibilityCriteria, PolicyAutoRenewalEligibilityCriteria>()
                .AddScoped<IRenewalEligibilityCriteria, PolicyCancelledCriteria>()
                .AddScoped<IRenewalEligibilityCriteria, PolicyIssuedCriteria>()
                .AddScoped<IRenewalEligibilityCriteria, PolicyInArrearsCriteria>()
                .AddScoped<IRenewalEligibilityCriteria, PolicyHasSpecialCircumstanceCriteria>()
                .AddScoped<IRenewalEligibilityCriteria, ProductAutoRenewalEligibilityCriteria>();

            
            services.AddSingleton(RenewalConfigHelper.GetConfig<CanAutoRenewProductConfig>("CanAutoRenewProductConfig.json"));
            services.AddSingleton(RenewalConfigHelper.GetConfig<RenewalStepTriggerDayConfig>("RenewalStepTriggerDayConfig.json"));
        }
    }
}

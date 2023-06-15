using BizCover.Application.Carts;
using BizCover.Application.Offers;
using BizCover.Application.Orders;
using BizCover.Application.Policies;
using BizCover.Application.Quotations;
using BizCover.Application.Renewals.Helpers;
using BizCover.Blaze.Infrastructure.Bus;
using BizCover.Blaze.Infrastructure.Bus.BlazeSNSConfigurator;
using BizCover.Blaze.MultiTenancy.GrpcInterceptors;
using BizCover.Consumer.Renewals;
using BizCover.Entity.Renewals;
using BizCover.Framework.Persistence.Mongo.Extensions;
using BizCover.Framework.Persistence.Mongo.Options;
using BizCover.Infrastructure.ServiceDiscovery;
using BizCover.Messages.Orders;
using BizCover.Messages.Policies;
using BizCover.Messages.Renewals;
using BizCover.Messages.Scheduler;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BizCover.Infrastructure.Renewals
{
    public static class ServiceCollectionExtensions
    {
        private const int VisibilityTimeout = 300; //5 minutes

        public static void AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddBlazeBus(configuration, subscriptions =>
                {
                    subscriptions.QueueAttributes["VisibilityTimeout"] = VisibilityTimeout;
                    subscriptions.Subscribe<PolicyBoundEvent>();
                    subscriptions.Subscribe<OrderCompletedEvent>();
                    subscriptions.Subscribe<StatusChange>();
                    subscriptions.Subscribe<PolicyStatusChangedEvent>();
                    subscriptions.Subscribe<AutoRenewalPendingPaymentCreatedEvent>();
                }, (configurator, registration) =>
                {
                    configurator.ConfigureConsumer<PolicyBoundEventConsumer>(registration);
                    configurator.ConfigureConsumer<PolicyStatusChangedEventConsumer>(registration);
                    configurator.ConfigureConsumer<MigratePolicyCommandConsumer>(registration);
                    configurator.ConfigureConsumer<SchedulerEventConsumer>(registration);
                    configurator.ConfigureConsumer<InitiateRenewalCommandConsumer>(registration);
                    configurator.ConfigureConsumer<GenerateAutoRenewalOrderCommandConsumer>(registration);
                    configurator.ConfigureConsumer<SubmitAutoRenewalOrderCommandConsumer>(registration);
                    configurator.ConfigureConsumer<OrderCompletedEventConsumer>(registration);
                    configurator.ConfigureConsumer<AutoRenewalPendingPaymentCreatedEventConsumer>(registration);
                },
                TopicWithoutConsumers,
                typeof(PolicyBoundEventConsumer),
                typeof(PolicyStatusChangedEventConsumer),
                typeof(MigratePolicyCommandConsumer),
                typeof(SchedulerEventConsumer),
                typeof(InitiateRenewalCommandConsumer),
                typeof(GenerateAutoRenewalOrderCommandConsumer),
                typeof(SubmitAutoRenewalOrderCommandConsumer),
                typeof(OrderCompletedEventConsumer),
                typeof(AutoRenewalPendingPaymentCreatedEventConsumer));
        }
        
        public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)));
            services.AddMongo(opt => opt.EntitiesFrom(typeof(Renewal)));
        }

        public static void RegisterInfra(this IServiceCollection services) =>
            services.AddScoped<IQueuePublisher, QueuePublisher>();

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add tenant context for multi-tenant behaviour
            services
                .AddBlazeTenantContext();
            services
                .AddScoped<TenantContextServerInterceptor>()
                .AddScoped<TenantContextClientInterceptor>();

            // Add grpc services
            services.AddGrpc(options =>
            {
                // Ensure tenant context is created from inbound metadata on grpc calls
                options.Interceptors.Add<TenantContextServerInterceptor>();
            });

            services
                .AddBizCoverServiceDiscovery(configuration)
                .AddGrpcClient<PoliciesService.PoliciesServiceClient>(ServiceNames.POLICIES)
                .AddGrpcClient<OffersService.OffersServiceClient>(ServiceNames.OFFERS)
                .AddGrpcClient<CartsService.CartsServiceClient>(ServiceNames.CARTS)
                .AddGrpcClient<OrderService.OrderServiceClient>(ServiceNames.ORDERS)
                .AddGrpcClient<gRPC.Payment.PaymentService.PaymentServiceClient>(ServiceNames.PAYMENT)
                .AddGrpcClient<QuotationsService.QuotationsServiceClient>(ServiceNames.QUOTATIONS);

            return services;
        }

        private static IServiceCollection AddGrpcClient<T>(this IServiceCollection services, string name) where T : class =>
        services
            .AddGrpcClient<T>((provider, options) =>
            {
                // discover the service endpoints
                var endpoints = provider
                    .GetRequiredService<IServiceEndpointFactory>()
                    .GetServiceEndpoint(name);
                // and use the GRPC address
                options.Address = endpoints.GrpcUri;
            })
            .ConfigureChannel(c => c.Credentials = ChannelCredentials.Insecure)
            // Ensure tenant context is propagated to outbound metadata on grpc calls
            .AddInterceptor<TenantContextClientInterceptor>(InterceptorScope.Client)
            .Services;

        private static void TopicWithoutConsumers(IBlazeSNSConfigurator configurator) 
        {
            configurator.ConfigureSNS<PolicyRenewedEvent>();
            configurator.ConfigureSNS<RenewalOrderGeneratedEvent>();
        }
    }
}

using BizCover.Blaze.Infrastructure.Bus.Sample.Consumer.Consumers;
using BizCover.Blaze.Infrastructure.Bus.Sample.Event;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using BizCover.Blaze.Infrastructure.Bus.Upgrade;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BizCover.Blaze.Infrastructure.Bus.Sample.Consumer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddHealthChecks();

            services.AddBlazeBus(Configuration,

                subscriptions => { subscriptions.Subscribe<OrderEvent>(); },

                (receiveEndpointConfigurator, registration) =>
                {
                    receiveEndpointConfigurator.ConfigureConsumer<OrderEventConsumer>(registration);
                    receiveEndpointConfigurator.ConfigureConsumer<AcceptOrderCommandConsumer>(registration, 
                        // overwrite default message retry with 5 time retrieval
                        cfg => 
                            cfg.UseMessageRetry(r => r.Intervals(500, 1000, 1500, 2000, 2500)));
                },
                null,

                new Type[]
                {
                    typeof(OrderEventConsumer),
                    typeof(AcceptOrderCommandConsumer)
                });

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = (check) => check.Tags.Contains("blazebus");
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("blazebus"),
                });

                endpoints.MapControllers();
            });
        }
    }
}

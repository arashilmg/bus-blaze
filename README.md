# BizCover.Blaze.Infrastructure.Bus

A BizCover Library for MassTransit

## Usage

In Startup.cs
* For adding a bus if the solution has both Consumers and Publisher
```cs
     services.AddBlazeBus(Configuration,

                subscriptions =>
                {
                    subscriptions.Subscribe<OrderEvent>();
                },

                (IReceiveEndpointConfigurator receiveEndpointConfigurator, IRegistration registration) =>
                {
                    receiveEndpointConfigurator.ConfigureConsumer<OrderEventConsumer>(registration);
                },

                new Type[]
                {
                    typeof(OrderEventConsumer)
                });
```
* If solution has only publisher
```cs
    services.AddBlazeBus(Configuration, null, null);
```

* Add the following to your appsettings.json while teting with localstack.
```json
 "AWS": {
    "Region": "ap-southeast-2",
    "ServiceURL": "http://localhost:4566",
  },
  "BLAZE_REGION": "au",
  "BLAZE_ENVIRONMENT": "dev",
  "BLAZE_SERVICE": "consumer" // used in queue name only
```
* Do not add `ServiceURL` in your appsettings.json while deloying to actual environment.
```json
 "AWS": {
    "Region": "ap-southeast-2",
  },
  "BLAZE_REGION": "au",
  "BLAZE_ENVIRONMENT": "dev",
  "BLAZE_SERVICE": "consumer" // used in queue name only
```
### Send Command
This library already injects a resolver called `IResolverEndpoint`. It has two methods
 - Uri ResolveDefaultEndpoint(string serviceName)
 - Uri ResolveDefaultEndpointForSelf()
 if consumer wants to supply a servicename (queuename) use `ResolveDefaultEndpoint` it automatically forms a uri like `amazonsqs://ap-southeast-2/au-dev-blaze-{serviceName}` based on `BLAZE_REGION` , `BLAZE_ENVIRONMENT` and string `blaze`
 if consumer wants to get the serivce QueueName please use `ResolveDefaultEndpointForSelf` method and this method automatically forms a uri like `amazonsqs://ap-southeast-2/au-dev-blaze-consumer` based on `BLAZE_REGION` , `BLAZE_ENVIRONMENT` , string `blaze` and `BLAZE_SERVICE`

To consumer `IResolverEndpoint` inject this to constructor and call the desired method to get the endpoint URI.
 
### TopicName Convention
if message namespace starts with `BizCover.Messages.` eg: `BizCover.Messages.Order` the topic name will be picked as `BLAZE_ENVIRONMET-BLAZE_REGION-blaze-order-orderevent`
if message namespace does not starts with `BizCover.Messages.` eg: `samples.Order` then topic name will be `BLAZE_ENVIRONMET-BLAZE_REGION-blaze-orderevent`
Fault topic will have `-fault` at the end.

### QueueName Convention
QueueName will be `BLAZE_ENVIRONMET-BLAZE_REGION-blaze-BLAZE_SERVICE`
Error queue will have `_error` at the end
Skipped queue will have `_skip` at the end

### HealthCheck
Add following in `Startup.ConfigureService` method

services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = (check) => check.Tags.Contains("blazebus");
            });

and in `Startup.Configure` method

app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("blazebus"),
                });
            });
### To run sample application
 - make Multiple startup in this solution
  - BizCover.Blaze.Infrastructure.Bus.Sample.Consumer
  - BizCover.Blaze.Infrastructure.Bus.Sample.Publisher
 - in `Consumer` launchsettings.json 
  -  "applicationUrl": "https://localhost:5002;http://localhost:5003"
 - in `Publisher` launchsetings.json
  - "applicationUrl": "https://localhost:5001;http://localhost:5000"
 - To publish message - POST method
  - http://localhost:5000/BusPublisher/publish
   - body as 
   `{
          "ProductName":"abce",
          "OrderId":"123"
    }`
- to check consumer - GET method
 - http://localhost:5003/busreceiver/get/123 
 
**Note: Make sure no Environment Variable `AWS_PROFILE` is added in local machine. If its already there do remove and restart your VisualStudio.**
 

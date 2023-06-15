using BizCover.Api.Renewals;
using BizCover.Application.Renewals;
using BizCover.Application.Renewals.UseCases.RenewalDetails;
using BizCover.Application.Renewals.UseCases.WordingChanges;
using BizCover.Framework.HealthChecks;
using BizCover.Infrastructure.Renewals;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

// Allow grpc to operate in a non-TLS environment
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

var builder = WebApplication.CreateBuilder(args);

if (Environment.GetEnvironmentVariable("ENVIRONMENT") != "Local")
{
    builder.Host.ConfigureLogging(loggingBuilder => loggingBuilder.AddJsonConsole(options =>
    {
        options.IncludeScopes = false;
        options.TimestampFormat = "hh:mm:ss.fff";
    }));
}
else
{
    builder.Host.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole(options => options.FormatterName = "Systemd" ));
}
builder.Services.AddGrpc();
builder.Services.AddInfrastructure(builder.Configuration);            

builder.Services.AddWordingChangesConfig();
builder.Services.AddRenewalsApplication(builder.Configuration);

builder.Services.AddEventBus(builder.Configuration);
builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(2);
    options.Predicate = check => check.Tags.Contains(HealthCheckTags.Bus);
});

builder.Services.AddMongoDb(builder.Configuration);

builder.Services
    .AddHealthChecks()
    .AddRepositoryHealthCheck(builder.Configuration);

builder.WebHost.UseKestrel(opt =>
{
    var (httpPort, grpcPort) = GetConfiguredPorts();
    // Operate one port in HTTP/1.1 mode for k8s health-checks etc
    opt.Listen(IPAddress.Any, httpPort, listen => listen.Protocols = HttpProtocols.Http1);
    // Operate one port in HTTP/2 mode for GRPC
    opt.Listen(IPAddress.Any, grpcPort, listen => listen.Protocols = HttpProtocols.Http2);
});

builder.Services.RegisterInfra();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcDiagnosticsService>();
app.MapGrpcService<GrpcRenewalsService>();
app.MapGet("/", () => "Renewal Service up and running");

app.MapGet("/renewals/policy/{id}", async (Guid id, GetRenewalDetails getRenewalDetails, CancellationToken cancellation) => 
    await getRenewalDetails.GetRenewalDetailsForExpiringPolicy(id, cancellation));

app.MapGet("/renewals/updateallrenewalsflagtotrue", async (UpdateRenewalDetails updateRenewalDetails, CancellationToken cancellation) =>
    await updateRenewalDetails.UpdateAllRenewalsFlagToTrueFromNull(cancellation));


app.UseHealthChecks(alivePredicate: check => false, readyPredicate: check => check.Tags.Contains(HealthCheckTags.Bus) ||
                                                                             check.Tags.Contains(HealthCheckTags.Database));
app.AddMongoDbIndexes();
app.Run();

static (int httpPort, int grpcPort) GetConfiguredPorts()
{
    var httpPort = int.Parse(Environment.GetEnvironmentVariable("HTTP_PORT") ?? "5000");
    var grpcPort = int.Parse(Environment.GetEnvironmentVariable("GRPC_PORT") ?? "5001");
    return (httpPort, grpcPort);
}

// to make it accessible in integration test
public partial class Program { }
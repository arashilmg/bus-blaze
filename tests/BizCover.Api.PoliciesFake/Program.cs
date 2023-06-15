using System.Net;
using BizCover.Api.PolicyFake.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.WebHost.UseKestrel(opt =>
{
    var (httpPort, grpcPort) = GetConfiguredPorts();
    opt.Listen(IPAddress.Any, httpPort, listen => listen.Protocols = HttpProtocols.Http1);
    opt.Listen(IPAddress.Any, grpcPort, listen => listen.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();
app.MapGrpcService<GrpcPolicyService>();
app.MapGet("/", () => "Fake");
app.Run();

static (int httpPort, int grpcPort) GetConfiguredPorts()
{
    var httpPort = int.Parse(Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8100");
    var grpcPort = int.Parse(Environment.GetEnvironmentVariable("GRPC_PORT") ?? "8101");
    return (httpPort, grpcPort);
}
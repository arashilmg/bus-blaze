using BizCover.Application.Diagnostics;
using Grpc.Core;

namespace BizCover.Api.Renewals
{
    public class GrpcDiagnosticsService: DiagnosticsService.DiagnosticsServiceBase
    {
        public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context) =>
            Task.FromResult(new PingResponse());
    }
}

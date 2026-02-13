namespace ServiceC.Services;

using Grpc.Core;
using Shared.Protos;

public class ServiceCGrpcService : ServiceC.ServiceCBase
{
    public override async Task<FinalizeResponse> FinalizeTransaction(
        FinalizeRequest request,
        ServerCallContext context)
    {
        await Task.Delay(500);
        
        var overallSuccess = request.ServiceA && request.ServiceB;

        var response = new FinalizeResponse
        {
            Success = overallSuccess,
            ProcessedAt = DateTime.UtcNow.ToString("O")
        };

        return response;
    }
}

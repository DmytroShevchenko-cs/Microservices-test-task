namespace ServiceC.Services;

using Grpc.Core;
using Shared.Protos;

public class ServiceCGrpcService(ILogger<ServiceCGrpcService> logger) : ServiceC.ServiceCBase
{
    public override async Task<FinalizeResponse> FinalizeTransaction(
        FinalizeRequest request,
        ServerCallContext context)
    {
        var overallSuccess = request.ServiceA && request.ServiceB;

        var response = new FinalizeResponse
        {
            Success = overallSuccess,
            ProcessedAt = DateTime.UtcNow.ToString("O")
        };

        logger.LogInformation("{RequestTransactionId} finished", request.TransactionId);
        
        return response;
    }
}

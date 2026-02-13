namespace ServiceC;

using Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateApplicationBuilder(args);
        
        builder.Services.AddGrpc();
        
        var app = builder.Build();

        app.MapGrpcService<ServiceCGrpcService>();

        app.MapGet("/", () => "gRPC server is running");
        
        await app.RunAsync();
    }

    private static WebApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        return builder;
    }
}
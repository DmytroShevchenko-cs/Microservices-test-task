namespace ServiceC;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateApplicationBuilder(args);
        
        builder.Services.AddGrpc();
        
        var app = builder.Build();

        //app.MapGrpcService<object>();

        app.MapGet("/", () => "gRPC server is running");
        
        await app.StartAsync();
    }

    private static WebApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        return builder;
    }
}
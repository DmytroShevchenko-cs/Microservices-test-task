namespace ServiceA;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateApplicationBuilder(args);
        
        var app = builder.Build();

        await app.StartAsync();
    }

    private static WebApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        return builder;
    }
}
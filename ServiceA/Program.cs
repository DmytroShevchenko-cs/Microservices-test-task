namespace ServiceA;

using Consumers;
using MassTransit;
using Microsoft.Extensions.Options;
using Services.TransactionStorage;
using Shared.Configurations;
using Shared.Consts;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateApplicationBuilder(args);
        
        builder.Services.AddControllers();
        
        builder.Services.AddSingleton<TransactionStorage>();
        
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
        builder.Services.Configure<GrpcSettings>(builder.Configuration.GetSection(nameof(GrpcSettings)));
        
        builder.Services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<ServiceAProcessConsumer>();
            cfg.AddConsumer<ServiceBProcessConsumer>();
            
            cfg.UsingRabbitMq((context, bus) =>
            {
                var settings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                bus.Host(settings.HostName, settings.Port, "/", h =>
                {
                    h.Username(settings.UserName);
                    h.Password(settings.Password);
                });

                bus.ReceiveEndpoint(RabbitConsts.MessageQueue.ServiceAQueue, e =>
                {
                    e.ConfigureConsumer<ServiceAProcessConsumer>(context);
                });
                
                bus.ReceiveEndpoint(RabbitConsts.MessageQueue.ServiceBQueue, e =>
                {
                    e.ConfigureConsumer<ServiceBProcessConsumer>(context);
                });
                
                bus.UseMessageRetry(r => r.Intervals(
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(5),
                    TimeSpan.FromMinutes(15)));
                
                bus.ConfigureEndpoints(context);
            });
        });
        
        builder.Services.AddGrpcClient<Shared.Protos.ServiceC.ServiceCClient>((sp, options) =>
        {
            var grpc = sp.GetRequiredService<IOptions<GrpcSettings>>().Value;
            options.Address = new Uri(grpc.ServiceCUrl);
        });
        
        var app = builder.Build();
        
        app.MapControllers();

        await app.RunAsync();
    }

    private static WebApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        return builder;
    }
}
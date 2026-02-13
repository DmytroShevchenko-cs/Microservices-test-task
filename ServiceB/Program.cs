namespace ServiceB;

using Consumers;
using MassTransit;
using Microsoft.Extensions.Options;
using Shared.Configurations;
using Shared.Consts;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateApplicationBuilder(args);
        
        builder.Services.AddControllers();
        
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
        
        builder.Services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<ProcessInServiceBConsumer>();
            
            cfg.UsingRabbitMq((context, bus) =>
            {
                var settings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                bus.Host(settings.HostName, settings.Port, "/", h =>
                {
                    h.Username(settings.UserName);
                    h.Password(settings.Password);
                });

                bus.ReceiveEndpoint(RabbitConsts.MessageQueue.ServiceBQueue, e =>
                {
                    e.ConfigureConsumer<ProcessInServiceBConsumer>(context);
                });
                
                bus.ConfigureEndpoints(context);
            });
        });
        
        var app = builder.Build();
        
        await app.RunAsync();
    }

    private static WebApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        return builder;
    }
}
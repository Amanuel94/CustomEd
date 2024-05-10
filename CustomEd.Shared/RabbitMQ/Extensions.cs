

using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using CustomEd.Shared.Settings;
using System; 

namespace CustomEd.Shared.RabbitMQ;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, string queueName)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.AddConsumers(Assembly.GetEntryAssembly());
            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                var rabbitMQSettings = context.GetRequiredService<IConfiguration>().GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                // var queueName = Assembly.GetEntryAssembly().GetName().Name;
                // var queueName = Guid.NewGuid().ToString();
                Console.WriteLine($"Queue Name: {queueName}");
                configurator.Host(rabbitMQSettings.Host);
                // configurator.ConfigureEndpoints(context);
                configurator.ReceiveEndpoint(queueName, endpointConfigurator =>
                {
                    endpointConfigurator.ConfigureConsumers(context);
                });

                configurator.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                });
            });
        });

        return services;
    }
}


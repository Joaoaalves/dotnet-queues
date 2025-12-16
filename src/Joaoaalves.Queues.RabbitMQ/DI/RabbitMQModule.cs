using Joaoaalves.Queues.Abstractions.Processing;
using Joaoaalves.Queues.Abstractions.Workers;
using Joaoaalves.Queues.RabbitMQ.Configuration;
using Joaoaalves.Queues.RabbitMQ.Internal;
using Joaoaalves.Queues.RabbitMQ.Processing;
using Joaoaalves.Queues.RabbitMQ.Workers;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Joaoaalves.Queues.RabbitMQ.DI;

public static class RabbitMqModule
{
    public static IServiceCollection AddRabbitMqQueues(
        this IServiceCollection services,
        Action<RabbitMqOptions> configure)
    {
        // Options
        var options = new RabbitMqOptions();
        configure(options);
        services.AddSingleton(options);

        // Connection factory
        services.AddSingleton(_ =>
            new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            });

        // (async lazy singleton)
        services.AddSingleton<RabbitMqConnectionHolder>();

        // Dispatcher
        services.AddSingleton<IJobDispatcher, RabbitMqJobDispatcher>();

        // Worker
        services.AddSingleton<IJobWorker, RabbitMqWorker>();

        return services;
    }
}
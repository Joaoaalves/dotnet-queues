using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.Abstractions.Processing;
using Joaoaalves.Queues.Abstractions.Workers;
using Joaoaalves.Queues.Processing;
using Joaoaalves.Queues.Store;
using Joaoaalves.Queues.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Joaoaalves.Queues.DI
{
    public static class QueuesModule
    {
        public static IServiceCollection AddQueues(this IServiceCollection services, Action<QueueOptions>? configure = null)
        {
            var options = new QueueOptions();
            configure?.Invoke(options);

            // Core services
            services.AddSingleton<JobProcessor>();
            services.AddSingleton<JobChainBuilder>();

            services.AddSingleton<IHostedService, QueueHostedService>();

            // register common abstractions to avoid missing registrations
            services.TryAddTransient<IJobDispatcher, DefaultJobDispatcher>();
            services.TryAddSingleton<IJobStore, InMemoryJobStore>(); // default fallback

            // discovery
            RegisterHandlersAndWorkers(services, Assembly.GetEntryAssembly()!, Assembly.GetCallingAssembly());

            return services;
        }

        private static void RegisterHandlersAndWorkers(IServiceCollection services, params Assembly[] assemblies)
        {
            // Register all IJobHandler implementations
            var handlerTypes = assemblies.SelectMany(a => a.ExportedTypes)
                .Where(t => !t.IsAbstract && typeof(IJobHandler).IsAssignableFrom(t));

            foreach (var t in handlerTypes)
                services.AddTransient(t);

            // Register all IJobWorker implementations
            var workerTypes = assemblies.SelectMany(a => a.ExportedTypes)
                .Where(t => !t.IsAbstract && typeof(IJobWorker).IsAssignableFrom(t));

            foreach (var t in workerTypes)
                services.AddSingleton(typeof(IJobWorker), t);
        }
    }

    public class QueueOptions
    {
        public TimeSpan DefaultPollingInterval { get; set; } = TimeSpan.FromSeconds(2);
    }
}

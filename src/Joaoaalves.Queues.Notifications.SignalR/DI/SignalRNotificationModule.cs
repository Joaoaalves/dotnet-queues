using Joaoaalves.Queues.Abstractions.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Joaoaalves.Queues.Notifications.SignalR.DI
{
    public static class SignalRNotificationModule
    {
        public static IServiceCollection AddSignalRNotificationModule(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<IJobProgressNotifier, Notifications.SignalRNotificationService>();
            return services;
        }
    }
}
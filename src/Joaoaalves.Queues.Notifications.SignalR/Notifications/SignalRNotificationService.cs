using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.Abstractions.Notifications;
using Joaoaalves.Queues.Notifications.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Joaoaalves.Queues.Notifications.SignalR.Notifications
{
    public sealed class SignalRNotificationService(
        IHubContext<JobStatusHub> hub
    ) : IJobProgressNotifier
    {
        private readonly IHubContext<JobStatusHub> _hub = hub;
        public Task NotifyAsync(IJob job, Progress progress, CancellationToken cancellationToken = default)
        {
            return _hub.Clients.All.SendAsync(
                job.Status.ToString(),
                new
                {
                    job.Id,
                    type = job.GetType(),
                    progress
                }, cancellationToken);
        }
    }
}
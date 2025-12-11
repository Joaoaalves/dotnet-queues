using Joaoaalves.Queues.Abstractions.Jobs;

namespace Joaoaalves.Queues.Abstractions.Notifications
{
    /// <summary>
    /// Abstraction to notify progress to front-end or other systems.
    /// Implementations may use SignalR, SSE, Webhooks, PubSub, etc.
    /// </summary>
    public interface IJobProgressNotifier
    {
        Task NotifyAsync(IJob job, Progress progress, CancellationToken cancellationToken = default);
    }
}
namespace Joaoaalves.Queues.Abstractions.Notifications
{
    public interface ICallbackNotificationService
    {
        Task NotifyCallbackAsync(string url, object payload, CancellationToken cancellationToken = default);
    }
}
namespace Joaoaalves.Queues.Abstractions.Notifications
{
    public interface IJobNotificationService
    {
        Task PublishJobStartedAsync(Guid jobId);
        Task PublishJobCompletedAsync(Guid jobId, bool success);
    }
}

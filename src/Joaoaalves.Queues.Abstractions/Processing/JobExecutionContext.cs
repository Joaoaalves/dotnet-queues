using Joaoaalves.Queues.Abstractions.DI;
using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.Abstractions.Notifications;

namespace Joaoaalves.Queues.Abstractions.Processing
{
    public sealed class JobExecutionContext(
        IJob job,
        IQueueServiceScope scope,
        IJobStore jobStore,
        IJobProgressNotifier progressNotifier,
        IJobNotificationService notificationService)
    {
        public IJob Job { get; } = job;
        public IQueueServiceScope Scope { get; } = scope;
        public IQueueServiceProvider Services => Scope.ServiceProvider;

        public IDictionary<string, object> Bag { get; } = new Dictionary<string, object>();

        public IJobStore JobStore { get; } = jobStore;
        public IJobProgressNotifier ProgressNotifier { get; } = progressNotifier;
        public IJobNotificationService NotificationService { get; } = notificationService;

        public T? Get<T>(string key) =>
            Bag.TryGetValue(key, out var value) ? (T)value : default;

        public void Set<T>(string key, T value) =>
            Bag[key] = value!;
    }

}
using Joaoaalves.Queues.Abstractions.DI;
using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.Abstractions.Notifications;
using Joaoaalves.Queues.Abstractions.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Joaoaalves.Queues.Processing
{
    public sealed class JobProcessor(
        IQueueServiceProvider provider,
        IQueueServiceScope serviceScope,
        JobChainBuilder chainBuilder,
        ILogger<JobProcessor>? logger,
        IJobStore store,
        IJobProgressNotifier notifier,
        IJobNotificationService notificationService
    )
    {
        private readonly IQueueServiceProvider _provider = provider;
        private readonly IQueueServiceScope _queueServiceScope = serviceScope;
        private readonly JobChainBuilder _chainBuilder = chainBuilder;
        private readonly ILogger<JobProcessor>? _logger = logger;
        private readonly IJobStore _store = store;
        private readonly IJobProgressNotifier _notifier = notifier;
        private readonly IJobNotificationService _notificationService = notificationService;

        public async Task ProcessAsync(IJob job, CancellationToken ct = default)
        {
            var ctx = new JobExecutionContext(job, _queueServiceScope, _store, _notifier, _notificationService);
            job.Run();

            await _store.UpdateAsync(job, ct);

            var pipeline = _chainBuilder.BuildChain();

            try
            {
                await pipeline(ctx);
                job.Complete();
                await _store.UpdateAsync(job, ct);

                await _notifier.NotifyAsync(job, new Progress(100, "Completed"), ct);
            }
            catch (OperationCanceledException)
            {
                job.Cancel("Operation was cancelled.");
                await _store.UpdateAsync(job, ct);
            }
            catch (Exception ex)
            {
                job.Fail(ex.Message);
                await _store.UpdateAsync(job, ct);

                _logger?.LogError(ex, "Job {JobId} failed with an exception.", job.Id);
            }
        }
    }
}
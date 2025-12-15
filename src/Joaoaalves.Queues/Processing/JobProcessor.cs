using Joaoaalves.Queues.Abstractions.DI;
using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.Abstractions.Notifications;
using Joaoaalves.Queues.Abstractions.Processing;
using Microsoft.Extensions.Logging;

namespace Joaoaalves.Queues.Processing
{
    public sealed class JobProcessor(
        IQueueServiceScopeFactory scopeFactory,
        JobChainBuilder chainBuilder,
        ILogger<JobProcessor>? logger
    )
    {
        private readonly IQueueServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly JobChainBuilder _chainBuilder = chainBuilder;
        private readonly ILogger<JobProcessor>? _logger = logger;

        public async Task ProcessAsync(IJob job, CancellationToken ct = default)
        {
            var scope = _scopeFactory.CreateScope();
            var store = scope.ServiceProvider.GetService<IJobStore>();
            var notifier = scope.ServiceProvider.GetService<IJobProgressNotifier>();

            var ctx = new JobExecutionContext(job, scope, store,
                notifier,
                scope.ServiceProvider.GetService<IJobNotificationService>()
            );

            job.Run();

            await store.UpdateAsync(job, ct);

            var pipeline = _chainBuilder.BuildChain();

            try
            {
                await pipeline(ctx);
                job.Complete();
                await store.UpdateAsync(job, ct);

                await notifier.NotifyAsync(job, new Progress(100, "Completed"), ct);
            }
            catch (OperationCanceledException)
            {
                job.Cancel("Operation was cancelled.");
                await store.UpdateAsync(job, ct);
            }
            catch (Exception ex)
            {
                job.Fail(ex.Message);
                await store.UpdateAsync(job, ct);

                _logger?.LogError(ex, "Job {JobId} failed with an exception.", job.Id);
            }
        }
    }
}
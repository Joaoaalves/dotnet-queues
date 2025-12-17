using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.Abstractions.Workers;
using Joaoaalves.Queues.DI;
using Joaoaalves.Queues.Processing;

namespace Joaoaalves.Queues.Workers
{
    /// <summary>
    /// Simple polling loop for a worker.
    /// </summary>
    public sealed class WorkerRuntime(IJobStore store, JobProcessor processor, IServiceProvider services, QueueOptions options) : IJobWorker
    {
        private readonly IJobStore _store = store;
        private readonly JobProcessor _processor = processor;
        private readonly IServiceProvider _services = services;
        private CancellationTokenSource? _cts;
        private Task? _executingTask;
        private readonly TimeSpan _pollInterval = options.DefaultPollingInterval;

        public IEnumerable<string> SupportedJobTypes => [];

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = Task.Run(() => RunAsync(_cts.Token), CancellationToken.None);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_cts == null) return;

            _cts.Cancel();
            await (_executingTask ?? Task.CompletedTask);
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var pending = await _store.FetchPendingAsync(5, cancellationToken);
                foreach (var job in pending)
                {
                    try
                    {
                        await _processor.ProcessAsync(job, cancellationToken);
                    }
                    catch
                    {
                        // JobProcessor already handles failures.
                    }
                }

                await Task.Delay(_pollInterval, cancellationToken);
            }
        }

        public async ValueTask DisposeAsync()
        {
            _cts?.Dispose();
        }
    }
}
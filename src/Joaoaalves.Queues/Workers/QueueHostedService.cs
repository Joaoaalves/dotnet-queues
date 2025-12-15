using Joaoaalves.Queues.Abstractions.Workers;
using Microsoft.Extensions.Hosting;

namespace Joaoaalves.Queues.Workers
{
    public class QueueHostedService(IEnumerable<IJobWorker> workers) : IHostedService
    {
        private readonly IEnumerable<IJobWorker> _workers = workers;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var w in _workers)
                await w.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var w in _workers)
                await w.StopAsync(cancellationToken);
        }
    }
}

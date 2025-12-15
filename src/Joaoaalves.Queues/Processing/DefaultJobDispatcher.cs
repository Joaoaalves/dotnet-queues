using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.Abstractions.Processing;

namespace Joaoaalves.Queues.Processing
{
    public sealed class DefaultJobDispatcher(
        IJobStore store
    ) : IJobDispatcher
    {
        private readonly IJobStore _store = store;
        public Task EnqueueAsync(IJob job, CancellationToken cancellationToken = default)
        {
            return _store.StoreAsync(job, cancellationToken);
        }
    }
}
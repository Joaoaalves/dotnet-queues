using System.Collections.Concurrent;
using Joaoaalves.Queues.Abstractions.Jobs;

namespace Joaoaalves.Queues.Store
{
    public class InMemoryJobStore : IJobStore
    {
        private readonly ConcurrentDictionary<Guid, IJob> _store = new();

        public Task StoreAsync(IJob job, CancellationToken cancellationToken = default)
        {
            _store[job.Id] = job;
            return Task.CompletedTask;
        }

        public Task<IJob?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _store.TryGetValue(id, out var job);
            return Task.FromResult(job);
        }

        public Task<IEnumerable<IJob>> FetchPendingAsync(int maxItems, CancellationToken cancellationToken = default)
        {
            var items = _store.Values.Where(j => j.Status == JobStatus.Pending)
                                     .Take(maxItems)
                                     .ToList()
                                     .AsEnumerable();
            return Task.FromResult(items);
        }

        public Task UpdateAsync(IJob job, CancellationToken cancellationToken = default)
        {
            _store[job.Id] = job;
            return Task.CompletedTask;
        }

        public Task MoveToDeadLetterAsync(IJob job, string reason, CancellationToken cancellationToken = default)
        {
            job.Status = JobStatus.DeadLetter;
            _store[job.Id] = job;
            return Task.CompletedTask;
        }
    }
}

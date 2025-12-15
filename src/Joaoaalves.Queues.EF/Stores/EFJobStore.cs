using System.Text.Json;
using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace Joaoaalves.Queues.EF.Stores
{
    public sealed class EfJobStore(
        DbContext db
    ) : IJobStore
    {
        private readonly DbContext _db = db;

        public async Task StoreAsync(IJob job, CancellationToken cancellationToken = default)
        {
            var entity = new QueueJob
            {
                Id = job.Id,
                Type = job.Type,
                PayloadJson = JsonSerializer.Serialize(job.Payload),
                Status = job.Status.ToString(),
                CreatedAt = job.CreatedAt,
                LastUpdatedAt = job.LastUpdatedAt
            };

            _db.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IJob?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var e = await _db.Set<QueueJob>().FindAsync([id], cancellationToken);
            if (e == null) return null;
            return new EfBackedJob(e);
        }

        public async Task<IEnumerable<IJob>> FetchPendingAsync(int maxItems, CancellationToken cancellationToken = default)
        {
            var items = await _db.Set<QueueJob>()
                                 .Where(x => x.Status == nameof(JobStatus.Pending))
                                 .OrderBy(x => x.CreatedAt)
                                 .Take(maxItems)
                                 .ToListAsync(cancellationToken);

            return items.Select(e => new EfBackedJob(e)).ToList();
        }

        public async Task UpdateAsync(IJob job, CancellationToken cancellationToken = default)
        {
            var e = await _db.Set<QueueJob>().FindAsync([job.Id], cancellationToken)
                ?? throw new InvalidOperationException("Job not found");

            e.Status = job.Status.ToString();
            e.LastUpdatedAt = job.LastUpdatedAt;
            e.PayloadJson = JsonSerializer.Serialize(job.Payload);
            _db.Update(e);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task MoveToDeadLetterAsync(IJob job, string reason, CancellationToken cancellationToken = default)
        {
            var e = await _db.Set<QueueJob>().FindAsync(new object[] { job.Id }, cancellationToken);
            if (e == null) throw new InvalidOperationException("Job not found");
            e.Status = nameof(JobStatus.DeadLetter);
            e.LastUpdatedAt = DateTime.UtcNow;
            _db.Update(e);
            await _db.SaveChangesAsync(cancellationToken);
        }

        private class EfBackedJob(QueueJob e) : IJob
        {
            public Guid Id => e.Id;
            public string Type => e.Type;
            public object? Payload => e.PayloadJson;
            public JobStatus Status
            {
                get => Enum.Parse<JobStatus>(e.Status);
                set => e.Status = value.ToString();
            }
            public DateTime CreatedAt => e.CreatedAt;
            public DateTime? LastUpdatedAt { get => e.LastUpdatedAt; set => e.LastUpdatedAt = value; }

            public void Cancel(string reason)
            {
                LastUpdatedAt = DateTime.UtcNow;
                Status = JobStatus.Cancelled;
            }

            public void Complete()
            {
                LastUpdatedAt = DateTime.UtcNow;
                Status = JobStatus.Succeeded;
            }

            public void Fail(string reason)
            {
                LastUpdatedAt = DateTime.UtcNow;
                Status = JobStatus.Failed;
            }

            public void Run()
            {
                LastUpdatedAt = DateTime.UtcNow;
                Status = JobStatus.Running;
            }

            public string ToJson()
            {
                return JsonSerializer.Serialize(e);
            }
        }
    }
}
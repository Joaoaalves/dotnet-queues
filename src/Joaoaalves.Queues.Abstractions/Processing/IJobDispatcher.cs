using Joaoaalves.Queues.Abstractions.Jobs;

namespace Joaoaalves.Queues.Abstractions.Processing
{
    /// <summary>
    /// Abstracts the act of enqueuing/publishing a job.
    /// </summary>
    public interface IJobDispatcher
    {
        Task EnqueueAsync(IJob job, CancellationToken cancellationToken = default);
    }
}
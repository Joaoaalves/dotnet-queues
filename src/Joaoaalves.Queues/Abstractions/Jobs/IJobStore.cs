namespace Joaoaalves.Queues.Abstractions.Jobs
{
    ///<summary>
    /// Persistence abstraction for jobs.
    /// Implementations can use EF, Redis, Mongo, etc
    /// </summary>
    public interface IJobStore
    {
        Task StoreAsync(IJob job, CancellationToken cancellationToken = default);
        Task<IJob?> FindAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<IJob>> FetchPendingAsync(int maxItems, CancellationToken cancellationToken = default);
        Task UpdateAsync(IJob job, CancellationToken cancellationToken = default);
        Task MoveToDeadLetterAsync(IJob job, string reason, CancellationToken cancellationToken = default);
    }
}
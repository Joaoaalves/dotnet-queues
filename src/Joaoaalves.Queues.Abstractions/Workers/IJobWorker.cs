using Microsoft.Extensions.Hosting;

namespace Joaoaalves.Queues.Abstractions.Workers
{
    /// <summary>
    /// A long running component that fetches jobs and processes them.
    /// </summary>
    public interface IJobWorker : IHostedService, IDisposable
    {
        /// <summary>
        /// The worker can declare wich job types it can process.
        /// </summary>
        IEnumerable<string> SupportedJobTypes { get; }
    }
}
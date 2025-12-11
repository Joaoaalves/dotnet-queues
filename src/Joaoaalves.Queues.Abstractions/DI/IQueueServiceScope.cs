namespace Joaoaalves.Queues.Abstractions.DI
{
    public interface IQueueServiceScope : IDisposable
    {
        IQueueServiceProvider ServiceProvider { get; }
    }
}
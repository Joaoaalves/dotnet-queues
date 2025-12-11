namespace Joaoaalves.Queues.Abstractions.DI
{
    public interface IQueueServiceScopeFactory
    {
        IQueueServiceScope CreateScope();
    }
}
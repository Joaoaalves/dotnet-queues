namespace Joaoaalves.Queues.Abstractions.DI
{
    public interface IQueueServiceProvider
    {
        object GetService(Type serviceType);
        T GetService<T>();
        IEnumerable<T> GetServices<T>();
    }
}
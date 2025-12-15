using Joaoaalves.Queues.Abstractions.DI;
using Microsoft.Extensions.DependencyInjection;

namespace Joaoaalves.Queues.DI
{
    public sealed class ServiceProvider(
        IServiceProvider provider
    ) : IQueueServiceProvider
    {
        private readonly IServiceProvider _provider = provider;
        public object GetService(Type serviceType) => _provider.GetService(serviceType)
            ?? throw new InvalidOperationException($"No service for type '{serviceType}' has been registered");

        public T GetService<T>()
        {
            var service = _provider.GetService(typeof(T))
                ?? throw new InvalidOperationException($"No service for type '{typeof(T)}' has been registered");

            return (T)service;
        }

        public IEnumerable<T> GetServices<T>()
        {
            return _provider.GetServices<T>() ?? [];
        }
    }
}
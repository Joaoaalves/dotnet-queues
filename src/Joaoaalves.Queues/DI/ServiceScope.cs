using Joaoaalves.Queues.Abstractions.DI;
using Microsoft.Extensions.DependencyInjection;

namespace Joaoaalves.Queues.DI
{
    public sealed class ServiceScope(
        IServiceScope scope
    ) : IQueueServiceScope
    {
        private readonly IServiceScope _scope = scope;
        public IQueueServiceProvider ServiceProvider => new ServiceProvider(_scope.ServiceProvider);
        public void Dispose() => _scope.Dispose();
    }
}
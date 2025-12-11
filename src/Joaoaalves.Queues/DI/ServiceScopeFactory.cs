using Joaoaalves.Queues.Abstractions.DI;
using Microsoft.Extensions.DependencyInjection;

namespace Joaoaalves.Queues.DI
{
    public sealed class ServiceScopeFactory(
        IServiceScopeFactory factory
    ) : IQueueServiceScopeFactory
    {
        private readonly IServiceScopeFactory _factory = factory;
        public IQueueServiceScope CreateScope()
        {
            var scope = _factory.CreateScope();
            return new ServiceScope(scope);
        }
    }
}
using Joaoaalves.Queues.Abstractions.DI;
using Joaoaalves.Queues.Abstractions.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace Joaoaalves.Queues.Processing
{
    /// <summary>
    /// Builds a chain of IJobHandler from all registered handlers in the container.
    /// The order can be controlled by an optional attribute or interface priority in future.
    /// </summary>
    public sealed class JobChainBuilder(
        IQueueServiceProvider provider
    )
    {
        private readonly IQueueServiceProvider _provider = provider;

        public Func<JobExecutionContext, Task> BuildChain()
        {
            var handlers = _provider.GetServices<IJobHandler>().ToArray();

            Func<JobExecutionContext, Task> pipeline = ctx => Task.CompletedTask;

            // Build chain in reverse order
            for (int i = handlers.Length - 1; i >= 0; i--)
            {
                var handler = handlers[i];
                var next = pipeline;
                pipeline = ctx => handler.HandleAsync(ctx, () => next(ctx));
            }

            return pipeline;
        }
    }
}

namespace Joaoaalves.Queues.Abstractions.Processing
{
    public abstract class JobHandlerBase : IJobHandler
    {
        public Task HandleAsync(JobExecutionContext context, Func<Task> next)
        {
            // Default behaviour
            return next();
        }
    }
}
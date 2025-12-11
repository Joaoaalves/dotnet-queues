namespace Joaoaalves.Queues.Abstractions.Processing
{
    public interface IJobHandler
    {
        Task HandleAsync(JobExecutionContext context, Func<Task> next);
    }
}
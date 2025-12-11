namespace Joaoaalves.Queues.Abstractions.Jobs
{
    public enum JobStatus
    {
        Pending,
        Running,
        Succeeded,
        Failed,
        Cancelled,
        DeadLetter
    }
}
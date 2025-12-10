namespace Joaoaalves.Queues.Abstractions.Jobs
{
    /// <summary>
    /// Minimal job contract.
    /// </summary>
    public interface IJob
    {
        Guid Id { get; }
        string Type { get; }
        object? Payload { get; }
        JobStatus Status { get; set; }
        DateTime CreatedAt { get; }
        DateTime? LastUpdatedAt { get; set; }

        public string ToJson();
    }
}
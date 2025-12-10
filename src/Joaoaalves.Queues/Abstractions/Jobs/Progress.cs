namespace Joaoaalves.Queues.Abstractions.Jobs
{
    public record Progress(int Percentage, string? Message = null);
}
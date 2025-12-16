namespace Joaoaalves.Queues.RabbitMQ.Configuration
{
    public sealed class RabbitMqOptions
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";

        public string ExchangeName { get; set; } = "queues.jobs";
        public string QueueName { get; set; } = "queues.jobs.default";
        public string RoutingKey { get; set; } = "job";

        public ushort PrefetchCount { get; set; } = 5;
    }
}
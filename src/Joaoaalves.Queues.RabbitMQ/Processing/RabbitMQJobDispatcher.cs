using System.Text.Json;
using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.Abstractions.Processing;
using Joaoaalves.Queues.RabbitMQ.Configuration;
using RabbitMQ.Client;

namespace Joaoaalves.Queues.RabbitMQ.Processing
{
    public sealed class RabbitMqJobDispatcher(IChannel channel, RabbitMqOptions options) : IJobDispatcher
    {
        private readonly IChannel _channel = channel;
        private readonly RabbitMqOptions _options = options;

        public async Task EnqueueAsync(IJob job, CancellationToken cancellationToken = default)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(new
            {
                job.Id,
                job.Type
            });

            var props = new BasicProperties
            {
                Persistent = true
            };

            await _channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: _options.RoutingKey,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}
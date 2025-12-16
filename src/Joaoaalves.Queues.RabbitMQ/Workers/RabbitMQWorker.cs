using System.Text.Json;
using Joaoaalves.Queues.Abstractions.DI;
using Joaoaalves.Queues.Abstractions.Jobs;
using Joaoaalves.Queues.Abstractions.Workers;
using Joaoaalves.Queues.Processing;
using Joaoaalves.Queues.RabbitMQ.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Joaoaalves.Queues.RabbitMQ.Workers
{

    public class RabbitMqWorker(
        IConnection connection,
        IJobStore jobStore,
        JobProcessor processor,
        IQueueServiceScopeFactory scopeFactory,
        RabbitMqOptions options) : IJobWorker
    {
        private readonly IConnection _connection = connection;
        private readonly IJobStore _jobStore = jobStore;
        private readonly JobProcessor _processor = processor;
        private readonly IQueueServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly RabbitMqOptions _options = options;

        private IChannel? _channel;
        private AsyncEventingBasicConsumer? _consumer;

        public IEnumerable<string> SupportedJobTypes { get; set; } = [];

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: _options.PrefetchCount,
                global: false,
                cancellationToken: cancellationToken);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += OnMessageAsync;

            await _channel.BasicConsumeAsync(
                queue: _options.QueueName,
                autoAck: false,
                consumer: _consumer,
                cancellationToken: cancellationToken);
        }

        private async Task OnMessageAsync(object sender, BasicDeliverEventArgs args)
        {
            if (_channel is null)
                return;

            try
            {
                using var json = JsonDocument.Parse(args.Body.ToArray());
                var jobId = json.RootElement.GetProperty("Id").GetGuid();

                var job = await _jobStore.FindAsync(jobId);
                if (job is null)
                {
                    await _channel.BasicAckAsync(args.DeliveryTag, false);
                    return;
                }

                var scope = _scopeFactory.CreateScope();
                await _processor.ProcessAsync(job);

                await _channel.BasicAckAsync(args.DeliveryTag, false);
            }
            catch
            {
                if (_channel is not null)
                {
                    await _channel.BasicNackAsync(
                        args.DeliveryTag,
                        multiple: false,
                        requeue: false);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel is not null)
            {
                await _channel.CloseAsync(cancellationToken);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
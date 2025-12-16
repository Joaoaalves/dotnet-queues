using RabbitMQ.Client;

namespace Joaoaalves.Queues.RabbitMQ.Internal
{

    public sealed class RabbitMqConnectionHolder(ConnectionFactory factory) : IAsyncDisposable
    {
        private readonly ConnectionFactory _factory = factory;
        private readonly SemaphoreSlim _lock = new(1, 1);

        private IConnection? _connection;

        public async Task<IConnection> GetAsync(CancellationToken cancellationToken = default)
        {
            if (_connection is not null)
                return _connection;

            await _lock.WaitAsync(cancellationToken);
            try
            {
                _connection ??= await _factory.CreateConnectionAsync(cancellationToken);

                return _connection;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.DisposeAsync();
            }
        }
    }
}
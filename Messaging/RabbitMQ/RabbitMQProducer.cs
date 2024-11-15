using Messaging.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace Messaging.RabbitMQ
{
    public class RabbitMQProducer : IMessageProducer, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private bool _disposed = false;

        private RabbitMQProducer(IConnection connection, IChannel channel)
        {
            _connection = connection;
            _channel = channel;
        }

        public static async Task<RabbitMQProducer> CreateAsync(string hostName = "localhost")
        {
            var factory = new ConnectionFactory { HostName = hostName };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            return new RabbitMQProducer(connection, channel);
        }

        public async Task SendMessageAsync<T>(string exchange, string routingKey, T message)
        {
            if (!_disposed)
            {
                // Declare the exchange if it doesn't exist
                await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, durable: true);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    Persistent = true,
                };

                await _channel.BasicPublishAsync(
                    exchange: exchange,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: default);

                Console.WriteLine($"Sent message to exchange: {exchange}, routing key: {routingKey}");
            }
            else
            {
                throw new ObjectDisposedException(nameof(RabbitMQProducer));
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
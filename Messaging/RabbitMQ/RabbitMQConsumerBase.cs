using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Messaging.RabbitMQ
{
    public abstract class RabbitMQConsumerBase<T>(string exchangeName,
                                                  string routingKey,
                                                  string queueName,
                                                  string hostName = "localhost") : BackgroundService, IDisposable
    {
        private readonly string _exchangeName = exchangeName;
        private readonly string _routingKey = routingKey;
        private readonly string _queueName = queueName;
        private readonly string _hostName = hostName;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _disposed = false;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = _hostName };
            _connection = await factory.CreateConnectionAsync(cancellationToken: stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, durable: true, cancellationToken: stoppingToken);
            await _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
            await _channel.QueueBindAsync(_queueName, _exchangeName, _routingKey, cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(0, 1, false, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deserializedMessage = JsonSerializer.Deserialize<T>(message);

                try
                {
                    await ProcessMessageAsync(deserializedMessage);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true, cancellationToken: stoppingToken);
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            await _channel.BasicConsumeAsync(_queueName, false, consumer, cancellationToken: stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public abstract Task ProcessMessageAsync(T? message);

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _channel?.CloseAsync().GetAwaiter().GetResult();
                    _connection?.CloseAsync().GetAwaiter().GetResult();
                }

                _channel?.Dispose();
                _connection?.Dispose();

                _disposed = true;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RabbitMQConsumerBase()
        {
            Dispose(false);
        }
    }
}
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Service.Models;
using System.Text;
using System.Text.Json;

namespace Product.Service.Consumers
{
    public class OrderConsumer : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync(cancellationToken: stoppingToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(queue: "orders",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null, 
                                 cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                //var message = Encoding.UTF8.GetString(body);
                using var stream = new MemoryStream(body);
                var order = await JsonSerializer.DeserializeAsync<Order>(stream);

                Console.WriteLine($"Order received: ProductId={order?.ProductId}, Quantity={order?.Quantity}");
                await Task.Delay(100); // شبیه‌سازی پردازش async
            };

            await channel.BasicConsumeAsync(queue: "orders",
                                 autoAck: true,
                                 consumer: consumer,
                                 cancellationToken: stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

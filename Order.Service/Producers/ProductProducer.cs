using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace Order.Service.Producers
{
    public class ProductProducer : IMessageProducer
    {
        public async void SendMessage<T>(T message)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "orders",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: "",
                                 routingKey: "orders",
                                 body: body);
        }
    }
}

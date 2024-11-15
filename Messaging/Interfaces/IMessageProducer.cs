namespace Messaging.Interfaces
{
    public interface IMessageProducer
    {
        Task SendMessageAsync<T>(string exchange, string routingKey, T message);
    }
}

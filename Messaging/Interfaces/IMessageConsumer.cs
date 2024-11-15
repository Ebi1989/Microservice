namespace Messaging.Interfaces
{
    public interface IMessageConsumer<T>
    {
        Task ProcessMessageAsync(T message);
    }
}

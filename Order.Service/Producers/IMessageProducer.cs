namespace Order.Service.Producers
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message);
    }
}

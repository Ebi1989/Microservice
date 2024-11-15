using Messaging.RabbitMQ;

namespace Product.Service.Consumers
{
    public class OrderConsumer : RabbitMQConsumerBase<OrderMessage>
    {
        public OrderConsumer() : base("order" , "order.created", "orders")
        {
        }

        public override async Task ProcessMessageAsync(OrderMessage? message)
        {
            Console.WriteLine($"Processing order: {message?.OrderId}");
            await Task.Delay(100); 
        }
    }

    public class OrderMessage
    {
        public int OrderId { get; set; }
    }
    
}

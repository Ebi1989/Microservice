using Messaging.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Order.Service.DTOs;
using Shared.Protos;

namespace Order.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(ProductGrpc.ProductGrpcClient productClient, IMessageProducer messageProducer) : ControllerBase
    {
        private readonly ProductGrpc.ProductGrpcClient _productClient = productClient;

        private readonly IMessageProducer _messageProducer = messageProducer;

        [HttpGet]
        public IActionResult Get() => Ok("Orders from OrderService");

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var productResponse = await _productClient.GetProductAsync(new GetProductRequest { Id = request.ProductId });

            var order = new
            {
                ProductId = request.ProductId,
                ProductName = productResponse.Name,
                Quantity = request.Quantity,
                TotalPrice = productResponse.Price * request.Quantity
            };

            await _messageProducer.SendMessageAsync("order", "order.created", order);

            return Ok(order);
        }
    }
}

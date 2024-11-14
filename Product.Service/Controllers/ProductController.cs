using Microsoft.AspNetCore.Mvc;

namespace Product.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(ILogger<ProductController> logger) : ControllerBase
    {

        private readonly ILogger<ProductController> _logger = logger;

        [HttpGet]
        public IActionResult Get() => Ok("Products from ProductService");
    }
}

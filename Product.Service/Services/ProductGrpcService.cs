namespace Product.Service.Services;
using Shared.Protos;
public class ProductService : ProductGrpc.ProductGrpcBase
{
    public override Task<ProductResponse> GetProduct(GetProductRequest request, Grpc.Core.ServerCallContext context)
    {
        return Task.FromResult(new ProductResponse
        {
            Id = request.Id,
            Name = "Sample Product",
            Price = 9.99
        });
    }
}

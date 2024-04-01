using MediatR;

namespace Application.Features.Products.Commands.DeleteProduct
{
    public sealed record DeleteProductCommand(int Id) : IRequest<bool>;
}

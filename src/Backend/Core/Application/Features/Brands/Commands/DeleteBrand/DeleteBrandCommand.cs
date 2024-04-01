using MediatR;

namespace Application.Features.Brands.Commands.DeleteBrand
{
    public sealed record class DeleteBrandCommand(int Id) : IRequest<bool>;
}

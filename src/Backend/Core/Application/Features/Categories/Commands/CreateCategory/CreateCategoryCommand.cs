using MediatR;
using Shared;

namespace Application.Features.Categories.Commands.CreateCategory
{
    public sealed record CreateCategoryCommand(string Name, string CreatedBy, string CreatedUserId) : IRequest<Result<CreatedCategoryDto>>;
}

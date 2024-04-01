using MediatR;

namespace Application.Features.CategoriesSub.Commands.DeleteCategorySub
{
    public sealed record DeleteCategorySubCommand(int Id) : IRequest<bool>;
}

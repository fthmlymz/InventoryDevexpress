using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetCompanyAllList();
        Task<List<Company>> GetCompanyById(int id);
    }
}

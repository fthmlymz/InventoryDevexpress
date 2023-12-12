using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Repositories
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetCompanyAllList();
        Task<List<Company>> GetCompanyById(int id);
    }
}

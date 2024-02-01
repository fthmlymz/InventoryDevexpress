using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence.Repositories
{
    internal class CompanyRepository : ICompanyRepository
    {
        private readonly IGenericRepository<Company> _repository;

        public CompanyRepository(IGenericRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task<List<Company>> GetCompanyAllList()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<Company>> GetCompanyById(int id)
        {
            return await _repository.Entities.Where(x => x.Id == id).ToListAsync();
        }
    }
}

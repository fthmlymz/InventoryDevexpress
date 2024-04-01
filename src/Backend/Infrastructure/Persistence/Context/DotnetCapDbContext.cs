using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence.Context
{
    public class DotnetCapDbContext : DbContext
    {

        public DotnetCapDbContext(DbContextOptions<DotnetCapDbContext> options) : base(options)
        {

        }
    }
}

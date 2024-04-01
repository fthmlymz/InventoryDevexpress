using Microsoft.EntityFrameworkCore;

namespace Persistence.Context
{
    public class DotnetCapDbContext : DbContext
    {

        public DotnetCapDbContext(DbContextOptions<DotnetCapDbContext> options) : base(options)
        {

        }
    }
}

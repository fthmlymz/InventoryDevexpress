using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IConfiguration _configuration;

        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventDispatcher dispatcher, IConfiguration configuration) : base(options)
        {
            _dispatcher = dispatcher;
            _configuration = configuration;
        }



        public DbSet<Company> Companies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategorySub> CategorySubs { get; set; }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AssignedProduct> AssignedProducts { get; set; }
        public DbSet<ProductMovement> ProductMovements { get; set; }
        public DbSet<TransferOfficier> TransferOfficiers { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SqlServerConnection"));
            base.OnConfiguring(optionsBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseAuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseAuditableEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    ((BaseAuditableEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;
                }
            }


            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            // ignore events if no dispatcher provided
            if (_dispatcher == null) return result;

            // dispatch events only if save was successful
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

            await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}



//public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
//{
//    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

//    // ignore events if no dispatcher provided
//    if (_dispatcher == null) return result;

//    // dispatch events only if save was successful
//    var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
//        .Select(e => e.Entity)
//        .Where(e => e.DomainEvents.Any())
//        .ToArray();

//    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

//    return result;
//}

//public override int SaveChanges()
//{
//    //#region Automatic Date
//    //foreach (var item in ChangeTracker.Entries())
//    //{

//    //    if (item.Entity is BaseAuditableEntity entityReference)
//    //    {
//    //        switch (item.State)
//    //        {
//    //            case EntityState.Added:
//    //                {
//    //                    Entry(entityReference).Property(x => x.UpdatedDate).IsModified = false;
//    //                    entityReference.CreatedDate = DateTime.UtcNow;
//    //                    break;
//    //                }
//    //            case EntityState.Modified:
//    //                {
//    //                    Entry(entityReference).Property(x => x.CreatedDate).IsModified = false;
//    //                    entityReference.UpdatedDate = DateTime.Now;
//    //                    break;
//    //                }
//    //        }
//    //    }
//    //}
//    //#endregion

//    return SaveChangesAsync().GetAwaiter().GetResult();
//}

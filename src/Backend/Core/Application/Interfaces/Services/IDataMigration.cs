﻿namespace InventoryManagement.Application.Interfaces.Services
{
    public interface IDataMigration
    {
        Task MigrateAsync();

        Task SeedAsync();
    }
}

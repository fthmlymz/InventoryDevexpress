using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Infrastructure.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}

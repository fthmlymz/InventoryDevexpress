using Microsoft.EntityFrameworkCore;
using Shared;

namespace Application.Extensions
{
    public static class QueryableExtensions
    {

        //Best performance
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken) where T : class
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            bool anyRecords = await source.AnyAsync();

            if (!anyRecords)
            {
                return PaginatedResult<T>.Create(new List<T>(), 0, pageNumber, pageSize);
            }
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return PaginatedResult<T>.Create(items, items.Count, pageNumber, pageSize);
        }

    }
}

/*
 * Original code
public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken) where T : class
{
    pageNumber = pageNumber == 0 ? 1 : pageNumber;
    pageSize = pageSize == 0 ? 10 : pageSize;
    int count = await source.CountAsync();
    pageNumber = pageNumber <= 0 ? 1 : pageNumber;
    List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    return PaginatedResult<T>.Create(items, count, pageNumber, pageSize);
}*/
namespace InventoryManagement.Frontend.Common
{
    public class PaginatedResult<T>
    {
        public List<T>? data { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public int totalCount { get; set; }
        public int pageSize { get; set; }
        public bool hasPreviousPage { get; set; }
        public bool hasNextPage { get; set; }
        public object? messages { get; set; }
        public bool succeeded { get; set; }
        public object? exception { get; set; }
        public int code { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace InventoryManagement.Frontend.Common
{
    public class PaginatedResult<T>
    {
        [JsonPropertyName("data")]
        public List<T>? data { get; set; }


        [JsonPropertyName("currentPage")]
        public int currentPage { get; set; }


        [JsonPropertyName("totalPages")]
        public int totalPages { get; set; }


        [JsonPropertyName("totalCount")]
        public int totalCount { get; set; }


        [JsonPropertyName("pageSize")]
        public int pageSize { get; set; }


        [JsonPropertyName("hasPreviousPage")]
        public bool hasPreviousPage { get; set; }


        [JsonPropertyName("hasNextPage")]
        public bool hasNextPage { get; set; }


        [JsonPropertyName("messages")]
        public object? messages { get; set; }


        [JsonPropertyName("succedded")]
        public bool succeeded { get; set; }


        [JsonPropertyName("exception")]
        public object? exception { get; set; }


        [JsonPropertyName("code")]
        public int code { get; set; }
    }
}

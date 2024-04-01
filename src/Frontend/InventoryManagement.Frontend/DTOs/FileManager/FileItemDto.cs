using System.Text.Json.Serialization;

namespace InventoryManagement.Frontend.DTOs.FileManager
{
    public class FileItemDto
    {
        [JsonPropertyName("fileName")]
        public string ? FileName { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("folderName")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string FolderName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonPropertyName("lastModifiedTime")]
        public DateTime LastModifiedTime { get; set; }

        [JsonPropertyName("fileType")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string FileType { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}

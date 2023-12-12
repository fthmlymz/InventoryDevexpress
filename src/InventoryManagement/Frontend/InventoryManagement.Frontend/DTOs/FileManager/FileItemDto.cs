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
        public string FolderName { get; set; }

        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonPropertyName("lastModifiedTime")]
        public DateTime LastModifiedTime { get; set; }

        [JsonPropertyName("fileType")]
        public string FileType { get; set; }
    }
}

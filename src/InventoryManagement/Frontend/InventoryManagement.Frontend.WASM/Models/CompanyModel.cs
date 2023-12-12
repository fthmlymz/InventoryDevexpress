using System.Text.Json;

namespace InventoryManagement.Frontend.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? CreatedBy { get; set; } = string.Empty;
        public string? CreatedUserId { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; } = string.Empty;
        public string? UpdatedUserId { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }




        public CompanyModel Clone()
        {
            var clone = new CompanyModel();
            clone.SetPropertiesFromJson(this);
            return clone;
        }

        public void SetPropertiesFromJson(JsonElement json)
        {
            Id = json.GetProperty("id").GetInt32();
            Name = json.GetProperty("name").GetString() ?? string.Empty; // null ise boş string ata
            Description = json.GetProperty("description").GetString() ?? string.Empty; // null ise boş string ata
            CreatedBy = json.GetProperty("createdBy").GetString() ?? string.Empty; // null ise boş string ata
            CreatedUserId = json.GetProperty("createdUserId").GetString() ?? string.Empty; // null ise boş string ata
            CreatedDate = json.GetProperty("createdDate").GetDateTime();
            UpdatedBy = json.GetProperty("updatedBy").GetString() ?? string.Empty; // null ise boş string ata
            UpdatedUserId = json.GetProperty("updatedUserId").GetString() ?? string.Empty; // null ise boş string ata
            UpdatedDate = json.GetProperty("updatedDate").GetDateTime(); // null ise varsayılan DateTime değeri ata
        }
        public void SetPropertiesFromJson(CompanyModel jsonData)
        {
            Id = jsonData.Id;
            Name = jsonData.Name ?? string.Empty;
            Description = jsonData.Description ?? string.Empty;
            CreatedBy = jsonData.CreatedBy ?? string.Empty;
            CreatedUserId = jsonData.CreatedUserId ?? string.Empty;
            CreatedDate = jsonData.CreatedDate;
            UpdatedBy = jsonData.UpdatedBy ?? string.Empty;
            UpdatedUserId = jsonData.UpdatedUserId ?? string.Empty;
            UpdatedDate = jsonData.UpdatedDate;
        }
    }
}

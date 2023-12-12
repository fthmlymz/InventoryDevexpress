using System.Text.Json;

namespace InventoryManagement.Frontend.DTOs.Company
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
            Name = json.GetProperty("name").GetString() ?? string.Empty;
            Description = json.GetProperty("description").GetString() ?? string.Empty;
            CreatedBy = json.GetProperty("createdBy").GetString() ?? string.Empty; 
            CreatedUserId = json.GetProperty("createdUserId").GetString() ?? string.Empty;
            CreatedDate = json.GetProperty("createdDate").GetDateTime();
            UpdatedBy = json.GetProperty("updatedBy").GetString() ?? string.Empty; 
            UpdatedUserId = json.GetProperty("updatedUserId").GetString() ?? string.Empty;
            UpdatedDate = json.GetProperty("updatedDate").GetDateTime();
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

using InventoryManagement.Frontend.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InventoryManagement.Frontend.DTOs.Brand
{
    public class BrandModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("companyId")]
        public int CompanyId { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdUserId")]
        public string CreatedUserId { get; set; }


        [JsonPropertyName("createdDate")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonPropertyName("updatedUserId")]
        public string UpdatedUserId { get; set; }

        [JsonPropertyName("updatedDate")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdatedDate { get; set; }

        [JsonPropertyName("models")]
        public List<Model> Models { get; set; }


        public BrandModel()
        {
            Models = new List<Model>();
        }
        public BrandModel Clone()
        {
            var clone = new BrandModel();
            clone.SetPropertiesFromJson(this);
            return clone;
        }
        public void SetPropertiesFromJson(JsonElement json)
        {
            Id = json.GetProperty("id").GetInt32();
            Name = json.GetProperty("name").GetString() ?? string.Empty;
            CompanyId = json.GetProperty("companyId").GetInt32();
            CreatedBy = json.GetProperty("createdBy").GetString() ?? string.Empty;
            CreatedUserId = json.GetProperty("createdUserId").GetString() ?? string.Empty;
            CreatedDate = json.GetProperty("createdDate").GetDateTime();
            UpdatedBy = json.GetProperty("updatedBy").GetString() ?? string.Empty;
            UpdatedUserId = json.GetProperty("updatedUserId").GetString() ?? string.Empty;
            UpdatedDate = json.GetProperty("updatedDate").GetDateTime();

            if (json.TryGetProperty("brands", out var brandsProperty) && brandsProperty.TryGetProperty("models", out var modelProperty))
            {
                Models = new List<Model>();
                foreach (var modelJson in modelProperty.EnumerateArray())
                {
                    var model = new Model();
                    model.SetPropertiesFromJson(modelJson);
                    Models.Add(model);
                }
            }
        }
        public void SetPropertiesFromJson(BrandModel jsonData)
        {
            Id = jsonData.Id;
            Name = jsonData.Name;
            CompanyId = jsonData.CompanyId;
            CreatedBy = jsonData.CreatedBy;
            CreatedUserId = jsonData.CreatedUserId;
            CreatedDate = jsonData.CreatedDate;
            UpdatedBy = jsonData.UpdatedBy;
            UpdatedUserId = jsonData.UpdatedUserId;
            Models = jsonData.Models;
        }
    }




    public class Model
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("brandId")]
        public int BrandId { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdUserId")]
        public string CreatedUserId { get; set; }


        [JsonPropertyName("createdDate")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonPropertyName("updatedUserId")]
        public string UpdatedUserId { get; set; }

        [JsonPropertyName("updatedDate")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdatedDate { get; set; }



        public Model Clone()
        {
            var clone = new Model();
            clone.SetPropertiesFromJson(this);
            return clone;
        }


        public void SetPropertiesFromJson(JsonElement json)
        {
            Id = json.GetProperty("id").GetInt32();
            Name = json.GetProperty("name").GetString() ?? string.Empty;
            BrandId = json.GetProperty("brandId").GetInt32();
            CreatedBy = json.GetProperty("createdBy").GetString() ?? string.Empty;
            CreatedUserId = json.GetProperty("createdUserId").GetString() ?? string.Empty;
            CreatedDate = json.GetProperty("createdDate").GetDateTime();
            UpdatedBy = json.GetProperty("updatedBy").GetString() ?? string.Empty;
            UpdatedUserId = json.GetProperty("updatedUserId").GetString() ?? string.Empty;
            UpdatedDate = json.GetProperty("updatedDate").GetDateTime();
        }
        public void SetPropertiesFromJson(Model jsonData)
        {
            Id = jsonData.Id;
            Name = jsonData.Name;
            BrandId = jsonData.BrandId;
            CreatedBy = jsonData.CreatedBy;
            CreatedUserId = jsonData.CreatedUserId;
            CreatedDate = jsonData.CreatedDate;
            UpdatedBy = jsonData.UpdatedBy;
            UpdatedUserId = jsonData.UpdatedUserId;
            UpdatedDate = jsonData.UpdatedDate;
        }
    }
}

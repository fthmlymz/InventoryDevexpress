using System.Text.Json;
using System.Text.Json.Serialization;
using static InventoryManagement.Frontend.Models.BrandModel;

namespace InventoryManagement.Frontend.Models
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
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonPropertyName("updatedUserId")]
        public string UpdatedUserId { get; set; }

        [JsonPropertyName("updatedDate")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
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
        public class JsonDateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String && reader.TryGetDateTime(out var dateTime))
                {
                    return dateTime;
                }
                return DateTime.MinValue; // veya varsayılan bir değer döndürebilirsiniz
            }
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }
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
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonPropertyName("updatedUserId")]
        public string UpdatedUserId { get; set; }

        [JsonPropertyName("updatedDate")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
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

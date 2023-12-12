using System.Text.Json;
using System.Text.Json.Serialization;
using static InventoryManagement.Frontend.Models.CategoryModel;

namespace InventoryManagement.Frontend.Models
{
    public class CategoryModel
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

        [JsonPropertyName("categorySubs")]
        public List<CategorySubModel> CategorySubs { get; set; }


        public CategoryModel()
        {
            CategorySubs = new List<CategorySubModel>();
        }
        public CategoryModel Clone()
        {
            var clone = new CategoryModel();
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
            //buraya categorySubs listesi eklenecek

            //if (json.TryGetProperty("categorySubs", out var categorySubsProperty))
            //{
            //    CategorySubs = new List<CategorySubModel>();
            //    foreach (var categorySubJson in categorySubsProperty.EnumerateArray())
            //    {
            //        var categorySub = new CategorySubModel();
            //        categorySub.SetPropertiesFromJson(categorySubJson);
            //        CategorySubs.Add(categorySub);
            //    }
            //}
            if (json.TryGetProperty("categories", out var categoriesProperty) && categoriesProperty.TryGetProperty("categorySubs", out var categorySubsProperty))
            {
                CategorySubs = new List<CategorySubModel>();
                foreach (var categorySubJson in categorySubsProperty.EnumerateArray())
                {
                    var categorySub = new CategorySubModel();
                    categorySub.SetPropertiesFromJson(categorySubJson);
                    CategorySubs.Add(categorySub);
                }
            }
        }
        public void SetPropertiesFromJson(CategoryModel jsonData)
        {
            Id = jsonData.Id;
            Name = jsonData.Name;
            CompanyId = jsonData.CompanyId;
            CreatedBy = jsonData.CreatedBy;
            CreatedUserId = jsonData.CreatedUserId;
            CreatedDate = jsonData.CreatedDate;
            UpdatedBy = jsonData.UpdatedBy;
            UpdatedUserId = jsonData.UpdatedUserId;
            CategorySubs = jsonData.CategorySubs;
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




    public class CategorySubModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

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



        public CategorySubModel Clone()
        {
            var clone = new CategorySubModel();
            clone.SetPropertiesFromJson(this);
            return clone;
        }


        public void SetPropertiesFromJson(JsonElement json)
        {
            Id = json.GetProperty("id").GetInt32();
            Name = json.GetProperty("name").GetString() ?? string.Empty;
            CategoryId = json.GetProperty("categoryId").GetInt32();
            CreatedBy = json.GetProperty("createdBy").GetString() ?? string.Empty;
            CreatedUserId = json.GetProperty("createdUserId").GetString() ?? string.Empty;
            CreatedDate = json.GetProperty("createdDate").GetDateTime();
            UpdatedBy = json.GetProperty("updatedBy").GetString() ?? string.Empty;
            UpdatedUserId = json.GetProperty("updatedUserId").GetString() ?? string.Empty;
            UpdatedDate = json.GetProperty("updatedDate").GetDateTime();
        }
        public void SetPropertiesFromJson(CategorySubModel jsonData)
        {
            Id = jsonData.Id;
            Name = jsonData.Name;
            CategoryId = jsonData.CategoryId;
            CreatedBy = jsonData.CreatedBy;
            CreatedUserId = jsonData.CreatedUserId;
            CreatedDate = jsonData.CreatedDate;
            UpdatedBy = jsonData.UpdatedBy;
            UpdatedUserId = jsonData.UpdatedUserId;
            UpdatedDate = jsonData.UpdatedDate;
        }
    }
}

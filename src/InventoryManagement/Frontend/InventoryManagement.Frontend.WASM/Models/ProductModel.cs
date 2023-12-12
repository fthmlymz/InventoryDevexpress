using System.Text.Json;
using System.Text.Json.Serialization;

namespace InventoryManagement.Frontend.Models
{
    public class ProductModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; } = string.Empty;

        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; } = string.Empty;

        [JsonPropertyName("imei")]
        public string Imei { get; set; } = string.Empty;

        [JsonPropertyName("mac")]
        public string Mac { get; set; } = string.Empty;

        [JsonPropertyName("dataClass")]
        public string? DataClass { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }


        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }

        [JsonPropertyName("categorySubId")]
        public int? CategorySubId { get; set; }


        [JsonPropertyName("brandId")]
        public int? BrandId { get; set; }

        [JsonPropertyName("modelId")]
        public int? ModelId { get; set; }




        [JsonPropertyName("productDate")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? ProductDate { get; set; }

        [JsonPropertyName("invoiceDate")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? InvoiceDate { get; set; }


        [JsonPropertyName("companyId")]
        public int CompanyId { get; set; }


        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; } = string.Empty;

        [JsonPropertyName("createdUserId")]
        public string? CreatedUserId { get; set; } = string.Empty;

        [JsonPropertyName("updatedBy")]
        public string? UpdatedBy { get; set; } = string.Empty;


        [JsonPropertyName("updatedUserId")]
        public string? UpdatedUserId { get; set; } = string.Empty;

        [JsonPropertyName("createdDate")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("updatedDate")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? UpdatedDate { get; set; }




        public ProductModel Clone()
        {
            var clone = new ProductModel();
            clone.SetPropertiesFromJson(this);
            return clone; // Değişiklik: return clone olarak değiştirildi
        }

        public void SetPropertiesFromJson(JsonElement json)
        {
            Id = json.GetProperty("id").GetInt32();
            Name = json.GetProperty("name").GetString() ?? string.Empty;
            Barcode = json.GetProperty("barcode").GetString() ?? string.Empty;
            SerialNumber = json.GetProperty("serialNumber").GetString() ?? string.Empty;
            Imei = json.GetProperty("imei").GetString() ?? string.Empty;
            Mac = json.GetProperty("mac").GetString() ?? string.Empty;
            DataClass = json.GetProperty("dataClass").GetString() ?? string.Empty;
            Status = json.GetProperty("status").GetString() ?? string.Empty;
          
            
            InvoiceDate = json.GetProperty("invoiceDate").GetDateTime();
            //InvoiceDate = json.GetProperty("invoiceDate").GetDateTime();
            CompanyId = json.GetProperty("companyId").GetInt32();
            CompanyId = json.GetProperty("categoryId").GetInt32();
            CompanyId = json.GetProperty("categorySubId").GetInt32();
            CompanyId = json.GetProperty("brandId").GetInt32();
            CompanyId = json.GetProperty("modelId").GetInt32();
            CreatedBy = json.GetProperty("createdBy").GetString() ?? string.Empty;
            CreatedUserId = json.GetProperty("createdUserId").GetString() ?? string.Empty;
            UpdatedBy = json.GetProperty("updatedBy").GetString() ?? string.Empty;
            UpdatedUserId = json.GetProperty("updatedUserId").GetString() ?? string.Empty;
            CreatedDate = json.GetProperty("createdDate").GetDateTime();
            UpdatedDate = json.GetProperty("updatedDate").GetDateTime();

            //Eğer alt hareketleri varsa buraya eklenecek
        }
        public void SetPropertiesFromJson(ProductModel jsonData)
        {
            Id = jsonData.Id;
            Name = jsonData.Name;
            Barcode = jsonData.Barcode;
            SerialNumber = jsonData.SerialNumber;
            Imei = jsonData.Imei;
            Mac = jsonData.Mac;
            DataClass = jsonData.DataClass;
            Status = jsonData.Status;
            InvoiceDate = jsonData.InvoiceDate;
            CompanyId = jsonData.CompanyId;
            CreatedBy = jsonData.CreatedBy;
            CreatedUserId = jsonData.CreatedUserId;
            UpdatedBy = jsonData.UpdatedBy;
            UpdatedUserId = jsonData.UpdatedUserId;
            CreatedDate = jsonData.CreatedDate;
            UpdatedDate = jsonData.UpdatedDate;
            CategorySubId = jsonData.CategorySubId;
            CategoryId = jsonData.CategoryId;
            BrandId = jsonData.BrandId;
            ModelId = jsonData.ModelId;
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
}

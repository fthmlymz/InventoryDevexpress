using InventoryManagement.Frontend.Common.Exceptions;
using InventoryManagement.Frontend.Models;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InventoryManagement.Frontend.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        public event Action<bool> DataLoaded;
        private readonly NotificationService notificationService;
        private readonly IJSRuntime _jsRuntime;
        private AuthenticationStateProvider _authenticationStateProvider;


        public ApiService(HttpClient httpClient, NotificationService notificationService, IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            this.notificationService = notificationService;
            _jsRuntime = jsRuntime;
            _authenticationStateProvider = authenticationStateProvider;
        }


        /*public async Task<T> GetAsync<T>(string apiUrl)
        {
            string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}"))
            {
                if (!string.IsNullOrEmpty(accessTokenResponse))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                }

                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                    {
                        notificationService.Notify(NotificationSeverity.Error, "Sunucuya erişilemiyor.");
                        throw new HttpRequestException("Sunucuya erişilemiyor.");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
        }*/

        public async Task<T> GetAsync<T>(string apiUrl)
        {
            string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}"))
            {
                if (!string.IsNullOrEmpty(accessTokenResponse))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                }

                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                    {
                        notificationService.Notify(NotificationSeverity.Error, "Sunucuya erişilemiyor.");
                        throw new HttpRequestException("Sunucuya erişilemiyor.");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new DateTimeConverter() }
                    };

                    return JsonSerializer.Deserialize<T>(content, options);
                }
            }
        }
        public class DateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), out DateTime result))
                {
                    return result;
                }
                return DateTime.MinValue; // veya hata durumu için exception fırlatabilirsiniz.
            }
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            }
        }


        //public async Task<TResponse> GetAsync<TResponse>(string apiUrl)
        //{
        //    string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");
        //    string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);

        //    using (var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}"))
        //    {
        //        if (!string.IsNullOrEmpty(accessTokenResponse))
        //        {
        //            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
        //        }

        //        using (var response = await _httpClient.SendAsync(request))
        //        {
        //            response.EnsureSuccessStatusCode();
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                notificationService.Notify(NotificationSeverity.Error, "Sunucuya erişilemiyor.");
        //                throw new HttpRequestException("Sunucuya erişilemiyor.");
        //            }

        //            var content = await response.Content.ReadAsStringAsync();
        //            return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
        //            {
        //                PropertyNameCaseInsensitive = true
        //            });
        //        }
        //    }
        //}




        public async Task<HttpResponseMessage> PostAsync<T>(string apiUrl, T data)
        {
            data.GetType().GetProperty("CreatedBy")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername);
            data.GetType().GetProperty("CreatedUserId")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Sub);

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);

            using (var request = new HttpRequestMessage(HttpMethod.Post, apiUrl))
            {
                if (!string.IsNullOrEmpty(accessTokenResponse))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                }
                request.Content = content;

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string responseJsonInner = await response.Content.ReadAsStringAsync();
                    JsonDocument responseDocument = JsonDocument.Parse(responseJsonInner);
                    JsonElement responseRoot = responseDocument.RootElement;
                    JsonElement exceptionElement = responseRoot.GetProperty("Exception");
                    string errorMessage = exceptionElement.GetProperty("Message").GetString();

                    notificationService.Notify(NotificationSeverity.Error, $"Hata", detail: $"Kayıt isteği gerçekleşemedi : {errorMessage}", duration: 6000);
                    throw new ApiException("Kayıt isteği gerçekleşemedi.", errorMessage);
                }

                string responseJson = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(responseJson);
                JsonElement root = document.RootElement;
                JsonElement dataElement = root.GetProperty("data");

                return new HttpResponseMessage
                {
                    StatusCode = response.StatusCode,
                    Content = new StringContent(dataElement.GetRawText(), Encoding.UTF8, "application/json")
                };
            }
        }


        public async Task<HttpResponseMessage> PutAsync<T>(string apiUrl, T data)
        {
            data.GetType().GetProperty("UpdatedBy")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername);
            data.GetType().GetProperty("UpdatedUserId")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Sub);

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);
            using (var request = new HttpRequestMessage(HttpMethod.Put, apiUrl))
            {
                if (!string.IsNullOrEmpty(accessTokenResponse))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                }
                request.Content = content;

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = response.Content.ReadAsStringAsync().Result; // async içerisinde olmadığı için Result kullanıyoruz.
                    var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(errorMessage);
                    if (errorData.TryGetValue("Exception", out var exceptionData))
                    {
                        var exceptionMessage = ((JsonElement)exceptionData).GetProperty("Message").GetString();
                        notificationService.Notify(NotificationSeverity.Error, $"Güncelleme isteği gerçekleşemedi. {exceptionMessage}");
                        throw new HttpRequestException("Güncelleme isteği gerçekleşemedi.");
                    }
                }

                return response;
            }
            /*data.GetType().GetProperty("UpdatedBy")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername);
            data.GetType().GetProperty("UpdatedUserId")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Sub);

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);
            using (var request = new HttpRequestMessage(HttpMethod.Put, apiUrl))
            {
                if (!string.IsNullOrEmpty(accessTokenResponse))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                }
                request.Content = content;

                return await _httpClient.SendAsync(request);
            }*/
        }
        public async Task<HttpResponseMessage> DeleteAsync(string apiUrl, int id)
        {
            //data.GetType().GetProperty("DeletedBy")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername);
            //data.GetType().GetProperty("DeletedUserId")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Sub);

            string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);

            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{apiUrl}/{id}"))
            {
                if (!string.IsNullOrEmpty(accessTokenResponse))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                }

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Silme isteği gerçekleşemedi.");
                    throw new HttpRequestException("Silme isteği gerçekleşemedi.");
                }
                return response;
            }

            /*string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);

            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{apiUrl}/{id}"))
            {
                if (!string.IsNullOrEmpty(accessTokenResponse))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                }
                return await _httpClient.SendAsync(request);
            }*/
        }


        protected virtual void OnDataLoaded(bool isLoaded)
        {
            var eventArgs = new DataLoadedEventArgs(isLoaded);
            DataLoaded?.Invoke(isLoaded);
        }
    }

    public class DataLoadedEventArgs : EventArgs
    {
        public bool IsLoaded { get; }
        public DataLoadedEventArgs(bool isLoaded)
        {
            IsLoaded = isLoaded;
        }
    }
}
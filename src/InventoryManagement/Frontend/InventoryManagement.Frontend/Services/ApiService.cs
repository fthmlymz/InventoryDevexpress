using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Common.Exceptions;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.FileManager;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InventoryManagement.Frontend.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        //public event Action<bool> DataLoaded;
        private readonly NotificationService _notificationService;
        private readonly ProtectedSessionStorage _sessionStorage;
        private AuthenticationStateProvider _authenticationStateProvider;


        public ApiService(HttpClient httpClient, NotificationService notificationService, AuthenticationStateProvider authenticationStateProvider, ProtectedSessionStorage sessionStorage)
        {
            _httpClient = httpClient;
            _notificationService = notificationService;
            _authenticationStateProvider = authenticationStateProvider;
            _sessionStorage = sessionStorage;
        }



        public async Task<T> GetAsync<T>(string apiUrl)
        {
            using (var response = await SendAuthorizedRequestAsync(HttpMethod.Get, apiUrl))
            {
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode)
                {
                    _notificationService.Notify(NotificationSeverity.Error, "Sunucuya erişilemiyor.");
                    throw new HttpRequestException("Sunucuya erişilemiyor.");
                }
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new DateTimeConverter() }
                };
                return JsonSerializer.Deserialize<T>(content, options)!;
            }
        }



        public async Task<HttpResponseMessage> PostAsync<T>(string apiUrl, T data)
        {
            data.GetType().GetProperty("CreatedBy")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername);
            data.GetType().GetProperty("CreatedUserId")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Sub);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new DateTimeConverter() }
            };
            var json = JsonSerializer.Serialize(data, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await SendAuthorizedRequestAsync(HttpMethod.Post, apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                string responseJsonInner = await response.Content.ReadAsStringAsync();
                JsonDocument responseDocument = JsonDocument.Parse(responseJsonInner);
                JsonElement responseRoot = responseDocument.RootElement;
                JsonElement exceptionElement = responseRoot.GetProperty("Exception");
                string errorMessage = exceptionElement.GetProperty("Message").GetString();

                _notificationService.Notify(NotificationSeverity.Error, $"Hata", detail: $"Kayıt isteği gerçekleşemedi : {errorMessage}", duration: 6000);
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

        public async Task<HttpResponseMessage> PutAsync<T>(string apiUrl, T data)
        {
            data.GetType().GetProperty("UpdatedBy")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername);
            data.GetType().GetProperty("UpdatedUserId")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Sub);

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await SendAuthorizedRequestAsync(HttpMethod.Put, apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                var errorData = JsonSerializer.Deserialize<Dictionary<string, object>>(errorMessage);
                if (errorData.TryGetValue("Exception", out var exceptionData))
                {
                    var exceptionMessage = ((JsonElement)exceptionData).GetProperty("Message").GetString();
                    _notificationService.Notify(NotificationSeverity.Error, $"Güncelleme isteği gerçekleşemedi. {exceptionMessage}");
                    throw new HttpRequestException("Güncelleme isteği gerçekleşemedi.");
                }
            }
            return response;
        }


        public async Task<HttpResponseMessage> DeleteAsync(string apiUrl, int id)
        {
            var response = await SendAuthorizedRequestAsync(HttpMethod.Delete, $"{apiUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Silme isteği gerçekleşemedi.");
                throw new HttpRequestException("Silme isteği gerçekleşemedi.");
            }
            return response;
        }
        //protected virtual void OnDataLoaded(bool isLoaded)
        //{
        //    var eventArgs = new DataLoadedEventArgs(isLoaded);
        //    DataLoaded?.Invoke(isLoaded);
        //}


        #region FileTransfer
        public async Task<HttpResponseMessage> UploadFileAsync(string apiUrl, MultipartFormDataContent content)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, apiUrl) { Content = content })
            {
                return await SendAuthorizedRequestAsync(request.Method, request.RequestUri.ToString(), content);
            }
        }
        public async Task<byte[]> DownloadFileAsync(string apiUrl)
        {
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, apiUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                throw new ApiException("Dosya indirme işlemi başarısız.", await response.Content.ReadAsStringAsync());
            }
        }
        public async Task<bool> DeleteFileAsync(string apiUrl)
        {
            var response = await SendAuthorizedRequestAsync(HttpMethod.Delete, apiUrl);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                // Hata yönetimi
                throw new ApiException("Dosya silme işlemi başarısız.", await response.Content.ReadAsStringAsync());
            }
        }
        public async Task<List<FileItemDto>> ListFilesAsync(string fileName = null)
        {
            string url = $"{ApiEndpointConstants.SearchFileManager}";
            if (!string.IsNullOrEmpty(fileName))
            {
                url += $"/{fileName}";
            }
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<PaginatedResult<FileItemDto>>(json);
                return apiResponse?.data ?? new List<FileItemDto>();
            }
            return new List<FileItemDto>();
        }
        #endregion


        private async Task<HttpResponseMessage> SendAuthorizedRequestAsync(HttpMethod method, string apiUrl, HttpContent content = null)
        {
            var encryptedAccessToken = await _sessionStorage.GetAsync<string>("permissions_token");
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken.Value) ? null : encryptedAccessToken.Value;

            using (var request = new HttpRequestMessage(method, apiUrl) { Content = content })
            {
                if (!string.IsNullOrEmpty(accessTokenResponse))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                }

                return await _httpClient.SendAsync(request);
            }
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

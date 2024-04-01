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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            data.GetType().GetProperty("CreatedBy")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
                var errorMessage = await ExtractErrorMessage(response);
                _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Kayıt isteği gerçekleşemedi: {errorMessage}", 6000);
                return response; // Hata durumunda orijinal HTTP yanıtını döndür
            }

            // Başarılı yanıt işleme
            var responseJson = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(responseJson);
            if (document.RootElement.TryGetProperty("data", out var dataElement))
            {
                // 'data' alanını içeren yeni bir HttpResponseMessage döndür
                return new HttpResponseMessage
                {
                    StatusCode = response.StatusCode,
                    Content = new StringContent(dataElement.GetRawText(), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                // 'data' alanı mevcut değilse, boş bir yanıt döndür
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
        }

        private async Task<string> ExtractErrorMessage(HttpResponseMessage response)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(responseJson);
            var root = document.RootElement;

            if (root.TryGetProperty("Exception", out var exceptionElement) && exceptionElement.TryGetProperty("Message", out var messageElement))
            {
#pragma warning disable CS8603 // Possible null reference return.
                return messageElement.GetString();
#pragma warning restore CS8603 // Possible null reference return.
            }

            return "Bir hata oluştu.";
        }



        public async Task<HttpResponseMessage> PutAsync<T>(string apiUrl, T data)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            data.GetType().GetProperty("UpdatedBy")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            data.GetType().GetProperty("UpdatedUserId")?.SetValue(data, ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Sub);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new DateTimeConverter() }
            };
            var json = JsonSerializer.Serialize(data, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await SendAuthorizedRequestAsync(HttpMethod.Put, apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await ExtractErrorMessage(response);
                _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Güncelleme isteği gerçekleşemedi: {errorMessage}", 6000);
                return response; // Hata durumunda orijinal HTTP yanıtını döndür
            }

            // Başarılı yanıt işleme
            return new HttpResponseMessage
            {
                StatusCode = response.StatusCode,
                Content = response.Content // Başarılı bir yanıtın içeriğini doğrudan döndür
            };
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                return await SendAuthorizedRequestAsync(request.Method, request.RequestUri.ToString(), content);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public async Task<List<FileItemDto>> ListFilesAsync(string fileName = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
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


#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private async Task<HttpResponseMessage> SendAuthorizedRequestAsync(HttpMethod method, string apiUrl, HttpContent content = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            var encryptedAccessToken = await _sessionStorage.GetAsync<string>("permissions_token");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken.Value) ? null : encryptedAccessToken.Value;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

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

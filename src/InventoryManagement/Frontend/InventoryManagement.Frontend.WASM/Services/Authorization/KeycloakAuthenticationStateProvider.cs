using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.Models.Keycloak;
using InventoryManagement.Frontend.Services.Token;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace InventoryManagement.Frontend.Services.Authorization
{
    public class KeycloakAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;
        private readonly NotificationService _notificationService;
        public UserInfo UserInfo { get; private set; }


        public KeycloakAuthenticationStateProvider(HttpClient httpClient, NavigationManager navigationManager, IJSRuntime jsRuntime,
            NotificationService notificationService)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
            _notificationService = notificationService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "access_token");
            string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);

            string encryptedRefreshToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "refresh_token");
            string refreshTokenResponse = string.IsNullOrEmpty(encryptedRefreshToken) ? null : EncryptionService.Decrypt(encryptedRefreshToken);

            if (!string.IsNullOrEmpty(accessTokenResponse))
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var jwtToken = jwtHandler.ReadJwtToken(accessTokenResponse);

                var expirationTime = jwtToken.ValidTo;
                var currentTime = DateTime.UtcNow;

                if (expirationTime < currentTime)
                {
                    if (!string.IsNullOrEmpty(refreshTokenResponse))
                    {
                        var newTokenResponse = await RefreshTokenAsync(refreshTokenResponse);
                        if (newTokenResponse != null)
                        {
                            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "access_token", EncryptionService.Encrypt(newTokenResponse.AccessToken));
                            accessTokenResponse = newTokenResponse.AccessToken;
                            jwtToken = jwtHandler.ReadJwtToken(accessTokenResponse);
                            expirationTime = jwtToken.ValidTo;
                        }
                    }
                    else
                    {
                        await LogoutAsync();

                        _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Mevcut oturumunuz sonlanmış, yeniden oturum açmanız gerekiyor.", duration: 6000);

                        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                    }
                }
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                var userInfo = await GetUserInfoAsync();
                var identity = new ClaimsIdentity(ParseClaims(userInfo), "Keycloak");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task<TokenResponse> LoginAsync(string username, string password)
        {
            var loginData = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            var content = new FormUrlEncodedContent(loginData);

            var tokenResponse = await _httpClient.PostAsync(ApiEndpointConstants.KeyCloakUserTokenEndpoint + "/getToken", content);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenResponseObject = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson);

                if (tokenResponseObject != null)
                {
                    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "access_token", EncryptionService.Encrypt(tokenResponseObject.AccessToken));
                    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "refresh_token", EncryptionService.Encrypt(tokenResponseObject.RefreshToken));

                    var userInfo = await GetUserInfoAsync();
                    if (userInfo != null)
                    {
                        var identity = new ClaimsIdentity(ParseClaims(userInfo), tokenResponseObject.AccessToken);
                        var user = new ClaimsPrincipal(identity);
                        var authState = new AuthenticationState(user);
                        NotifyAuthenticationStateChanged(Task.FromResult(authState));

                        //Token izinlerini al
                        await GetTokenPermissionsAsync(tokenResponseObject.AccessToken);
                    }
                }
                return tokenResponseObject;
            }
            return null;
        }
        public async Task LogoutAsync()
        {
            try
            {
                // refresh_token'i al
                string encryptedRefreshToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "refresh_token");
                string refreshToken = string.IsNullOrEmpty(encryptedRefreshToken) ? null : EncryptionService.Decrypt(encryptedRefreshToken);

                string encryptedAccessToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "access_token");
                string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken) ? null : EncryptionService.Decrypt(encryptedAccessToken);

                var logoutData = new Dictionary<string, string>
                {
                    { "refresh_token", refreshToken }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, ApiEndpointConstants.KeyCloakUserLogoutEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse);
                request.Content = new FormUrlEncodedContent(logoutData);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    UserInfo = null;
                    await NotifyAndRedirect();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await NotifyAndRedirect("Yetkilendirme Hatası", "Kullanıcı yetkilendirilmemiş");
                }
                else
                {
                    await NotifyAndRedirect("Hata", $"Logout işlemi başarısız. Hata kodu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                await NotifyAndRedirect("Hata", $"Sunucudan çıkış işlemi başarısız. Hata: {ex.Message}");
            }
            //finally
            //{
            //    _httpClient.Dispose();
            //}
        }

        private async Task NotifyAndRedirect(string title = null, string message = null)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(message))
            {
                _notificationService.Notify(NotificationSeverity.Error, title, message);
            }

            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "access_token");
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "refresh_token");
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "permissions_token");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
            _navigationManager.NavigateTo("/login");
        }



        private async Task<UserInfo> GetUserInfoAsync()
        {
            string encryptedToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "access_token");
            string tokenResponse = string.IsNullOrEmpty(encryptedToken) ? null : EncryptionService.Decrypt(encryptedToken);

            if (!string.IsNullOrEmpty(tokenResponse))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse);
                var response = await _httpClient.GetAsync(ApiEndpointConstants.KeyCloakUserInfoEndpoint);
                if (response.IsSuccessStatusCode)
                {
                    var userInfoJson = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonSerializer.Deserialize<UserInfo>(userInfoJson);
                    UserInfo = JsonSerializer.Deserialize<UserInfo>(userInfoJson);

                    return userInfo;
                }
                else
                {
                    _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Kullanıcı bilgileri alınamadı {response.StatusCode}", duration: 6000);
                    _navigationManager.NavigateTo("/login");
                }
            }

            return null;
        }
        private async Task<TokenPermissions> GetTokenPermissionsAsync(string access_token)
        {
            var permissionsData = new Dictionary<string, string> { { "claim_token", access_token } };

            if (!string.IsNullOrEmpty(access_token))
            {
                var content = new FormUrlEncodedContent(permissionsData);
                var response = await _httpClient.PostAsync(ApiEndpointConstants.KeyCloakUserTokenEndpoint + "/permissions", content);

                if (response.IsSuccessStatusCode)
                {
                    var permissionInfoJson = await response.Content.ReadAsStringAsync();
                    var permissionsResponseObject = JsonSerializer.Deserialize<TokenResponse>(permissionInfoJson);

                    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "permissions_token", EncryptionService.Encrypt(permissionsResponseObject.AccessToken));

                    // Notify Authentication State Changed
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

                    // Ana sayfaya yönlendirme işlemi
                    _navigationManager.NavigateTo("/", forceLoad: true);


                    return JsonSerializer.Deserialize<TokenPermissions>(permissionInfoJson);
                }
                else
                {
                    _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Kullanıcı izin bilgileri alınamadı {response.StatusCode}", duration: 6000);
                }
            }
            return null;
        }
        private IEnumerable<Claim> ParseClaims(UserInfo userInfo)
        {
            var claims = new List<Claim>();

            if (userInfo != null)
            {
                if (!string.IsNullOrEmpty(userInfo.Sub))
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, userInfo.Sub));
                }

                if (!string.IsNullOrEmpty(userInfo.PreferredUsername))
                {
                    claims.Add(new Claim("PreferredUsername", userInfo.PreferredUsername));
                }

                claims.Add(new Claim("email_verified", userInfo.EmailVerified.ToString()));

                if (!string.IsNullOrEmpty(userInfo.Name))
                {
                    claims.Add(new Claim(ClaimTypes.Name, userInfo.Name));
                }

                if (!string.IsNullOrEmpty(userInfo.GivenName))
                {
                    claims.Add(new Claim(ClaimTypes.GivenName, userInfo.GivenName));
                }

                if (!string.IsNullOrEmpty(userInfo.FamilyName))
                {
                    claims.Add(new Claim(ClaimTypes.Surname, userInfo.FamilyName));
                }

                if (!string.IsNullOrEmpty(userInfo.Email))
                {
                    claims.Add(new Claim(ClaimTypes.Email, userInfo.Email));
                }
            }
            return claims;
        }
        private async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var refreshTokenData = new Dictionary<string, string> { { "refresh_token", refreshToken } };

            var content = new FormUrlEncodedContent(refreshTokenData);

            var tokenResponse = await _httpClient.PostAsync(ApiEndpointConstants.KeyCloakUserTokenEndpoint + "/refreshToken", content);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenResponseObject = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson);

                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "access_token", EncryptionService.Encrypt(tokenResponseObject.AccessToken));
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "refresh_token", EncryptionService.Encrypt(tokenResponseObject.RefreshToken));
                return tokenResponseObject;
            }
            return null;
        }
    }
}

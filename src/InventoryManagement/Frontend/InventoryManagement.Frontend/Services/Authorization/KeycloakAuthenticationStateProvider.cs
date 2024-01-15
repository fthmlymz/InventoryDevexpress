using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Keycloak;
using InventoryManagement.Frontend.Services.Token;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
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
        private readonly ProtectedSessionStorage _browserStorage;
        private readonly NotificationService _notificationService;
        private Timer _tokenRefreshTimer;
        private readonly IConfiguration _configuration;

        public UserInfo UserInfo { get; private set; }


        public KeycloakAuthenticationStateProvider(HttpClient httpClient, NavigationManager navigationManager,
            NotificationService notificationService, ProtectedSessionStorage browserStorage, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _notificationService = notificationService;
            _browserStorage = browserStorage;
            _configuration = configuration;
        }


        public async Task<TokenResponse> LoginAsync(string username, string password)
        {
            var loginData = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password },
                { "grant_type", _configuration.GetSection("Keycloak:GrantType").Value },
                { "client_id",  _configuration.GetSection("Keycloak:ClientId").Value },
                { "client_secret",  _configuration.GetSection("Keycloak:ClientSecret").Value }
            };

            var content = new FormUrlEncodedContent(loginData);

            var tokenResponse = await _httpClient.PostAsync(ApiEndpointConstants.KeycloakUserGetTokenEndpoint, content);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenResponseObject = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson);

                if (tokenResponseObject != null)
                {
                    await _browserStorage.SetAsync("access_token", tokenResponseObject.AccessToken);
                    await _browserStorage.SetAsync("refresh_token", tokenResponseObject.RefreshToken);

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


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var encryptedAccessToken = await _browserStorage.GetAsync<string>("access_token");
                string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken.Value) ? null : encryptedAccessToken.Value;

                var encryptedRefreshToken = await _browserStorage.GetAsync<string>("refresh_token");
                string refreshTokenResponse = string.IsNullOrEmpty(encryptedRefreshToken.Value) ? null : encryptedRefreshToken.Value;


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
                                await _browserStorage.SetAsync("access_token", newTokenResponse.AccessToken);
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
            catch (Exception)
            {

            }
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }





        public async Task LogoutAsync()
        {
            try
            {
                var encryptedRefreshToken = await _browserStorage.GetAsync<string>("refresh_token");
                string refreshToken = string.IsNullOrEmpty(encryptedRefreshToken.Value) ? null : encryptedRefreshToken.Value;

                var encryptedAccessToken = await _browserStorage.GetAsync<string>("access_token");
                string accessTokenResponse = string.IsNullOrEmpty(encryptedAccessToken.Value) ? null : encryptedAccessToken.Value;


                var logoutData = new Dictionary<string, string>
                {
                    { "refresh_token", refreshToken },
                    { "client_id",  _configuration.GetSection("Keycloak:ClientId").Value },
                    { "client_secret",  _configuration.GetSection("Keycloak:ClientSecret").Value }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, ApiEndpointConstants.KeycloakEndpoint);
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
        }

        protected internal async Task NotifyAndRedirect(string? title = null, string? message = null)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(message))
            {
                _notificationService.Notify(NotificationSeverity.Error, title, message);
            }

            await _browserStorage.DeleteAsync("access_token");
            await _browserStorage.DeleteAsync("refresh_token");
            await _browserStorage.DeleteAsync("permissions_token");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
            _navigationManager.NavigateTo("/login", forceLoad: true);
        }


        private async Task<UserInfo> GetUserInfoAsync()
        {
            var encryptedToken = await _browserStorage.GetAsync<string>("access_token");
            string tokenResponse = string.IsNullOrEmpty(encryptedToken.Value) ? null : encryptedToken.Value;

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
                    await NotifyAndRedirect($"Kullanıcı bilgileri alınamadı", $"Kullanıcı bilgileri alınamadı{response.StatusCode}");
                    _navigationManager.NavigateTo("/login"); //force load iptal edilecek
                }
            }
            return null;
        }

        private async Task<TokenPermissions> GetTokenPermissionsAsync(string access_token)
        {
            var permissionsData = new Dictionary<string, string> {
                { "claim_token", access_token },
                { "grant_type", _configuration.GetSection("Keycloak:GrantTypePermissions").Value },
                { "client_id",  _configuration.GetSection("Keycloak:ClientId").Value },
                { "client_secret",  _configuration.GetSection("Keycloak:ClientSecret").Value },
                { "realm",  _configuration.GetSection("Keycloak:Realm").Value },
                { "audience",  _configuration.GetSection("Keycloak:Audience").Value }
            };

            if (!string.IsNullOrEmpty(access_token))
            {
                var content = new FormUrlEncodedContent(permissionsData);
                var response = await _httpClient.PostAsync(ApiEndpointConstants.KeycloakUserGetTokenEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var permissionInfoJson = await response.Content.ReadAsStringAsync();
                    var permissionsResponseObject = JsonSerializer.Deserialize<TokenResponse>(permissionInfoJson);

                    await _browserStorage.SetAsync("permissions_token", permissionsResponseObject.AccessToken);

                    // Notify Authentication State Changed
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

                    // Ana sayfaya yönlendirme işlemi
                    _navigationManager.NavigateTo("/", forceLoad: false);


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


        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var refreshTokenData = new Dictionary<string, string> {
                { "client_id",  _configuration.GetSection("Keycloak:ClientId").Value },
                { "client_secret",  _configuration.GetSection("Keycloak:ClientSecret").Value },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };
            var content = new FormUrlEncodedContent(refreshTokenData);
            var tokenResponse = await _httpClient.PostAsync(ApiEndpointConstants.KeycloakUserGetTokenEndpoint, content);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenResponseObject = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson);

                await _browserStorage.SetAsync("access_token", tokenResponseObject.AccessToken);
                await _browserStorage.SetAsync("refresh_token", tokenResponseObject.RefreshToken);

                await StartOrResetTokenRefreshTimer(tokenResponseObject.ExpiresIn);

                var userInfo = await GetUserInfoAsync(); // UserInfo güncellemesi
                if (userInfo != null)
                {
                    var identity = new ClaimsIdentity(ParseClaims(userInfo), "Bearer");
                    var user = new ClaimsPrincipal(identity);
                    var authState = new AuthenticationState(user);
                    NotifyAuthenticationStateChanged(Task.FromResult(authState));
                }


                var permissionsData = new Dictionary<string, string> { { "claim_token", tokenResponseObject.AccessToken } };
                if (!string.IsNullOrEmpty(tokenResponseObject.AccessToken))
                {
                    var content_permission = new FormUrlEncodedContent(permissionsData);
                    var response = await _httpClient.PostAsync(ApiEndpointConstants.KeycloakUserGetTokenEndpoint, content_permission);

                    if (response.IsSuccessStatusCode)
                    {
                        var permissionInfoJson = await response.Content.ReadAsStringAsync();
                        var permissionsResponseObject = JsonSerializer.Deserialize<TokenResponse>(permissionInfoJson);

                        await _browserStorage.SetAsync("permissions_token", permissionsResponseObject.AccessToken);

                        //NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    }
                }
                return tokenResponseObject;
            }
            return null;
        }
        private async Task StartOrResetTokenRefreshTimer(int tokenLifetime)
        {
            Console.WriteLine("zamanlayıcı şimdi çalışıyor");
            var accessTokenResult = await _browserStorage.GetAsync<string>("access_token");
            if (!accessTokenResult.Success || string.IsNullOrEmpty(accessTokenResult.Value))
            {
                await NotifyAndRedirect("Kimlik Doğrulama", "Kimlik doğrulaması yapılamadı !");
                return;
            }

            var refreshInterval = TimeSpan.FromSeconds(tokenLifetime * 0.7);
            _tokenRefreshTimer?.Dispose();
            _tokenRefreshTimer = new Timer(RefreshToken, null, refreshInterval, Timeout.InfiniteTimeSpan);
        }
        private async void RefreshToken(object state)
        {
            var refreshTokenResult = await _browserStorage.GetAsync<string>("refresh_token");

            if (refreshTokenResult.Success && !string.IsNullOrEmpty(refreshTokenResult.Value))
            {
                await RefreshTokenAsync(refreshTokenResult.Value);
            }
        }
    }
}

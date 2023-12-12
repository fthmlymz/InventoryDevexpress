using Microsoft.JSInterop;
using Radzen;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;

namespace InventoryManagement.Frontend.Services.Authorization
{
    public interface IAuthorizationService
    {
        bool HasPermission(string tableName, string permission);
    }

    public class PermissionsService : IAuthorizationService
    {
        private readonly NotificationService _notificationService;
        private readonly IDictionary<string, IList<string>> _permissions;
        private bool _tokenInitialized;

        public PermissionsService(IJSRuntime runtime, NotificationService notificationService)
        {
            _notificationService = notificationService;
            _permissions = new Dictionary<string, IList<string>>();
            _tokenInitialized = false;

            InitializePermissionsAsync(runtime);
        }

        private async void InitializePermissionsAsync(IJSRuntime runtime)
        {
            string encryptedAccessToken = await runtime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");

            if (string.IsNullOrEmpty(encryptedAccessToken))
            {
                return;
            }

            string idToken = EncryptionService.Decrypt(encryptedAccessToken);
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(idToken);


            if (jwtSecurityToken.Payload.ContainsKey("authorization"))
            {
                var authorization = jwtSecurityToken.Payload["authorization"].ToString();

                var permissions = JsonDocument.Parse(authorization).RootElement.GetProperty("permissions").EnumerateArray();

                foreach (var permission in permissions)
                {
                    var table = permission.GetProperty("rsname").ToString();
                    var scopes = permission.GetProperty("scopes").EnumerateArray().Select(s => s.ToString()).ToList();

                    if (!_permissions.ContainsKey(table))
                    {
                        _permissions[table] = new List<string>();
                    }

                    foreach (var scope in scopes)
                    {
                        _permissions[table].Add(scope);
                    }
                }
            }

            _tokenInitialized = true;
        }

        public bool HasPermission(string tableName, string permission)
        {
            if (!_tokenInitialized)
            {
                return false;
            }

            if (_permissions.ContainsKey(tableName))
            {
                return _permissions[tableName].Contains(permission);
            }

            return false;
        }
    }
}



/*namespace InventoryManagement.Frontend.Services.Authorization
{
    public interface IAuthorizationService
    {
        bool HasPermission(string tableName, string permission);
    }

    public class PermissionsService : IAuthorizationService
    {
        private readonly NotificationService _notificationService;
        private string _token;
        private readonly IJSRuntime _runtime;
        private bool _tokenInitialized;
        public PermissionsService(IJSRuntime runtime, NotificationService notificationService)
        {
            _runtime = runtime;
            _tokenInitialized = false;
            InitializeTokenAsync();
            _notificationService = notificationService;
        }
        private async void InitializeTokenAsync()
        {
            if (!_tokenInitialized)
            {
                string decryptedToken = await GetDecryptedToken();
                _token = decryptedToken;
                _tokenInitialized = true;
            }
        }
        public bool HasPermission(string tableName, string permission)
        {
            if (!_tokenInitialized)
            {
                return false;
            }
            if (tableName.StartsWith("res:") && permission.StartsWith("scopes:"))
            {
                string tableType = tableName.Substring(4);
                string scope = permission;

                return CheckTokenForPermission(scope) && CheckUserHasTablePermission(tableType);
            }
            return false;
        }
        private bool CheckUserHasTablePermission(string tableType)
        {
            if (_token.Contains($"{tableType}"))
            {
                return true;
            }
            else
            {
                //Burada kullanıcı ekranına otify vermek yerine bir log veya console yazdırmak daha iyi
                //_notificationService.Notify(NotificationSeverity.Error, "Hata", $"{tableType} tablosunu görüntüleme yetkisi bulunamadı", duration: 6000);
                return false;
            }
        }
        private bool CheckTokenForPermission(string scope)
        {
            switch (scope)
            {
                case "scopes:read":
                    return CheckTokenForReadPermission();
                case "scopes:create":
                    return CheckTokenForCreatePermission();
                case "scopes:update":
                    return CheckTokenForUpdatePermission();
                case "scopes:delete":
                    return CheckTokenForDeletePermission();
                default:
                    return false;
            }
        }
        private bool CheckUserHasPermission(string scope)
        {
            if (_token.Contains($"scopes:{scope}"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckTokenForReadPermission()
        {
            return CheckUserHasPermission("read");
        }
        private bool CheckTokenForCreatePermission()
        {
            return CheckUserHasPermission("create");
        }
        private bool CheckTokenForUpdatePermission()
        {
            return CheckUserHasPermission("update");
        }
        private bool CheckTokenForDeletePermission()
        {
            return CheckUserHasPermission("delete");
        }
        private async Task<string> GetDecryptedToken()
        {
            string encryptedAccessToken = await _runtime.InvokeAsync<string>("sessionStorage.getItem", "permissions_token");

            if (string.IsNullOrEmpty(encryptedAccessToken))
            {
                return string.Empty;
               // throw new ArgumentException("Encrypted data cannot be null or empty.");
            }
            string idToken = EncryptionService.Decrypt(encryptedAccessToken);
            var token = idToken;
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            return jwtSecurityToken.ToString();
        }
    }
}
*/
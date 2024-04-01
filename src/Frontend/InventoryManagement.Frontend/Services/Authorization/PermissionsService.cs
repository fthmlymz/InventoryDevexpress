using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace InventoryManagement.Frontend.Services.Authorization
{
    public interface IAuthorizationService
    {
        Task InitializeAsync();
        bool HasPermission(string tableName, string permission);
    }

    public class PermissionsService : IAuthorizationService
    {
        private readonly ProtectedSessionStorage _browserStorage;
        private readonly IDictionary<string, IList<string>> _permissions;
        private bool _tokenInitialized;

        public PermissionsService(ProtectedSessionStorage browserStorage)
        {
            _permissions = new Dictionary<string, IList<string>>();
            _tokenInitialized = false;
            _browserStorage = browserStorage;
        }


        public async Task InitializeAsync()
        {
            var encryptedAccessToken = await _browserStorage.GetAsync<string>("permissions_token");

            if (!string.IsNullOrEmpty(encryptedAccessToken.Value))
            {
                var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(encryptedAccessToken.Value);

                if (jwtSecurityToken.Payload.ContainsKey("authorization"))
                {
                    var authorization = jwtSecurityToken.Payload["authorization"].ToString();
#pragma warning disable CS8604 // Possible null reference argument.
                    ProcessPermissions(authorization);
#pragma warning restore CS8604 // Possible null reference argument.
                }
            }
            _tokenInitialized = true;
        }
        private void ProcessPermissions(string authorization)
        {
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

using System.Text.Json.Serialization;

namespace InventoryManagement.Frontend.Services.Token
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }


        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }


        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }


        [JsonPropertyName("session_state")]
        public string SessionState { get; set; }


        [JsonPropertyName("scope")]
        public string Scope { get; set; }




        /*[JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("not_before_policy")]
        public int NotBeforePolicy { get; set; }

        [JsonPropertyName("session_state")]
        public string SessionState { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("clientSecret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("clientApp")]
        public object ClientApp { get; set; }

        [JsonPropertyName("grantTypePermission")]
        public string GrantTypePermission { get; set; }

        [JsonPropertyName("realm")]
        public string Realm { get; set; }

        [JsonPropertyName("audience")]
        public string Audience { get; set; }

        [JsonPropertyName("userInfoEndpoint")]
        public string UserInfoEndpoint { get; set; }

        [JsonPropertyName("tokenEndpoint")]
        public string TokenEndpoint { get; set; }

        [JsonPropertyName("tokenPermissionsEndpoint")]
        public string TokenPermissionsEndpoint { get; set; }*/
    }
}

using System.Text.Json.Serialization;

namespace InventoryManagement.Frontend.DTOs.Keycloak
{
    public class UserInfo
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("preferred_username")]
        public string PreferredUsername { get; set; }

        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}

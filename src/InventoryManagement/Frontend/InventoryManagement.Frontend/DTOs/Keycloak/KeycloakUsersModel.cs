namespace InventoryManagement.Frontend.DTOs.Keycloak
{
    public class KeycloakUsersModel
    {
        public string? Id { get; set; }
        public long? CreatedTimestamp { get; set; }
        public string? Username { get; set; }
        public bool? Enabled { get; set; }
        public bool? Totp { get; set; }
        public bool? EmailVerified { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? FederationLink { get; set; }
        public Attributes? Attributes { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public string? AssignedUserId { get; set; }
        public string? AssignedUserPhoto { get; set; } //test sonrası sil
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? AssignedUserName { get; set; }
        public string? FullName { get; set; }
        public string? Barcode { get; set; }
    }

    public class Attributes
    {
        public List<string>? LDAP_ENTRY_DN { get; set; }
        public List<string>? LDAP_ID { get; set; }
        public List<string>? sAMAccountName { get; set; }
        public List<string>? thumbnailPhoto { get; set; }
    }
}

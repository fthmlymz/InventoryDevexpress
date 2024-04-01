namespace InventoryManagement.Frontend.DTOs.Keycloak
{
    public class KeycloakUsersDto
    {
        public string? Id { get; set; }
        public long CreatedTimestamp { get; set; }
        public string? Username { get; set; }
        public bool Enabled { get; set; }
        public bool Totp { get; set; }
        public bool EmailVerified { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? FederationLink { get; set; }
        public Attributes? Attributes { get; set; }
        public List<string>? DisableableCredentialTypes { get; set; }
        public List<string>? RequiredActions { get; set; }
        public int NotBefore { get; set; }
        public Access? Access { get; set; }
    }

    public class Attributes
    {
        public List<string>? LDAP_ENTRY_DN { get; set; }
        public List<string>? sAMAccountName { get; set; }
        public List<string>? ThumbnailPhoto { get; set; }
        public List<string>? ModifyTimestamp { get; set; }
        public List<string>? LDAP_ID { get; set; }
        public List<string>? CreateTimestamp { get; set; }
        public List<string>? Company { get; set; }
        public List<string>? PhysicalDeliveryOfficeName { get; set; }
        public List<string>? Title { get; set; }
        public List<string>? Manager { get; set; }
        public List<string>? Department { get; set; }
    }

    public class Access
    {
        public bool ManageGroupMembership { get; set; }
        public bool View { get; set; }
        public bool MapRoles { get; set; }
        public bool Impersonate { get; set; }
        public bool Manage { get; set; }
    }


    /*public class KeycloakUsersDto
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
    }*/
}

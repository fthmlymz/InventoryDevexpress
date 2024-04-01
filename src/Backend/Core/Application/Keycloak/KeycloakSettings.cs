namespace Application.Keycloak
{
    public class KeycloakSettings
    {
        public string? Authority { get; set; }
        public string? Audience { get; set; }
        public string? KeycloakResourceUrl { get; set; }
        public string? ClientCredentialsTokenAddress { get; set; }
    }
}

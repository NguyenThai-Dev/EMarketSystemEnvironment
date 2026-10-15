namespace EMarket.Modules.UserModule.DTOs
{
    public class ExternalLoginRequestDTO
    {
        public string Provider { get; set; } = "Google";
        public string Email { get; set; }
        public string IdToken { get; set; }
        public string FullName { get; set; }
    }
}

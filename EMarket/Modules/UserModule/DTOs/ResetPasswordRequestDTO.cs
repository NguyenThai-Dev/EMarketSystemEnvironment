namespace EMarket.Modules.UserModule.DTOs
{
    public class ResetPasswordRequestDTO
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}
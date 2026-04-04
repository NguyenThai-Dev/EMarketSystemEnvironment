using EMarket.Modules.UserModule.Enums;

namespace EMarket.Modules.UserModule.DTOs
{
    public class LoginResponseDTO
    {
        public LoginStatus Status { get; set; }
        public CurrentUserDTO User { get; set; }
    }

}
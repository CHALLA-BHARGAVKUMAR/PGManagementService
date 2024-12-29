using PGManagementService.Data.DTO;
using PGManagementService.Models;

namespace PGManagementService.Interfaces
{
    public interface IRegistrationBL
    {
        Task<string> Login(LoginRequest loginRequest);

        Task<bool> ResetPassword(ResetPasswordRequest resetRequest);

        Task<bool> ForgotPassword(string email);
    }
}

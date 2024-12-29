using System.ComponentModel.DataAnnotations;
using PGManagementService.Data.enums;

namespace PGManagementService.Data.DTO
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public string Otp {  get; set; }
    }

}

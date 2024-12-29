using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PGManagementService.Data;
using PGManagementService.Data.DTO;
using PGManagementService.Interfaces;
using PGManagementService.Models;
using PGManagementService.Services;

namespace PGManagementService.BusinessLogic
{
    public class RegistrationBL: IRegistrationBL
    {
        private readonly ILogger<RegistrationBL> _logger;
        private readonly PGManagementServiceDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;

        public RegistrationBL(ILogger<RegistrationBL> logger, PGManagementServiceDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, EmailService emailService)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<string> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                return GenerateJwtToken(user); ;                        
            }
            return default;
               
        }

        public async Task<bool> ForgotPassword(string email)
        {
            
            try
            {
                var user = await _userManager.FindByNameAsync(email);

                if (user == null)
                {
                    return false;
                }

                var otp = new Random().Next(100000, 999999).ToString();

                user.OTP = otp;
                user.OTPExpiration = DateTime.UtcNow.AddMinutes(10);
                await  _userManager.UpdateAsync(user);

                // Send OTP via email
                var subject = "Reset Password OTP";
                var body = $"Your OTP for resetting the password is {otp}. It is valid for 10 minutes.";
                await  _emailService.SendEmailAsync(email, subject, body);
            }
            catch(Exception ex)
            {
                _logger.LogError($"RegistrationBl : ForgotPassword {ex}");
                return false;
            }
            return true;

        }


        public async Task<bool> ResetPassword(ResetPasswordRequest resetRequest)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {

                try
                {
                    var user = await _userManager.FindByNameAsync(resetRequest.Email);
                    if (user == null)
                    {
                        return false;
                    }

                    if (user.OTP != resetRequest.Otp || user.OTPExpiration < DateTime.UtcNow)
                    {
                        return false;
                    }

                    // Reset password
                    var resetResult = await _userManager.ResetPasswordAsync(
                        user,
                        await _userManager.GeneratePasswordResetTokenAsync(user),
                        resetRequest.NewPassword
                    );

                    if (!resetResult.Succeeded)
                    {
                        return false;
                    }

                    // Clear OTP
                    user.OTP = null;
                    user.OTPExpiration = null;
                    await _userManager.UpdateAsync(user);
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"RegistrationBl : ResetPassword {ex}");
                    return false;
                }

                return true;
            }
            

        }



        #region private method
        private string GenerateJwtToken(IdentityUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bhargavkumarchallaPGManagementService")); //secret key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role, "Member"),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var token = new JwtSecurityToken(
                issuer: "domain",        
                audience: "domain",   
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion private method

    }
}

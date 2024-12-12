using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PGManagementService.Data.DTO;
using PGManagementService.Interfaces;
using PGManagementService.Models;
using PGManagementService.Services;

namespace PGManagementService.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, EmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
        }

        // Login GET
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login POST
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                /*return RedirectToAction("ResetPassword", new { userId = user.Id })*/;
            }

            ViewBag.ErrorMessage = "Invalid login attempt.";
            return View();
        }

        public IActionResult AccessDenied()
        {
            if (Request.Query.ContainsKey("ReturnUrl"))
            {
                var returnUrl = Request.Query["ReturnUrl"];
                // Do something with the returnUrl, like storing it for later or using it for redirection
            }

            return View();
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ResetPassword(string userId)
        {
            // Validate if user exists
            var user = _userManager.FindByNameAsync(userId).Result;
            if (user == null)
            {
                return NotFound();
            }

            // Create the ResetPasswordViewModel and pass it to the view
            var model = new ResetPasswordViewModel
            {
                UserId = userId
            };

            return View(model);
        }


        


        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Generate a 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Store OTP and expiration
            user.OTP = otp;
            user.OTPExpiration = DateTime.UtcNow.AddMinutes(10); // OTP valid for 10 minutes
            await _userManager.UpdateAsync(user);

            // Send OTP via email
            var subject = "Reset Password OTP";
            var body = $"Your OTP for resetting the password is {otp}. It is valid for 10 minutes.";
            await _emailService.SendEmailAsync(email, subject, body);

            return Ok("OTP sent to your email.");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }

                var user = await _userManager.FindByNameAsync(model.UserId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (user.OTP != model.Otp || user.OTPExpiration < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired OTP.");
            }

            // Reset password
            var resetResult = await _userManager.ResetPasswordAsync(
                user,
                await _userManager.GeneratePasswordResetTokenAsync(user),
                model.NewPassword
            );

            if (!resetResult.Succeeded)
            {
                return BadRequest("Password reset failed.");
            }

            // Clear OTP
            user.OTP = null;
            user.OTPExpiration = null;
            await _userManager.UpdateAsync(user);

            return Ok("Password reset successful.");
        }



    }
}


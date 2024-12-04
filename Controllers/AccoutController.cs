using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PGManagementService.Data.DTO;
using PGManagementService.Interfaces;
using PGManagementService.Models;

namespace PGManagementService.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
                return RedirectToAction("ResetPassword", new { userId = user.Id });
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
            var user = _userManager.FindByIdAsync(userId).Result;
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
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                // Check if the user is logging in with the default password
                var isDefaultPassword = await _userManager.CheckPasswordAsync(user, "DefaultPassword123!");

                if (isDefaultPassword)
                {
                    // Change the user's password
                    var result = await _userManager.ChangePasswordAsync(user, "DefaultPassword123!", model.NewPassword);

                    if (result.Succeeded)
                    {
                        // Sign the user in after resetting the password
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    // If default password is not used, redirect or handle accordingly
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }

    }
}


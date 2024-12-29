using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PGManagementService.Data.DTO;
using PGManagementService.Interfaces;
using PGManagementService.Models;

namespace PGManagementService.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : Controller
    {

        private readonly ILogger<RegistrationController> _logger;
        private readonly IRegistrationBL _registrationBL;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IValidator<LoginRequest> _loginValidator;

        public RegistrationController(IRegistrationBL registrationBL, ILogger<RegistrationController> logger, SignInManager<ApplicationUser> signInManager, IValidator<LoginRequest> loginValidator)
        {
            _registrationBL = registrationBL;
            _logger = logger;
            _signInManager = signInManager;
            _loginValidator = loginValidator;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var token = await _registrationBL.Login(loginRequest);

            if (token != null)
            {
               
                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    token =  token
                });
            }
            else
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid username or password",
                    returnUrl = "/login"
                });
            }
        }

        [HttpPost("ForgotPassword")]
        public ActionResult ForgotPassword([FromBody] string email)
        {
            var validator = new InlineValidator<string>();
            validator.RuleFor(x => x)
                     .NotEmpty().WithMessage("Email is required.")
                     .EmailAddress().WithMessage("Invalid email format.");

            var result = validator.Validate(email);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            var isOtpGenerated =_registrationBL.ForgotPassword(email).Result;

            if (isOtpGenerated)
            {

                return Ok(new
                {
                    success = true,
                    message = "Otp generated"
                });
            }
            else
            {
                return NotFound(new
                {
                    success = false,
                    message = "Invalid email"
                });
            }
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetRequest)
        {
            var isResetted = await _registrationBL.ResetPassword(resetRequest);

            if (isResetted)
            {

                return Ok(new
                {
                    success = true,
                    message = "password resetted"
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid Email or Invalid/Expired OTP"
                });
            }
        }

    }
}

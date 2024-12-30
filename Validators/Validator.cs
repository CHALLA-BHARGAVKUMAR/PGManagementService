using FluentValidation;
using PGManagementService.Data.DTO;

namespace PGManagementService.Validators
{
    public class RoomRequestValidator : AbstractValidator<RoomRequest>
    {
        public RoomRequestValidator()
        {
            RuleFor(x => x.RoomNo)
            .NotEmpty().WithMessage("Room number is required.");

            RuleFor(x => x.RoomType)
                .IsInEnum().WithMessage("Invalid room type.");
        }
    }

    public class MemberRequestValidator : AbstractValidator<MemberRequest>
    {
        public MemberRequestValidator()
        {
            RuleFor(x => x.FullName)
             .NotEmpty().WithMessage("Name is required.")
             .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Mobile number is required.")
                .Matches(@"^\d{10}$").WithMessage("Invalid mobile number format. Please enter 10 digits.");
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }

    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            // NewPassword validation (e.g., minimum length of 8 characters)
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            // ConfirmPassword validation: must match NewPassword
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.NewPassword).WithMessage("Confirm password must match the new password.");

            // Otp validation: should not be empty
            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP is required.")
                .Length(6).WithMessage("OTP must be exactly 6 digits.");
        }
    }

    public class PaginationRequestDtoValidator : AbstractValidator<PaginationRequestDto>
    {
        public PaginationRequestDtoValidator()
        {
            // Validate PageNumber (must be greater than or equal to 1)
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1");

            // Validate PageSize (must be greater than or equal to 1 and optionally less than a max limit)
            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be greater than or equal to 1")
                .LessThanOrEqualTo(100)  // Example of setting a maximum page size
                .WithMessage("Page size must be less than or equal to 100");

            // Validate SortBy (if provided, ensure it is not empty or null)
            RuleFor(x => x.SortBy)
                .NotEmpty()
                .When(x => !string.IsNullOrEmpty(x.SortBy)) // Ensure SortBy is not empty if it's provided
                .WithMessage("SortBy cannot be empty.");

            // Validate SortDescending (boolean, so no specific validation needed, but you can add logic if necessary)
            RuleFor(x => x.SortDescending)
                .NotNull()
                .WithMessage("SortDescending cannot be null.");
        }
    }

}

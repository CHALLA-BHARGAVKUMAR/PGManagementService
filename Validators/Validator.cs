using FluentValidation;
using PGManagementService.Data.DTO;
  
namespace PGManagementService.Validators
{
    public class RoomRequestValidator: AbstractValidator<RoomRequest>
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


}

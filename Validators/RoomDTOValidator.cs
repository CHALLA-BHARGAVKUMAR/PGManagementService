using FluentValidation;
using PGManagementService.Data.DTO;

namespace PGManagementService.Validators
{
    public class RoomDTOValidator: AbstractValidator<RoomDTO>
    {
        public RoomDTOValidator()
        {
            RuleFor(x => x.RoomNo)
            .NotEmpty().WithMessage("Room number is required.");

            RuleFor(x => x.RoomType)
                .IsInEnum().WithMessage("Invalid room type.");
        }
    }
}

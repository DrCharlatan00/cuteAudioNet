using cuteAudioNet.APIModels.DTO;
using FluentValidation;

namespace cuteAudioNet.APIModels.Validators
{
    public class ValidatorTrack : AbstractValidator<DTOTrack>
    {
        public ValidatorTrack()
        {
            RuleFor(x => x.Name)
                .NotNull().WithErrorCode("NAME_NULL").WithMessage("Name is not")
                .NotEmpty().WithErrorCode("NAME_EMPTY").WithMessage("Name can't be null")
                .MinimumLength(3).WithErrorCode("NAME_MIN_ERROR").WithMessage("Name can't be lower 3");

            RuleFor(x => x.Genre)
                .NotEmpty().WithErrorCode("GENRE_NULL").WithMessage("Genre track can't be null, use zero for none");

            RuleFor(x => x.AlbumID)
                .NotNull().WithErrorCode("ALBUM_NULL").WithMessage("Album id can't be null")
                .NotEmpty().WithErrorCode("NAME_EMPTY").WithMessage("Name can't be null");


        }
    }
}

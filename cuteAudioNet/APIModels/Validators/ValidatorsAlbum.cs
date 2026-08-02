using cuteAudioNet.APIModels.DTO.Albums;
using FluentValidation;

namespace cuteAudioNet.APIModels.Validators
{
    public class ValidatorCreateAlbum : AbstractValidator<DTOCreateAlbum>
    {
        public ValidatorCreateAlbum()
        {
            RuleFor(x => x.Name)
                .NotNull().WithErrorCode("NAME_NULL").WithMessage("Name is not")
                .NotEmpty().WithErrorCode("NAME_EMPTY").WithMessage("Name can't be null")
                .MinimumLength(3).WithErrorCode("NAME_MIN_ERROR").WithMessage("Name can't be lower 3");

            RuleFor(x => x.IdArtist)
                .NotEmpty().WithErrorCode("ID_ARTIST_IS_EMPTY").WithMessage("Id Artist can't be empty");
        }
    }

    public class ValidatorUpdateAlbum: AbstractValidator<DTOUpdateAlbum>
    {
        public ValidatorUpdateAlbum()
        {
            RuleFor(x => x.AlbumName)
                .NotNull().WithErrorCode("NAME_NULL").WithMessage("Name is not")
                .NotEmpty().WithErrorCode("NAME_EMPTY").WithMessage("Name can't be null")
                .MinimumLength(3).WithErrorCode("NAME_MIN_ERROR").WithMessage("Name can't be lower 3");

        }
    }

}

using cuteAudioNet.APIModels.DTO.Artists;
using FluentValidation;

namespace cuteAudioNet.APIModels.Validators
{
    public class ValidatorsArtist : AbstractValidator<DTOArtist>
    {
        public ValidatorsArtist()
        {
            RuleFor(x => x.Name)
                .NotNull().NotEmpty()
                .WithMessage("Name can't be null").WithErrorCode("NAME_IS_NULL");

            RuleFor(x => x.NickName)
                 .NotNull().NotEmpty()
                 .WithMessage("NIck can't be null").WithErrorCode("NICK_IS_NULL");
        }
    }
}

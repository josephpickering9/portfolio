using FluentValidation;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Validators;

public sealed class CreateTagValidator : AbstractValidator<CreateTag>
{
    private const int MaxTitleLength = 200;
    private const int MaxIconLength = 200;
    private const string HexColourPattern = "^#[0-9A-Fa-f]{6}$";

    public CreateTagValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorCodes.TitleRequired)
            .MaximumLength(MaxTitleLength).WithMessage(ErrorCodes.TitleTooLong);
        RuleFor(x => x.Icon)
            .MaximumLength(MaxIconLength).WithMessage(ErrorCodes.IconTooLong)
            .When(x => x.Icon is not null);
        RuleFor(x => x.CustomColour)
            .Matches(HexColourPattern).WithMessage(ErrorCodes.CustomColourInvalid)
            .When(x => x.CustomColour is not null);
    }
}

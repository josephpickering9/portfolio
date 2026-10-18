using FluentValidation;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Validators;

public sealed class CreateCompanyValidator : AbstractValidator<CreateCompany>
{
    private const int MaxNameLength = 200;
    private const int MaxDescriptionLength = 5000;
    private const int MaxWebsiteLength = 500;

    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorCodes.NameRequired)
            .MaximumLength(MaxNameLength).WithMessage(ErrorCodes.NameTooLong);
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorCodes.DescriptionRequired)
            .MaximumLength(MaxDescriptionLength).WithMessage(ErrorCodes.DescriptionTooLong);
        RuleFor(x => x.Website)
            .MaximumLength(MaxWebsiteLength).WithMessage(ErrorCodes.WebsiteTooLong)
            .When(x => x.Website is not null);
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage(ErrorCodes.EndDateBeforeStartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}

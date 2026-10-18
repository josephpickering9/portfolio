using FluentValidation;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Validators;

public sealed class CreateProjectValidator : AbstractValidator<CreateProject>
{
    private const int MaxTitleLength = 200;
    private const int MaxShortDescriptionLength = 500;
    private const int MaxDescriptionLength = 50000;
    private const int MaxCaseStudyFieldLength = 2000;
    private const int MaxWebsiteLength = 500;
    private const int MaxRepositoryTitleLength = 200;
    private const int MaxRepositoryUrlLength = 500;
    private const int MaxStatFieldLength = 100;

    public CreateProjectValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorCodes.TitleRequired)
            .MaximumLength(MaxTitleLength).WithMessage(ErrorCodes.TitleTooLong);
        RuleFor(x => x.ShortDescription)
            .NotEmpty().WithMessage(ErrorCodes.ShortDescriptionRequired)
            .MaximumLength(MaxShortDescriptionLength).WithMessage(ErrorCodes.ShortDescriptionTooLong);
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorCodes.DescriptionRequired)
            .MaximumLength(MaxDescriptionLength).WithMessage(ErrorCodes.DescriptionTooLong);
        RuleFor(x => x.ProblemStatement)
            .MaximumLength(MaxCaseStudyFieldLength).WithMessage(ErrorCodes.ProblemStatementTooLong)
            .When(x => x.ProblemStatement is not null);
        RuleFor(x => x.KeyDecision)
            .MaximumLength(MaxCaseStudyFieldLength).WithMessage(ErrorCodes.KeyDecisionTooLong)
            .When(x => x.KeyDecision is not null);
        RuleFor(x => x.Outcome)
            .MaximumLength(MaxCaseStudyFieldLength).WithMessage(ErrorCodes.OutcomeTooLong)
            .When(x => x.Outcome is not null);
        RuleFor(x => x.Website)
            .MaximumLength(MaxWebsiteLength).WithMessage(ErrorCodes.WebsiteTooLong)
            .When(x => x.Website is not null);
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage(ErrorCodes.EndDateBeforeStartDate)
            .When(x => x.EndDate.HasValue);
        RuleForEach(x => x.Repositories).ChildRules(repository =>
        {
            repository.RuleFor(r => r.Title)
                .NotEmpty().WithMessage(ErrorCodes.RepositoryTitleRequired)
                .MaximumLength(MaxRepositoryTitleLength).WithMessage(ErrorCodes.RepositoryTitleTooLong);
            repository.RuleFor(r => r.Url)
                .NotEmpty().WithMessage(ErrorCodes.RepositoryUrlRequired)
                .MaximumLength(MaxRepositoryUrlLength).WithMessage(ErrorCodes.RepositoryUrlTooLong);
        });
        RuleForEach(x => x.Stats).ChildRules(stat =>
        {
            stat.RuleFor(s => s.Label)
                .NotEmpty().WithMessage(ErrorCodes.StatLabelRequired)
                .MaximumLength(MaxStatFieldLength).WithMessage(ErrorCodes.StatLabelTooLong);
            stat.RuleFor(s => s.Value)
                .NotEmpty().WithMessage(ErrorCodes.StatValueRequired)
                .MaximumLength(MaxStatFieldLength).WithMessage(ErrorCodes.StatValueTooLong);
        });
    }
}

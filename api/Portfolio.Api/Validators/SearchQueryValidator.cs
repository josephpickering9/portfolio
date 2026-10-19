using FluentValidation;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Validators;

public sealed class SearchQueryValidator : AbstractValidator<SearchQuery>
{
    private const int MaxSearchLength = 100;

    public SearchQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(MaxSearchLength).WithMessage(ErrorCodes.SearchTooLong)
            .When(x => x.Search is not null);
    }
}

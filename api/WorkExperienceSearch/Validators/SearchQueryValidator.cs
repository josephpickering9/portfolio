using FluentValidation;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Validators;

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

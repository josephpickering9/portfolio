using FluentValidation;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Validators;

public sealed class VertexQueryRequestValidator : AbstractValidator<VertexQueryRequest>
{
    private const int MaxQueryLength = 500;

    public VertexQueryRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage(ErrorCodes.QueryRequired)
            .MaximumLength(MaxQueryLength).WithMessage(ErrorCodes.QueryTooLong);
    }
}

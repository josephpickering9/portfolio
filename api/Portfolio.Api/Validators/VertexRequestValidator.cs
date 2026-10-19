using FluentValidation;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Validators;

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

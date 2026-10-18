using FluentValidation;
using Work_Experience_Search.Requests;

namespace Work_Experience_Search.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProject>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(50000);
        RuleFor(x => x.ProblemStatement).MaximumLength(2000).When(x => x.ProblemStatement != null);
        RuleFor(x => x.KeyDecision).MaximumLength(2000).When(x => x.KeyDecision != null);
        RuleFor(x => x.Outcome).MaximumLength(2000).When(x => x.Outcome != null);
        RuleFor(x => x.Website).MaximumLength(500).When(x => x.Website != null);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).When(x => x.EndDate.HasValue);
        RuleForEach(x => x.Repositories).ChildRules(repo =>
        {
            repo.RuleFor(r => r.Title).NotEmpty().MaximumLength(200);
            repo.RuleFor(r => r.Url).NotEmpty().MaximumLength(500);
        });
        RuleForEach(x => x.Stats).ChildRules(stat =>
        {
            stat.RuleFor(s => s.Label).NotEmpty().MaximumLength(100);
            stat.RuleFor(s => s.Value).NotEmpty().MaximumLength(100);
        });
    }
}

using FluentValidation;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;
using Work_Experience_Search.Validators;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Contract;

public class ValidatorErrorCodeTests
{
    private static readonly string TooLong = new('x', 60000);

    public static IEnumerable<object[]> InvalidRequests =>
    [
        [new CreateProjectValidator(), new CreateProject
        {
            Title = TooLong, ShortDescription = TooLong, Description = TooLong,
            ProblemStatement = TooLong, KeyDecision = TooLong, Outcome = TooLong, Website = TooLong,
            StartDate = new DateOnly(2022, 1, 1), EndDate = new DateOnly(2021, 1, 1),
            Repositories = [new CreateProjectRepository { Title = TooLong, Url = "" }],
            Stats = [new CreateProjectStat { Label = "", Value = TooLong }]
        }],
        [new CreateProjectValidator(), new CreateProject { Title = "", ShortDescription = "", Description = "" }],
        [new CreateCompanyValidator(), new CreateCompany
        {
            Name = TooLong, Description = TooLong, Website = TooLong,
            StartDate = new DateOnly(2022, 1, 1), EndDate = new DateOnly(2021, 1, 1)
        }],
        [new CreateCompanyValidator(), new CreateCompany { Name = "", Description = "" }],
        [new CreateTagValidator(), new CreateTag { Title = TooLong, Type = default, Icon = TooLong, CustomColour = "red" }],
        [new CreateTagValidator(), new CreateTag { Title = "", Type = default }],
        [new VertexQueryRequestValidator(), new VertexQueryRequest(TooLong)],
        [new VertexQueryRequestValidator(), new VertexQueryRequest("")],
    ];

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public void Validator_EveryFailureMessageIsAnErrorCode(IValidator validator, object request)
    {
        var result = validator.Validate(new ValidationContext<object>(request));

        Assert.False(result.IsValid);
        Assert.All(result.Errors, failure => Assert.Contains(failure.ErrorMessage, ErrorCodes.All));
    }
}

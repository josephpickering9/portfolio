using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Contract;

public partial class ErrorContractTests
{
    [Theory]
    [InlineData(ErrorCodes.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorCodes.ProjectNotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorCodes.CompanyNotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorCodes.TagNotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorCodes.FileNotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorCodes.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ErrorCodes.ProjectAlreadyExists, StatusCodes.Status409Conflict)]
    [InlineData(ErrorCodes.CompanyAlreadyExists, StatusCodes.Status409Conflict)]
    [InlineData(ErrorCodes.TagAlreadyExists, StatusCodes.Status409Conflict)]
    [InlineData(ErrorCodes.Unauthorized, StatusCodes.Status401Unauthorized)]
    [InlineData(ErrorCodes.Forbidden, StatusCodes.Status403Forbidden)]
    [InlineData(ErrorCodes.RateLimited, StatusCodes.Status429TooManyRequests)]
    [InlineData(ErrorCodes.InternalError, StatusCodes.Status500InternalServerError)]
    [InlineData(ErrorCodes.FileUploadFailed, StatusCodes.Status500InternalServerError)]
    [InlineData(ErrorCodes.UpstreamUnavailable, StatusCodes.Status502BadGateway)]
    [InlineData(ErrorCodes.UpstreamEmptyResponse, StatusCodes.Status502BadGateway)]
    [InlineData(ErrorCodes.ImageOptimisationUnavailable, StatusCodes.Status502BadGateway)]
    [InlineData(ErrorCodes.BadRequest, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorCodes.FileRequired, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorCodes.TitleRequired, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorCodes.QueryTooLong, StatusCodes.Status400BadRequest)]
    public void ErrorStatusCodes_MapsCodeToStatus(string errorCode, int expectedStatus)
    {
        Assert.Equal(expectedStatus, ErrorStatusCodes.For(errorCode));
    }

    [Fact]
    public void ErrorCodes_AreUniqueSnakeCase()
    {
        var values = typeof(ErrorCodes).GetFields()
            .Where(field => field.IsLiteral)
            .Select(field => (string)field.GetRawConstantValue()!)
            .ToList();

        Assert.Equal(values.Count, values.Distinct().Count());
        Assert.All(values, value => Assert.Matches(SnakeCase(), value));
        Assert.Equal(values.Count, ErrorCodes.All.Count);
    }

    [Fact]
    public void ToErrorResponse_WrapsCodeInApiErrorDtoWithMappedStatus()
    {
        var response = ErrorCodes.ProjectNotFound.ToErrorResponse();

        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        Assert.Equal(new ApiErrorDto(ErrorCodes.ProjectNotFound), response.Value);
    }

    [Fact]
    public void ToResponse_SuccessWithData_ReturnsOk()
    {
        var response = "value".ToOk().ToResponse();

        var ok = Assert.IsType<OkObjectResult>(response);
        Assert.Equal("value", ok.Value);
    }

    [Fact]
    public void ToResponse_Failure_ReturnsErrorBody()
    {
        Result<string> result = ErrorCodes.UpstreamUnavailable.ToFail();

        var response = Assert.IsType<ObjectResult>(result.ToResponse());

        Assert.Equal(StatusCodes.Status502BadGateway, response.StatusCode);
        Assert.Equal(new ApiErrorDto(ErrorCodes.UpstreamUnavailable), response.Value);
    }

    [Fact]
    public void ToFail_BlankCode_Throws()
    {
        Assert.Throws<ArgumentException>(() => " ".ToFail());
    }

    [GeneratedRegex("^[a-z]+(_[a-z]+)*$")]
    private static partial Regex SnakeCase();
}

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Api.Types;
using Xunit;

namespace Portfolio.Api.Tests.Unit.Contract;

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
    public async Task ToResponse_SuccessWithValue_ReturnsOk()
    {
        var response = await Task.FromResult("value".ToOk()).ToResponse();

        var ok = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Equal("value", ok.Value);
    }

    [Fact]
    public async Task ToResponse_Failure_ReturnsErrorBody()
    {
        Result<string> result = ErrorCodes.UpstreamUnavailable.ToFail();

        var response = await Task.FromResult(result).ToResponse();

        var error = Assert.IsType<ObjectResult>(response.Result);
        Assert.Equal(StatusCodes.Status502BadGateway, error.StatusCode);
        Assert.Equal(new ApiErrorDto(ErrorCodes.UpstreamUnavailable), error.Value);
    }

    [Fact]
    public async Task ToCreatedResponse_Success_ReturnsCreatedAtAction()
    {
        var response = await Task.FromResult("value".ToOk()).ToCreatedResponse("GetById", value => new { id = value });

        var created = Assert.IsType<CreatedAtActionResult>(response.Result);
        Assert.Equal("GetById", created.ActionName);
        Assert.Equal("value", created.RouteValues?["id"]);
        Assert.Equal("value", created.Value);
    }

    [Fact]
    public async Task ToCreatedResponse_Failure_ReturnsErrorBody()
    {
        Result<string> result = ErrorCodes.ProjectAlreadyExists.ToFail();

        var response = await Task.FromResult(result).ToCreatedResponse("GetById", value => new { id = value });

        var error = Assert.IsType<ObjectResult>(response.Result);
        Assert.Equal(StatusCodes.Status409Conflict, error.StatusCode);
    }

    [Fact]
    public async Task ToNoContentOrError_Success_ReturnsNoContent()
    {
        var response = await Task.FromResult(Result.Ok()).ToNoContentOrError();

        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async Task ToNoContentOrError_Failure_ReturnsErrorBody()
    {
        Result result = ErrorCodes.TagNotFound.ToFail();

        var response = await Task.FromResult(result).ToNoContentOrError();

        var error = Assert.IsType<ObjectResult>(response);
        Assert.Equal(StatusCodes.Status404NotFound, error.StatusCode);
        Assert.Equal(new ApiErrorDto(ErrorCodes.TagNotFound), error.Value);
    }

    [Fact]
    public void ToResultOrFail_Null_ReturnsFailure()
    {
        string? missing = null;

        Assert.Equal(ErrorCodes.NotFound, missing.ToResultOrFail(ErrorCodes.NotFound).ExpectFailure());
    }

    [Fact]
    public void ToResultOrFail_Value_ReturnsSuccess()
    {
        Assert.Equal("value", "value".ToResultOrFail(ErrorCodes.NotFound).ExpectSuccess());
    }

    [Fact]
    public void ToFail_BlankCode_Throws()
    {
        Assert.Throws<ArgumentException>(() => " ".ToFail());
    }

    [GeneratedRegex("^[a-z]+(_[a-z]+)*$")]
    private static partial Regex SnakeCase();
}

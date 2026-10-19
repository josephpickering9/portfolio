using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Exceptions;
using Portfolio.Api.Extensions;
using Portfolio.Api.Types;
using Xunit;

namespace Portfolio.Api.Tests.Unit.Contract;

public class GlobalExceptionHandlerTests
{
    public static IEnumerable<object[]> Exceptions =>
    [
        [new NotFoundException(), StatusCodes.Status404NotFound, ErrorCodes.NotFound],
        [new ConflictException(), StatusCodes.Status409Conflict, ErrorCodes.Conflict],
        [new InvalidOperationException("boom"), StatusCodes.Status500InternalServerError, ErrorCodes.InternalError],
    ];

    [Theory]
    [MemberData(nameof(Exceptions))]
    public async Task TryHandleAsync_WritesErrorCodeBody(Exception exception, int expectedStatus, string expectedCode)
    {
        var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };

        var handled = await new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance)
            .TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(expectedStatus, context.Response.StatusCode);
        context.Response.Body.Position = 0;
        using var body = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal(expectedCode, body.RootElement.GetProperty("error").GetString());
    }
}

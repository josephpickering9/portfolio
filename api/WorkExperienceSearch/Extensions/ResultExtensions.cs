using Microsoft.AspNetCore.Mvc;
using Work_Experience_Search.Services.Image;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToResponse<T>(this Result<T> result)
    {
        if (result.Error is { } errorCode) return errorCode.ToErrorResponse();

        return result.Data is null ? new NoContentResult() : new OkObjectResult(result.Data);
    }

    public static ActionResult ToFileResponse(this Result<ImageData> result) =>
        result is { IsSuccess: true, Data: not null }
            ? new FileContentResult(result.Data.File, result.Data.ContentType)
            : (result.Error ?? ErrorCodes.FileNotFound).ToErrorResponse();

    public static ObjectResult ToErrorResponse(this string errorCode) =>
        new(new ApiErrorDto(errorCode)) { StatusCode = ErrorStatusCodes.For(errorCode) };

    public static Task WriteErrorAsync(this HttpResponse response, string errorCode, CancellationToken cancellationToken = default)
    {
        response.StatusCode = ErrorStatusCodes.For(errorCode);
        return response.WriteAsJsonAsync(new ApiErrorDto(errorCode), cancellationToken);
    }
}

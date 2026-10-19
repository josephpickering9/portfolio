using Microsoft.AspNetCore.Mvc;
using Work_Experience_Search.Services.Image;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Extensions;

public static class ResultExtensions
{
    public static async Task<ActionResult<T>> ToResponse<T>(this Task<Result<T>> task)
    {
        var result = await task;
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : result.Error!.ToErrorResponse();
    }

    public static async Task<ActionResult<T>> ToCreatedResponse<T>(
        this Task<Result<T>> task, string actionName, Func<T, object> routeValues)
    {
        var result = await task;
        return result is { IsSuccess: true, Value: { } value }
            ? new CreatedAtActionResult(actionName, null, routeValues(value), value)
            : (result.Error ?? ErrorCodes.InternalError).ToErrorResponse();
    }

    public static async Task<IActionResult> ToNoContentOrError(this Task<Result> task)
    {
        var result = await task;
        return result.IsSuccess ? new NoContentResult() : result.Error!.ToErrorResponse();
    }

    public static async Task<IActionResult> ToFileResponse(this Task<Result<ImageData>> task)
    {
        var result = await task;
        return result is { IsSuccess: true, Value: { } image }
            ? new FileContentResult(image.File, image.ContentType)
            : (result.Error ?? ErrorCodes.FileNotFound).ToErrorResponse();
    }

    public static ObjectResult ToErrorResponse(this string errorCode) =>
        new(new ApiErrorDto(errorCode)) { StatusCode = ErrorStatusCodes.For(errorCode) };

    public static Task WriteErrorAsync(this HttpResponse response, string errorCode, CancellationToken cancellationToken = default)
    {
        response.StatusCode = ErrorStatusCodes.For(errorCode);
        return response.WriteAsJsonAsync(new ApiErrorDto(errorCode), cancellationToken);
    }
}

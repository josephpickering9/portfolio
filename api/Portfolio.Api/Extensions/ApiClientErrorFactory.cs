using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Portfolio.Api.Types;

namespace Portfolio.Api.Extensions;

public sealed class ApiClientErrorFactory : IClientErrorFactory
{
    public IActionResult GetClientError(ActionContext actionContext, IClientErrorActionResult clientError) =>
        ErrorCodeFor(clientError.StatusCode).ToErrorResponse();

    private static string ErrorCodeFor(int? statusCode) => statusCode switch
    {
        StatusCodes.Status401Unauthorized => ErrorCodes.Unauthorized,
        StatusCodes.Status403Forbidden => ErrorCodes.Forbidden,
        StatusCodes.Status404NotFound => ErrorCodes.NotFound,
        StatusCodes.Status409Conflict => ErrorCodes.Conflict,
        >= StatusCodes.Status500InternalServerError => ErrorCodes.InternalError,
        _ => ErrorCodes.BadRequest,
    };
}

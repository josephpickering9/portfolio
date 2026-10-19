using Work_Experience_Search.Types;

namespace Work_Experience_Search.Extensions;

public static class ErrorStatusCodes
{
    public static int For(string errorCode) => errorCode switch
    {
        ErrorCodes.NotFound
            or ErrorCodes.ProjectNotFound
            or ErrorCodes.CompanyNotFound
            or ErrorCodes.TagNotFound
            or ErrorCodes.FileNotFound => StatusCodes.Status404NotFound,

        ErrorCodes.Conflict
            or ErrorCodes.ProjectAlreadyExists
            or ErrorCodes.CompanyAlreadyExists
            or ErrorCodes.TagAlreadyExists => StatusCodes.Status409Conflict,

        ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorCodes.Forbidden => StatusCodes.Status403Forbidden,
        ErrorCodes.RateLimited => StatusCodes.Status429TooManyRequests,

        ErrorCodes.InternalError
            or ErrorCodes.FileUploadFailed => StatusCodes.Status500InternalServerError,

        ErrorCodes.UpstreamUnavailable
            or ErrorCodes.UpstreamEmptyResponse
            or ErrorCodes.ImageOptimisationUnavailable => StatusCodes.Status502BadGateway,

        _ => StatusCodes.Status400BadRequest,
    };
}

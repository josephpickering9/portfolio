using Portfolio.Api.Types;

namespace Portfolio.Api.Tests;

public static class ResultAssertions
{
    public static T ExpectSuccess<T>(this Result<T> result)
    {
        if (result.IsSuccess && result.Value is not null)
            return result.Value;

        throw new InvalidOperationException(result.IsSuccess
            ? "Expected success with a value but was null."
            : $"Expected success but was failure: {result.Error}");
    }

    public static void ExpectSuccess(this Result result)
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException($"Expected success but was failure: {result.Error}");
    }

    public static string ExpectFailure<T>(this Result<T> result) =>
        result.IsSuccess ? throw new InvalidOperationException("Expected failure but got success.") : result.Error!;

    public static string ExpectFailure(this Result result) =>
        result.IsSuccess ? throw new InvalidOperationException("Expected failure but got success.") : result.Error!;
}

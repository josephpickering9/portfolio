using Work_Experience_Search.Types;

namespace WorkExperienceSearchTests.Tests;

public static class ResultAssertions
{
    public static T ExpectSuccess<T>(this Result<T> result)
    {
        if (result.IsSuccess && result.Data is not null)
            return result.Data;

        throw new InvalidOperationException(result.IsSuccess
            ? "Expected success with data but was null."
            : $"Expected success but was failure: {result.Error}");
    }

    public static string ExpectFailure<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Expected failure but got success.");

        return result.Error!;
    }
}

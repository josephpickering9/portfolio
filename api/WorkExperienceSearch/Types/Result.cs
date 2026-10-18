namespace Work_Experience_Search.Types;

public sealed class Result<T>
{
    private Result(T? data, string? error)
    {
        Data = data;
        Error = error;
    }

    public T? Data { get; }
    public string? Error { get; }
    public bool IsSuccess => Error is null;

    public static Result<T> Ok(T data) => new(data, null);

    public static Result<T> Fail(string errorCode) => new(default, errorCode);

    public static implicit operator Result<T>(ResultFailure failure) => Fail(failure.ErrorCode);
}

public sealed record ResultFailure
{
    public ResultFailure(string errorCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorCode);
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }
}

public static class ResultFactory
{
    public static Result<T> ToOk<T>(this T value) => Result<T>.Ok(value);

    public static ResultFailure ToFail(this string errorCode) => new(errorCode);

    public static Result<T> ToResultOrFail<T>(this T? value, string errorCode) where T : class =>
        value is null ? Result<T>.Fail(errorCode) : Result<T>.Ok(value);
}

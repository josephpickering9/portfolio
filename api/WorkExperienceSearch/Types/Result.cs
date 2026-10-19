namespace Work_Experience_Search.Types;

public readonly record struct Result
{
    private Result(string? error) => Error = error;

    public string? Error { get; }
    public bool IsSuccess => Error is null;

    public static Result Ok() => new(null);

    public static Result Fail(string errorCode) => new(errorCode);

    public static implicit operator Result(ResultFailure failure) => Fail(failure.ErrorCode);
}

public readonly record struct Result<T>
{
    private Result(T? value, string? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess => Error is null;

    public static Result<T> Ok(T value) => new(value, null);

    public static Result<T> Fail(string errorCode) => new(default, errorCode);

    public static implicit operator Result<T>(ResultFailure failure) => Fail(failure.ErrorCode);
}

public readonly record struct ResultFailure
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

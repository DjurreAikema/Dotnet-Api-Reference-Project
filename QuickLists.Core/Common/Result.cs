namespace QuickLists.Core.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error Error { get; }

    private Result(bool isSuccess, T? value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, Error.None);

    public static Result<T> Failure(Error error) => new(false, default, error);

    public static Result<T> NotFound(string message = "Resource not found") => Failure(Error.NotFound(message));

    public static Result<T> Forbidden(string message = "Access denied") => Failure(Error.Forbidden(message));
}

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string message) => new("NotFound", message);

    public static Error Forbidden(string message) => new("Forbidden", message);

    public static Error Conflict(string message) => new("Conflict", message);

    public static Error Validation(string message) => new("Validation", message);
}
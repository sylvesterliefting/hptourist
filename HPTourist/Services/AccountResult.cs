namespace HPTourist.Services;

public class AccountResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public AccountResult() { }

    public AccountResult(bool success, string? errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }

    public static AccountResult Ok() => new(true, null);
    public static AccountResult Fail(string message) => new(false, message);
}

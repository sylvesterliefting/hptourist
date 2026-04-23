namespace HPTourist.Services;

public sealed record AccountResult(bool Success, string? ErrorMessage)
{
    public static AccountResult Ok() => new(true, null);

    public static AccountResult Fail(string message) => new(false, message);
}

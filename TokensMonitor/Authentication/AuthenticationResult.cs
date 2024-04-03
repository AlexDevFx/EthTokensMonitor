namespace TokensMonitor.Authentication;

public record AuthenticationResult(string? ErrorMessage, NewTokenRequest? TokenRequest)
{
    public bool Success = string.IsNullOrEmpty(ErrorMessage) && string.IsNullOrWhiteSpace(ErrorMessage);
}
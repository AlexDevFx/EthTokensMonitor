namespace TokensMonitor.Errors;

public class ErrorResponse(string error)
{
    public string Error { get; private set; } = error;
}
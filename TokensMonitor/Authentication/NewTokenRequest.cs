namespace TokensMonitor.Authentication;

public record NewTokenRequest(string Address, string Signature, DateTime Issued, DateTime ExpirationTime);
namespace TokensMonitor.Authentication;

public record AuthenticationRequest(string? EncodedMessage, string? Signature)
{
    public string? Validate()
    {
        if (string.IsNullOrEmpty(EncodedMessage) || string.IsNullOrWhiteSpace(EncodedMessage))
        {
            return "Message body cannot be empty";
        }
        
        if (string.IsNullOrEmpty(Signature) || string.IsNullOrWhiteSpace(Signature))
        {
            return "You have to sign the message";
        }

        return null;
    }
}
namespace TokensMonitor.Authentication;

public interface IUserContext
{
    string UserId { get; internal set; }
    string Address { get; internal set; }
}
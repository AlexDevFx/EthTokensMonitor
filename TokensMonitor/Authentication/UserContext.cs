namespace TokensMonitor.Authentication;

public class UserContext: IUserContext
{
    public string UserId { get; set; }
    public string Address { get; set; }
}
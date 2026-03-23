namespace BankApp.Cli.Application.Model.Impl;

public class UserContext : IUserContext
{
    public Guid? CurrentSession { get; private set; }

    public void Login(Guid sessionId)
    {
        CurrentSession = sessionId;
    }

    public void Logout()
    {
        CurrentSession = null;
    }
}
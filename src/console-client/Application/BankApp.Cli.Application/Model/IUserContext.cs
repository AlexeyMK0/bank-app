namespace BankApp.Cli.Application.Model;

public interface IUserContext
{
    Guid? CurrentSession { get; }

    void Login(Guid sessionId);

    void Logout();
}
namespace BankApp.Cli.Application.Model;

public interface IUserContext
{
    Guid? CurrentSession { get; set; }

    bool IsQuit { get; set; }
}
namespace BankApp.Cli.Application.Model;

public interface ISessionKeeper
{
    Guid? CurrentSession { get; set; }
}
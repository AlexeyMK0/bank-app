namespace BankApp.Cli.Application.Model.Impl;

public class UserContext : IUserContext
{
    public Guid? CurrentSession { get; set; }

    public bool IsQuit { get; set; } = false;
}
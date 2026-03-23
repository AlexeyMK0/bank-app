namespace BankApp.Cli.Presentation.Cli;

public class AppLifetimeManager
{
    public bool IsExisting { get; private set; } = true;

    public void Stop()
    {
        IsExisting = false;
    }
}
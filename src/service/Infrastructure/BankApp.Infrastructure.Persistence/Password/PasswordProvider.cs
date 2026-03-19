using Abstractions.Password;
using Microsoft.Extensions.Options;

namespace Lab1.Infrastructure.Persistence.Password;

public sealed class PasswordProvider : IPasswordProvider
{
    public PasswordProvider(IOptions<PasswordOptions> options)
    {
        Password = options.Value.Password;
    }

    public string Password { get; }
}
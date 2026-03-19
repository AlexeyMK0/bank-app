using System.ComponentModel.DataAnnotations;

namespace Lab1.Infrastructure.Persistence.Password;

public sealed class PasswordOptions
{
    [Required]
    [MinLength(1)]
    public required string Password { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Lab1.Application.Model;

public class DefaultIsolationLevel
{
    [Required]
    public IsolationLevel IsolationLevel { get; init; }
}
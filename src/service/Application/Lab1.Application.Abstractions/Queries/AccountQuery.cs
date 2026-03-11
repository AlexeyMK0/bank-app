using Lab1.Domain.Accounts;
using SourceKit.Generators.Builder.Annotations;

namespace Abstractions.Queries;

[GenerateBuilder]
public partial record AccountQuery(
    string? KeyCursor,
    [RequiredValue] int PageSize,
    AccountId[] AccountIds);
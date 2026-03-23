using BankApp.Cli.Application.Contracts.Model;
using BankApp.Cli.Application.Models;

namespace BankApp.Cli.Application.Mappers;

public static class OperationRecordMapper
{
    public static OperationRecordDto MapToDto(this OperationRecord record)
    {
        return new OperationRecordDto(record.Type, record.Time, record.AccountId, record.SessionId);
    }
}
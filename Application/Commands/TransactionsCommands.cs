using Application.ViewModels;
using MediatR;
using System;

namespace Application.Commands
{
    public record CreateTransactionCommand(Guid AccountId, decimal Amount, DateTime TransactionDate, string Description) : IRequest<TransactionViewModel>;

    public record UpdateTransactionCommand(Guid Id, Guid AccountId, decimal Amount, DateTime TransactionDate, string Description) : IRequest<TransactionViewModel>;
}


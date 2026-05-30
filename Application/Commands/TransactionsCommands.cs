using Application.ViewModels;
using MediatR;

public record CreateTransactionCommand(Guid AccountId, decimal Amount, string Description) : IRequest<TransactionViewModel>;


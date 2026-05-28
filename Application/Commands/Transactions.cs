using Domain.Entities;
using MediatR;

public record CreateTransactionCommand(int AccountId, decimal Amount, DateTime Date) : IRequest<Transaction>;


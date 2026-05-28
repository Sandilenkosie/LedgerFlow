using Domain.Entities;
using MediatR;

public record CreateAccountCommand(string AccountNumber) : IRequest<Account>;
public record CloseAccountCommand(int AccountId) : IRequest;


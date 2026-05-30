using Application.ViewModels;
using Domain.Entities;
using MediatR;

public record CreateAccountCommand(Guid UserId, decimal Balance) : IRequest<AccountViewModel>;
public record CloseAccountCommand(int AccountId) : IRequest;


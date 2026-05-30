using Application.ViewModels;
using Domain.Entities;
using MediatR;

namespace Application.Commands;

public record CreateAccountCommand(Guid UserId, decimal Balance, string AccountType) : IRequest<AccountViewModel>;
public record CloseAccountCommand(int AccountId) : IRequest;


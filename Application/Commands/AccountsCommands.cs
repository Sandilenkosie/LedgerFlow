using Application.ViewModels;
using Domain.Entities;
using MediatR;

namespace Application.Commands;

public record CreateAccountCommand(Guid UserId, decimal Balance, string AccountType) : IRequest<AccountViewModel>;
// Update account: only support close operation from UI
public record UpdateAccountCommand(Guid AccountId, bool Close) : IRequest<AccountViewModel>;


using Domain.Entities;
using MediatR;
using Application.ViewModels;

public record RegisterCommand(RegisterViewModel Person) : IRequest<AuthenticationResult>;
public record LoginCommand(LoginViewModel Person) : IRequest<AuthenticationResult>;


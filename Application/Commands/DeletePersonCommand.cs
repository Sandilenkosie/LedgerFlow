using MediatR;
using System;

namespace Application.Commands;

public record DeletePersonCommand(Guid Id) : IRequest<bool>;

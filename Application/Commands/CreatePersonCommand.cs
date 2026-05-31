using Application.ViewModels;
using MediatR;

namespace Application.Commands
{
    public record CreatePersonCommand(PersonViewModel Person) : IRequest<PersonViewModel>;
}

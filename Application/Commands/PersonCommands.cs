using Application.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands
{
    public record PersonCommand(PersonViewModel Person) : IRequest<PersonViewModel>;

}

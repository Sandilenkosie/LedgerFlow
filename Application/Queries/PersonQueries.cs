using Application.ViewModels;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Queries
{
    public record GetPersonsQuery(int Page, int PageSize) : IRequest<IEnumerable<PersonViewModel>>;
    public record GetAccountsQuery() : IRequest<IEnumerable<AccountViewModel>>;
}

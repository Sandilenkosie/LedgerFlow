using Application.Queries;
using Application.ViewModels;
using Domain.Entities;
using Domain.Interfaces;
using AutoMapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Handlers;

public class GetPersonsHandler : IRequestHandler<GetPersonsQuery, IEnumerable<PersonViewModel>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetPersonsHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PersonViewModel>> Handle(GetPersonsQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(request.Page, request.PageSize);

        return _mapper.Map<IEnumerable<PersonViewModel>>(users);
    }
}

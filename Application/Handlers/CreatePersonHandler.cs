using Application.Commands;
using Application.ViewModels;
using Domain.Interfaces;
using MediatR;
using AutoMapper;

namespace Application.Handlers
{
    public class CreatePersonHandler : IRequestHandler<CreatePersonCommand, PersonViewModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreatePersonHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PersonViewModel> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var existing = await _userRepository.GetByUsernameAsync(request.Person.Username);
            if (existing != null) throw new InvalidOperationException("Username already exists.");

            // use a temporary password hash placeholder; password flow should be separate
            var tempHash = Guid.NewGuid().ToString();
            var user = new Domain.Entities.User(request.Person.Username, request.Person.IdNumber, request.Person.Name, tempHash);

            await _userRepository.AddAsync(user);

            return _mapper.Map<PersonViewModel>(user);
        }
    }
}

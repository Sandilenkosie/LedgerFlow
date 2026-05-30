using Application.Queries;
using Application.ViewModels;
using Domain.Entities;
using Domain.Interfaces;
using AutoMapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Commands;

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

public class GetAccountsHandler : IRequestHandler<GetAccountsQuery, IEnumerable<AccountViewModel>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public GetAccountsHandler(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

        public async Task<IEnumerable<AccountViewModel>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<AccountViewModel>>(accounts);
    }
}

public class UpdatePersonsHandler : IRequestHandler<PersonCommand, PersonViewModel>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UpdatePersonsHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PersonViewModel> Handle(PersonCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(request.Person);
        await _userRepository.UpdateAsync(request.Person.Id, user);
        return _mapper.Map<PersonViewModel>(user);
    }
}

public class AddAccountHandler : IRequestHandler<CreateAccountCommand, AccountViewModel>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public AddAccountHandler(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<AccountViewModel> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var newAccountNumber = await _accountRepository.GetNextAccountNumberAsync();
        var account = new Account(request.UserId, newAccountNumber, request.Balance);
        await _accountRepository.AddAsync(account);
        return _mapper.Map<AccountViewModel>(account);
    }
}

public class TransactionHandler : IRequestHandler<CreateTransactionCommand, TransactionViewModel>
{
    private readonly ITransactionRepository _transRepository;
    private readonly IMapper _mapper;

    public TransactionHandler(ITransactionRepository transRepository, IMapper mapper)
    {
        _transRepository = transRepository;
        _mapper = mapper;
    }

    public async Task<TransactionViewModel> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        // create and persist transaction using account id, amount and description
        var transaction = new Transaction(request.AccountId, request.Amount, request.Description);
        await _transRepository.AddAsync(transaction);
        return _mapper.Map<TransactionViewModel>(transaction);
    }
}

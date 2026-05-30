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
        var existing = await _userRepository.GetByIdAsync(request.Person.Id);
        if (existing == null) throw new InvalidOperationException("User not found.");

        existing.Name = request.Person.Name;

        // Preserve existing values when incoming values are null/empty
        var usernameToSet = !string.IsNullOrWhiteSpace(request.Person.Username) ? request.Person.Username : existing.Username;
        if (!string.IsNullOrWhiteSpace(usernameToSet))
        {
            existing.UpdateUsername(usernameToSet);
        }

        var idNumberToSet = !string.IsNullOrWhiteSpace(request.Person.IdNumber) ? request.Person.IdNumber : existing.IdNumber;
        if (!string.IsNullOrWhiteSpace(idNumberToSet))
        {
            existing.UpdateIdNumber(idNumberToSet);
        }

        await _userRepository.UpdateAsync(existing);
        return _mapper.Map<PersonViewModel>(existing);
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
        var account = new Account(request.UserId, newAccountNumber, request.Balance, request.AccountType);
        await _accountRepository.AddAsync(account);
        return _mapper.Map<AccountViewModel>(account);
    }
}

public class TransactionHandler : IRequestHandler<CreateTransactionCommand, TransactionViewModel>, IRequestHandler<UpdateTransactionCommand, TransactionViewModel>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public TransactionHandler(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<TransactionViewModel> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        // create transaction domain object with provided transaction date
        var transaction = new Transaction(request.AccountId, request.Amount, request.TransactionDate, request.Description);

        // load the account and apply domain behavior
        var account = await _accountRepository.GetByIdAsync(request.AccountId);
        if (account == null) throw new InvalidOperationException("Account not found.");

        account.AddTransaction(transaction);

        // persist account (which will persist the new transaction)
        await _accountRepository.UpdateAsync(account);

        return _mapper.Map<TransactionViewModel>(transaction);
    }

    public async Task<TransactionViewModel> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        // load the existing transaction
        var existingTxn = await _accountRepository.GetTransactionByIdAsync(request.Id);
        if (existingTxn == null) throw new InvalidOperationException("Transaction not found.");

        // update values (this will refresh CaptureDate)
        existingTxn.Update(request.Amount, request.TransactionDate, request.Description);

        // persist via repository
        await _accountRepository.UpdateTransactionAsync(existingTxn);

        return _mapper.Map<TransactionViewModel>(existingTxn);
    }
}

public class DeletePersonHandler : IRequestHandler<DeletePersonCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public DeletePersonHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        await _userRepository.DeleteAsync(request.Id);
        return true;
    }
}

// IAccountRepository.cs
using Domain.Entities;

namespace Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id);
    Task<IEnumerable<Account>> GetAllAsync();
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
    Task CloseAsync(Guid id);
    Task<string> GetNextAccountNumberAsync();
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task UpdateTransactionAsync(Transaction transaction);
}

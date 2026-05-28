// ITransactionRepository.cs
using Domain.Entities;

namespace Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
}

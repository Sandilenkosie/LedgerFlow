// IAccountRepository.cs
using Domain.Entities;

namespace Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id);
    Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
    Task CloseAsync(Guid id);
}

using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task<IEnumerable<User>> GetAllAsync(int page, int pageSize);
    Task UpdateAsync(Guid personId, User user);
    Task DeleteAsync(Guid id);
}

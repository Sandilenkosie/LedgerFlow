using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<IEnumerable<User>> GetAllAsync(int page, int pageSize);
    Task UpdateAsync(Guid userId, User user);
    Task DeleteAsync(Guid id);
}

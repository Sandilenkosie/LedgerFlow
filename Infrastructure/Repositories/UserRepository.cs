using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await _context.Users
                .Include(u => u.Accounts)
                    .ThenInclude(a => a.Transactions)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existingUser is not null)
            {
                existingUser.Name = user.Name;
                existingUser.UpdateUsername(user.Username);
                existingUser.UpdateIdNumber(user.IdNumber);
                existingUser.PasswordHash = user.PasswordHash;
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users
                .Include(u => u.Accounts)
                    .ThenInclude(a => a.Transactions)
                .FirstOrDefaultAsync(u => u.Username == username);

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _context.Users
                .Include(u => u.Accounts)
                    .ThenInclude(a => a.Transactions)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task UpdateAsync(Guid personId,User user) 
        {
            var existingUser = await _context.Users
                .Include(u => u.Accounts)
                    .ThenInclude(a => a.Transactions)
                .FirstOrDefaultAsync(u => u.Id == personId);
            if (existingUser is not null)
            {
                existingUser.Name = user.Name;
                existingUser.UpdateUsername(user.Username);
                existingUser.UpdateIdNumber(user.IdNumber);
                existingUser.PasswordHash = user.PasswordHash;
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is not null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize) =>
              await _context.Users
                    .Include(u => u.Accounts)
                        .ThenInclude(a => a.Transactions)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
    }
}

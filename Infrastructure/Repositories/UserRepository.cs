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

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.Include(a => a.Accounts).FirstOrDefaultAsync(u => u.Username == username);

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid userId, User user) 
        {
            var existingUser = await _context.Users.Include(a => a.Accounts).FirstOrDefaultAsync(u => u.Id == userId);
            if (existingUser is not null)
            {
                existingUser.Name = user.Name;
                existingUser.UpdateUsername(user.Username);
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
              await _context.Users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}

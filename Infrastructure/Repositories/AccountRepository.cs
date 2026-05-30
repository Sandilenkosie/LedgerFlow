using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid id) =>
        await _context.Accounts
            .Include(a => a.Transactions)
            .Include(a => a.Person)
            .Include(a => a.Status)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Account>> GetAllAsync() =>
        await _context.Accounts
            .Include(a => a.Transactions)
            .Include(a => a.Person)
            .Include(a => a.Status)
            .ToListAsync();

    public async Task AddAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GetNextAccountNumberAsync()
    {
        // Account number format: ACC-{year}-{sequence:000}
        var year = DateTime.UtcNow.Year;
        var latest = await _context.Accounts
            .OrderByDescending(a => a.Id)
            .Select(a => a.AccountNumber)
            .FirstOrDefaultAsync();

        var prefix = $"ACC-{year}-";
        int nextSeq = 1;

        if (!string.IsNullOrEmpty(latest) && latest.StartsWith(prefix))
        {
            var seqPart = latest.Substring(prefix.Length);
            if (int.TryParse(seqPart, out var seq))
            {
                nextSeq = seq + 1;
            }
        }

        return $"{prefix}{nextSeq:000}";
    }

    public async Task UpdateAsync(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task CloseAsync(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account != null)
        {
            account.CloseAccount();
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }
}

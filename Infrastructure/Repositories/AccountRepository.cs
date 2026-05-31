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
        // Ensure the referenced Status row exists to satisfy FK constraints
        if (account.StatusId != Guid.Empty)
        {
            var statusExists = await _context.Statuses.AnyAsync(s => s.Id == account.StatusId);
            if (!statusExists)
            {
                var toAdd = account.Status ?? (account.StatusId == Status.OpenId ? Status.Open() : Status.Closed());
                _context.Statuses.Add(toAdd);
            }
        }

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
        // Load the tracked account and apply domain changes
        var existing = await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == account.Id);

        if (existing == null) throw new InvalidOperationException("Account not found when attempting update.");

        // Apply domain state changes (close/reopen) so invariants are respected
        if (existing.IsClosed != account.IsClosed)
        {
            if (account.IsClosed) existing.CloseAccount(); else existing.ReopenAccount();
        }

        // Update allowed scalar
        existing.AccountType = account.AccountType;

        // Add any new transactions that are not already tracked
        if (account.Transactions != null)
        {
            foreach (var tx in account.Transactions)
            {
                if (!existing.Transactions.Any(t => t.Id == tx.Id)) _context.Transactions.Add(tx);
            }
        }

        // Ensure Status row exists (check DB/local first)
        var statusId = existing.StatusId;
        if (statusId != Guid.Empty)
        {
            var found = await _context.Statuses.FindAsync(statusId);
            if (found == null && _context.Statuses.Local.All(s => s.Id != statusId))
            {
                _context.Statuses.Add(statusId == Status.OpenId ? Status.Open() : Status.Closed());
            }
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            var inner = ex.InnerException?.Message ?? ex.Message;
            throw new InvalidOperationException($"An error occurred while saving the entity changes: {inner}", ex);
        }
    }

    public async Task CloseAsync(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null) return;

        account.CloseAccount();

        var statusToEnsure = account.StatusId;
        if (statusToEnsure != Guid.Empty)
        {
            var found = await _context.Statuses.FindAsync(statusToEnsure);
            if (found == null && _context.Statuses.Local.All(s => s.Id != statusToEnsure))
            {
                _context.Statuses.Add(statusToEnsure == Status.OpenId ? Status.Open() : Status.Closed());
            }
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            var inner = ex.InnerException?.Message ?? ex.Message;
            throw new InvalidOperationException($"An error occurred while saving the entity changes: {inner}", ex);
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

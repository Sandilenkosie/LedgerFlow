namespace Domain.Entities;

public class Account
{
    // EF Core requires a parameterless constructor for materialization
    protected Account() { }

    public Guid Id { get; private set; }
    public string AccountNumber { get; private set; }
    public string AccountType { get; set; }
    public decimal Balance { get; private set; }
    public DateTime Created { get; private set; }
    // Legacy flag retained for quick checks. Prefer using Status / StatusId.
    public bool IsClosed { get; private set; }
    public Guid StatusId { get; private set; }
    public Status Status { get; private set; }
    public Guid UserId { get; private set; }
    public User Person { get; private set; }

    private readonly List<Transaction> _transactions = new();

    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    public Account(string accountNumber)
    {
        AccountNumber = accountNumber;
        AccountType = string.Empty;
        Balance = 0;
        IsClosed = false;
        // default to Open status
        StatusId = Status.OpenId;
        Created = DateTime.UtcNow;
    }

    public Account(Guid userId, string accountNumber, decimal initialBalance) : this(accountNumber)
    {
        UserId = userId;
        if (initialBalance < 0) throw new ArgumentException("Initial balance cannot be negative.");
        Balance = initialBalance;
    }

    public Account(Guid userId, string accountNumber, decimal initialBalance, string accountType) : this(userId, accountNumber, initialBalance)
    {
        AccountType = accountType ?? string.Empty;
    }



    public Account(Guid userId, string accountNumber) : this(accountNumber)
    {
        UserId = userId;
    }

    public Account(Guid userId, string accountNumber, string accountType) : this(userId, accountNumber)
    {
        AccountType = accountType ?? string.Empty;
    }

    public void AddTransaction(Transaction transaction)
    {
        // Do not allow transactions when account is closed. Check both the legacy flag and the Status reference.
        if (IsClosed || (Status != null && string.Equals(Status.Name, "Closed", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Cannot add transactions to a closed account.");
        if (transaction.Amount == 0) throw new InvalidOperationException("Transaction amount cannot be zero.");

        // If the user provided a negative amount, subtract its absolute value; if positive, add it.
        if (transaction.Amount < 0)
        {
            Balance -= Math.Abs(transaction.Amount);
        }
        else
        {
            Balance += transaction.Amount;
        }
        _transactions.Add(transaction);
    }

    public void UpdateTransaction(Guid transactionId, decimal amount, DateTime transactionDate, string description)
    {
        // Do not allow updates when account is closed
        if (IsClosed || (Status != null && string.Equals(Status.Name, "Closed", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Cannot update transactions on a closed account.");

        if (amount == 0) throw new InvalidOperationException("Transaction amount cannot be zero.");

        var existing = _transactions.FirstOrDefault(t => t.Id == transactionId);
        if (existing == null) throw new InvalidOperationException("Transaction not found on this account.");

        // compute balance delta and apply update through the transaction aggregate
        var delta = amount - existing.Amount;
        existing.Update(amount, transactionDate, description ?? string.Empty);
        Balance += delta;
    }

    public void CloseAccount()
    {
        if (Balance != 0) throw new InvalidOperationException("Cannot close account with non-zero balance.");
        IsClosed = true;
        // Mark status as Closed (use the well-known ClosedId)
        StatusId = Status.ClosedId;
    }

    public void ReopenAccount()
    {
        // Reopen: clear closed flag and set status to Open
        IsClosed = false;
        StatusId = Status.OpenId;
    }
}

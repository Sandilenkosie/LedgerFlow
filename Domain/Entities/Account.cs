namespace Domain.Entities;

public class Account
{
    // EF Core requires a parameterless constructor for materialization
    protected Account() { }

    public Guid Id { get; private set; }
    public string AccountNumber { get; private set; }
    public decimal Balance { get; private set; }
    public bool IsClosed { get; private set; }
    public Guid UserId { get; private set; }
    public User Person { get; private set; }

    private readonly List<Transaction> _transactions = new();

    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    public Account(string accountNumber)
    {
        AccountNumber = accountNumber;
        Balance = 0;
        IsClosed = false;
    }

    public Account(Guid userId, string accountNumber, decimal initialBalance) : this(accountNumber)
    {
        UserId = userId;
        if (initialBalance < 0) throw new ArgumentException("Initial balance cannot be negative.");
        Balance = initialBalance;
    }

    public Account(Guid userId, string accountNumber) : this(accountNumber)
    {
        UserId = userId;
    }

    public void AddTransaction(Transaction transaction)
    {
        if (IsClosed) throw new InvalidOperationException("Cannot add transactions to a closed account.");
        if (transaction.Amount == 0) throw new InvalidOperationException("Transaction amount cannot be zero.");
        Balance += transaction.Amount;
        _transactions.Add(transaction);
    }

    public void CloseAccount()
    {
        if (Balance != 0) throw new InvalidOperationException("Cannot close account with non-zero balance.");
        IsClosed = true;
    }
}

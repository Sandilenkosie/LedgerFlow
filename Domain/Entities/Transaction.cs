namespace Domain.Entities;

public class Transaction
{
    // Parameterless constructor required by EF Core for materialization
    protected Transaction() { }
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public DateTime CaptureDate { get; private set; }
    public string Description { get; private set; }

    public Guid AccountId { get; private set; }
    public Account Account { get; private set; }

    private Transaction(decimal amount, DateTime transactionDate, string description = "")
    {
        if (transactionDate > DateTime.UtcNow)
            throw new InvalidOperationException("Transaction date cannot be in the future.");

        Amount = amount;
        TransactionDate = transactionDate;
        CaptureDate = DateTime.UtcNow;
        Description = description ?? string.Empty;
    }

    public Transaction(Guid accountId, decimal amount, string description) : this(amount, DateTime.UtcNow, description)
    {
        AccountId = accountId;
    }
}

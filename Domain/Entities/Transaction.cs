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
        Id = Guid.NewGuid();
        // Compare only the date portion to avoid rejecting same-day transactions due to time-of-day or timezone differences
        if (transactionDate.Date > DateTime.UtcNow.Date)
            throw new InvalidOperationException("Transaction date cannot be in the future.");

        Amount = amount;
        TransactionDate = transactionDate;
        CaptureDate = DateTime.UtcNow;
        Description = description ?? string.Empty;
    }

    public Transaction(Guid accountId, decimal amount, DateTime transactionDate, string description) : this(amount, transactionDate, description)
    {
        AccountId = accountId;
    }

    // Update an existing transaction's key values. This refreshes the CaptureDate to now.
    public void Update(decimal amount, DateTime transactionDate, string description)
    {
        // Compare only the date portion to avoid rejecting same-day transactions due to time-of-day or timezone differences
        if (transactionDate.Date > DateTime.UtcNow.Date)
            throw new InvalidOperationException("Transaction date cannot be in the future.");

        Amount = amount;
        TransactionDate = transactionDate;
        Description = description ?? string.Empty;
        CaptureDate = DateTime.UtcNow;
    }
}

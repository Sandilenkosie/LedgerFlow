namespace Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public DateTime CaptureDate { get; private set; }

    public Guid AccountId { get; private set; }

    public Transaction(decimal amount, DateTime transactionDate)
    {
        if (transactionDate > DateTime.UtcNow)
            throw new InvalidOperationException("Transaction date cannot be in the future.");

        Amount = amount;
        TransactionDate = transactionDate;
        CaptureDate = DateTime.UtcNow;
    }
}

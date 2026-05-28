using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string? Name { get; set; }
    public string Username { get; private set; }
    public string? IdNumber { get; private set; }
    public string PasswordHash { get; set; }

    private readonly List<Account> _accounts = new();

    public IReadOnlyCollection<Account> Accounts => _accounts.AsReadOnly();


    // Full constructor — idNumber and name are optional; idNumber is validated only when provided
    public User(string username, string? idNumber, string? name, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.");

        if (!string.IsNullOrWhiteSpace(idNumber))
        {
            if (idNumber.Length != 13 || !idNumber.All(char.IsDigit))
                throw new ArgumentException("IDNumber must be 13 digits.");
            IdNumber = idNumber;
        }

        Username = username;
        Name = name;
        PasswordHash = passwordHash;
    }

    // Minimal constructor for registration when idNumber and name are not provided
    public User(string username, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.");

        Username = username;
        Name = username;
        IdNumber = null;
        PasswordHash = passwordHash;
    }

    public void UpdateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new Exception("Username is required");

        Username = username;
    }

    public void AddAccount(Account account)
    {
        if (account == null) throw new ArgumentNullException(nameof(account));
        _accounts.Add(account);
    }
}

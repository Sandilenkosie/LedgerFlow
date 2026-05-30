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

    // Minimal constructor for registration when idNumber are not provided
    public User(string username, string name, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.");

        Username = username;
        Name = name;
        IdNumber = null;
        PasswordHash = passwordHash;
    }

    public void UpdateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new Exception("Username is required");

        Username = username;
    }

    public void UpdateIdNumber(string idNumber)
    {
        if (string.IsNullOrWhiteSpace(idNumber))
            throw new Exception("ID Number is required");

        if (idNumber.Length != 13 || !idNumber.All(char.IsDigit))
            throw new Exception("ID Number must be 13 digits.");

        IdNumber = idNumber;
    }

    public void AddAccount(Account account)
    {
        if (account == null) throw new ArgumentNullException(nameof(account));
        _accounts.Add(account);
    }
}

using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModels
{
    public class PersonViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("Full Name")]
        [Required(ErrorMessage = "Full Name is required")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = null!;
        [DisplayName("ID Number")]
        [Required(ErrorMessage = "ID Number is required")]
        public string IdNumber { get; set; } = null!;

        // Related accounts
        public IEnumerable<RelatedAccountsViewModel> Accounts { get; set; } = new List<RelatedAccountsViewModel>();

    }

    public class RelatedAccountsViewModel
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = null!;
        public bool IsClosed { get; set; }
        public string AccountType { get; set; }
        public DateTime Created { get; set; }
        public string OwnerName { get; set; }
        public IEnumerable<RelatedTransactionsViewModel> Transactions { get; set; } = new List<RelatedTransactionsViewModel>();

    }

    public class RelatedTransactionsViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;

        public Guid AccountId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}

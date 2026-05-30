using Domain.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels
{
    public class AccountViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("Account Number")]
        [Required(ErrorMessage = "Account Number is required")]
        public string AccountNumber { get; set; } = null!;
        public decimal Balance { get; set; }
        public bool IsClosed { get; set; }
        public string AccountType { get; set; } = null!;
        public DateTime Created { get; set; }
        public Guid PersonId { get; set; }
        public User Person { get; set; }
        public IEnumerable<RelatedTransactionsViewModel> Transactions { get; set; } = new List<RelatedTransactionsViewModel>();
    }
}

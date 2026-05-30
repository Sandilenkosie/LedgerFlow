using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModels
{
    public class TransactionViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }

        [DisplayName("Transaction Date")]
        public DateTime TransactionDate { get; set; }
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
    }
}

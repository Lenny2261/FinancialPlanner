using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class TransactionViewModel
    {
        public Account account { get; set; }
        public ICollection<Transaction> transactions { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class BudgetViewModel
    {
        public Account account { get; set; }
        public Budget budget { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class BudgetCategories
    {

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int BudgetId { get; set; }
        public double AmountDedicated { get; set; }

        public virtual Categories Category { get; set; }
        public virtual Budget Budget { get; set; }

    }
}
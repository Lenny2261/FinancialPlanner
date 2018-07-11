using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Budget
    {

        public Budget()
        {
            BudgetCategories = new HashSet<BudgetCategories>();
        }

        public int Id { get; set; }
        public double Amount { get; set; }

        public virtual ICollection<BudgetCategories> BudgetCategories { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Categories
    {

        public Categories()
        {
            this.SubCategories = new HashSet<SubCategories>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SubCategories> SubCategories { get; set; }
    }
}
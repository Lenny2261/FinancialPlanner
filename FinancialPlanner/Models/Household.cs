using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Household
    {

        public Household()
        {
            this.applicationUsers = new HashSet<ApplicationUser>();
            this.accounts = new HashSet<Account>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ApplicationUser> applicationUsers { get; set; }
        public ICollection<Account> accounts { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class JoinNotifications
    {

        public int Id { get; set; }
        public string Message { get; set; }
        public int HouseholdId { get; set; }
        public string UserId { get; set; }
        public bool seen { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Household Household { get; set; }
    }
}
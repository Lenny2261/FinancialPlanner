using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinancialPlanner.Models;
using Microsoft.AspNet.Identity;

namespace FinancialPlanner.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Users
        [Authorize]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);
            var householdUsers = db.Users.Where(u => u.Household.applicationUsers.Select(h => h.HouseholdId).Contains(currentUser.HouseholdId));
            return View(db.Users.Except(householdUsers).ToList());
        }
    }
}
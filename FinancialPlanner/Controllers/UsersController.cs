using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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


        public ActionResult Accept(int? Id, int? houseId)
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var notify = db.joinNotifications.Find(Id);

            if (currentUser.HouseholdId != null)
            {
                TempData["NotificationId"] = Id;
                TempData["JoiningHouse"] = houseId;
                TempData["InHouse"] = "Yes";
                return RedirectToAction("Index", "JoinNotifications");
            }

            notify.seen = true;
            currentUser.HouseholdId = houseId;
            db.SaveChanges();
            return RedirectToAction("Details", "Households");
        }

        public ActionResult LeaveJoin()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var notify = db.joinNotifications.Find((int)TempData["NotificationId"]);

            if (TempData["JoiningHouse"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            currentUser.HouseholdId = (int)TempData["JoiningHouse"];
            notify.seen = true;
            db.SaveChanges();
            return RedirectToAction("Details", "Households", new { id = currentUser.HouseholdId });
        }

        public ActionResult Reject(int? Id)
        {
            var notify = db.joinNotifications.Find(Id);
            notify.seen = true;
            db.SaveChanges();
            return RedirectToAction("Index", "JoinNotifications");
        }
    }
}
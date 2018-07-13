using FinancialPlanner.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinancialPlanner.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(bool? intent, bool? malIntent)
        {

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var currentUser = db.Users.Find(userId);
                if (currentUser.HouseholdId != null && intent == null)
                {
                    return RedirectToAction("Details", "Households");
                }
            }

            if(malIntent == true)
            {
                TempData["MalIntent"] = "True";
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
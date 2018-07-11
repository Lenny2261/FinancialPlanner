using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPlanner.Models;
using Microsoft.AspNet.Identity;
using FinancialPlanner.Helpers;

namespace FinancialPlanner.Controllers
{

    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        [Authorize]
        public ActionResult Index()
        {
            return View(db.households.ToList());
        }

        // GET: Households/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());

            if (id == null && currentUser.HouseholdId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.households.Find(currentUser.HouseholdId);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                var role = new RoleHelper();
                var currentUser = db.Users.Find(User.Identity.GetUserId());

                if(currentUser.HouseholdId != null)
                {

                    if (User.IsInRole("Head"))
                    {
                        TempData["InHouse"] = "Head";
                        return RedirectToAction("Create", "Households");
                    }

                    TempData["JoiningHouse"] = household.Id;
                    TempData["InHouse"] = "Yes";
                    return RedirectToAction("Create", "Households");
                }

                if(!User.IsInRole("Admin"))
                {
                    role.AddUserToRole(User.Identity.GetUserId(), "Head");
                }
                currentUser.HouseholdId = household.Id;
                db.households.Add(household);
                db.SaveChanges();
                return RedirectToAction("Details", "Households", new { household.Id });
            }

            return View(household);
        }

        public ActionResult LeaveJoin()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());

            if (TempData["JoiningHouse"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            currentUser.HouseholdId = (int)TempData["JoiningHouse"];
            db.SaveChanges();
            return RedirectToAction("Details", "Households", new { id = currentUser.HouseholdId });
        }

        // GET: Households/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.households.Find(id);
            db.households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult _HouseholdView()
        {
            return View(db.Users.Find(User.Identity.GetUserId()));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

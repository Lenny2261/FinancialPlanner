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

namespace FinancialPlanner.Controllers
{
    [Authorize(Roles = "Admin,Head")]
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var accounts = db.accounts.Include(a => a.AccountType).Include(a => a.Budget).Include(a => a.Household);
            return View(accounts.ToList());
        }

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            Account account = db.accounts.Find(id);
            if (account == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            var currentUser = db.Users.Find(User.Identity.GetUserId());

            if (currentUser.HouseholdId != account.HouseholdId)
            {
                return RedirectToAction("Index", "Home", new { intent = true, malIntent = true });
            }

            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            ViewBag.AccountTypeId = new SelectList(db.accountTypes, "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.budgets, "Id", "Id");
            ViewBag.HouseholdId = new SelectList(db.households, "Id", "Name");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Balance,Created,InterestRate,AccountTypeId,BudgetId,HouseholdId")] Account account)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var currentUser = db.Users.Find(userId);
                var budget = new Budget();

                var count = db.budgets.Count();
                budget.Id = count + 1;
                budget.Amount = 0;
                db.budgets.Add(budget);

                account.BudgetId = budget.Id;
                account.Created = DateTimeOffset.Now;
                account.HouseholdId = (int)currentUser.HouseholdId;
                account.CurrentBalance = account.Balance;
                account.IsDeleted = false;
                db.accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Details", "Households");
            }

            ViewBag.AccountTypeId = new SelectList(db.accountTypes, "Id", "Name", account.AccountTypeId);
            ViewBag.BudgetId = new SelectList(db.budgets, "Id", "Id", account.BudgetId);
            ViewBag.HouseholdId = new SelectList(db.households, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            Account account = db.accounts.Find(id);
            if (account == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            var currentUser = db.Users.Find(User.Identity.GetUserId());

            if (currentUser.HouseholdId != account.HouseholdId)
            {
                return RedirectToAction("Index", "Home", new { intent = true, malIntent = true });
            }

            ViewBag.AccountTypeId = new SelectList(db.accountTypes, "Id", "Name", account.AccountTypeId);
            ViewBag.BudgetId = new SelectList(db.budgets, "Id", "Id", account.BudgetId);
            ViewBag.HouseholdId = new SelectList(db.households, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Balance,Created,InterestRate,AccountTypeId,BudgetId,HouseholdId")] Account account)
        {
            if (ModelState.IsValid)
            {

                account.Id = (int)TempData["Id"];
                account.HouseholdId = (int)TempData["Household"];
                account.BudgetId = (int?)TempData["Budget"];
                account.Balance = (double)TempData["Balance"];
                account.CurrentBalance = (double)TempData["CBalance"];
                account.Created = (DateTimeOffset)TempData["Created"];
                account.IsDeleted = false;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Households");
            }
            ViewBag.AccountTypeId = new SelectList(db.accountTypes, "Id", "Name", account.AccountTypeId);
            ViewBag.BudgetId = new SelectList(db.budgets, "Id", "Id", account.BudgetId);
            ViewBag.HouseholdId = new SelectList(db.households, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            Account account = db.accounts.Find(id);
            if (account == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            var currentUser = db.Users.Find(User.Identity.GetUserId());

            if (currentUser.HouseholdId != account.HouseholdId)
            {
                return RedirectToAction("Index", "Home", new { intent = true, malIntent = true });
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.accounts.Find(id);
            account.IsDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Details", "Households", new { id = account.HouseholdId });
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

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
    public class BudgetCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetCategories
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var budgetCategories = db.BudgetCategories.Include(b => b.Budget).Include(b => b.Category);
            return View(budgetCategories.ToList());
        }

        // GET: BudgetCategories/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetCategories budgetCategories = db.BudgetCategories.Find(id);
            if (budgetCategories == null)
            {
                return HttpNotFound();
            }
            return View(budgetCategories);
        }

        // GET: BudgetCategories/Create
        public ActionResult Create(int id, int accountId)
        {
            TempData["BudgetId"] = id;
            TempData["AccountId"] = accountId;
            List<Categories> less = new List<Categories>();
            var budget = db.BudgetCategories.Where(b => b.BudgetId == id).ToList();
            foreach(var item in budget)
            {
                less.Add(db.categories.Where(c => c.Id == item.CategoryId).FirstOrDefault());
            }
            ViewBag.BudgetId = new SelectList(db.budgets, "Id", "Id");
            if(less == null)
            {
                ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name");
            }
            else
            {
                ViewBag.CategoryId = new SelectList(db.categories.ToList().Except(less), "Id", "Name");
            }

            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var account = db.accounts.Find(accountId);

            if (currentUser.HouseholdId != account.HouseholdId)
            {
                return RedirectToAction("Index", "Home", new { intent = true, malIntent = true });
            }

            return View();
        }

        // POST: BudgetCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryId,BudgetId,AmountDedicated")] BudgetCategories budgetCategories)
        {
            if (ModelState.IsValid)
            {
                var budgetId = (int)TempData["BudgetId"];
                var budget = db.budgets.Find(budgetId);
                var accountId = db.accounts.Where(a => a.BudgetId == budgetId).FirstOrDefault();

                budget.Amount = budget.Amount + budgetCategories.AmountDedicated;
                budgetCategories.BudgetId = budgetId;
                db.BudgetCategories.Add(budgetCategories);
                db.SaveChanges();
                return RedirectToAction("Index", "Budgets", new { accountId.Id });
            }

            ViewBag.BudgetId = new SelectList(db.budgets, "Id", "Id", budgetCategories.BudgetId);
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", budgetCategories.CategoryId);
            return View(budgetCategories);
        }

        // GET: BudgetCategories/Edit/5
        public ActionResult Edit(int? id, int? accountId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetCategories budgetCategories = db.BudgetCategories.Find(id);
            if (budgetCategories == null)
            {
                return HttpNotFound();
            }

            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var account = db.accounts.Find(accountId);

            if (currentUser.HouseholdId != account.HouseholdId)
            {
                return RedirectToAction("Index", "Home", new { intent = true, malIntent = true });
            }

            ViewBag.BudgetId = new SelectList(db.budgets, "Id", "Id", budgetCategories.BudgetId);
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", budgetCategories.CategoryId);
            return View(budgetCategories);
        }

        // POST: BudgetCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,BudgetId,AmountDedicated")] BudgetCategories budgetCategories)
        {
            if (ModelState.IsValid)
            {
                int budgetId = (int)TempData["BudgetId"];
                var budget = db.budgets.Find(budgetId);
                var accountId = db.accounts.Where(a => a.BudgetId == budgetId).FirstOrDefault();

                budget.Amount = budget.Amount - (double)TempData["CurrentAmount"];
                budget.Amount = budget.Amount + budgetCategories.AmountDedicated;
                budgetCategories.BudgetId = budgetId;
                budgetCategories.Id = (int)TempData["Id"];
                budgetCategories.CategoryId = (int)TempData["CategoryId"];
                db.Entry(budgetCategories).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Budgets", new { accountId.Id });
            }
            ViewBag.BudgetId = new SelectList(db.budgets, "Id", "Id", budgetCategories.BudgetId);
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", budgetCategories.CategoryId);
            return View(budgetCategories);
        }

        // GET: BudgetCategories/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetCategories budgetCategories = db.BudgetCategories.Find(id);
            if (budgetCategories == null)
            {
                return HttpNotFound();
            }
            return View(budgetCategories);
        }

        // POST: BudgetCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetCategories budgetCategories = db.BudgetCategories.Find(id);
            db.BudgetCategories.Remove(budgetCategories);
            db.SaveChanges();
            return RedirectToAction("Index");
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

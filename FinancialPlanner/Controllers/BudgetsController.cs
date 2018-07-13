using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPlanner.Models;

namespace FinancialPlanner.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        [Authorize]
        public ActionResult Index(int id)
        {
            var budgetId = db.accounts.Find(id);
            var model = new BudgetViewModel
            {
                account = db.accounts.Find(id),
                budget = db.budgets.Find(budgetId.BudgetId)
            };

            DateTime current = DateTime.Now;
            DateTime check;
            var days = 30 - current.Day;
            double percent = 0;
            double total = 0, util = 0, fun = 0, shop = 0;
            if (days == -1)
            {
                check = current.AddDays(-30);
            }
            else if (current.Month == 2)
            {
                check = current.AddDays(-current.Day + 1);
            }
            else
            {
                check = current.AddDays(-current.Day + 1);
            }
            var transactions = model.account.Transactions.Where(t => t.Date > check).Where(t => t.Account.Budget.BudgetCategories.Select(b => b.CategoryId).Contains(t.SubCategory.CategoryId)).Where(t => t.TransactionType.Name == "Debit").Where(t => t.TransactionStatus.Name != "Void");

            foreach (var item in transactions)
            {
                total = total + item.Amount;

                if(item.SubCategory.Category.Name == "Utilities")
                {
                    util = util + item.Amount;
                }
                else if(item.SubCategory.Category.Name == "Daily Life")
                {
                    shop = shop + item.Amount;
                }
                else
                {
                    fun = fun + item.Amount;
                }
            }
            if (total > 0)
            {
                percent = ((total / model.budget.Amount) * 100);
            }

            TempData["Percent"] = percent.ToString("0.##");
            TempData["Shop"] = shop;
            TempData["Util"] = util;
            TempData["Fun"] = fun;
            TempData["Rest"] = model.budget.Amount - shop - util - fun;

            int count = 1;

            foreach(var item in db.subCategories)
            {
                TempData["SubCat" + count] = 0;
                var subTransactions = model.account.Transactions.Where(t => t.Date > check).Where(t => t.SubCategoryId == item.Id).Where(t => t.TransactionType.Name == "Debit").Where(t => t.TransactionStatus.Name != "Void").ToList();
                foreach(var item2 in subTransactions)
                {
                    var subTotal = TempData["SubCat" + count];
                    TempData["SubCat" + count] = Convert.ToDouble(subTotal) + item2.Amount;
                }
                count++;
            }

            foreach(var item in model.budget.BudgetCategories)
            {
                switch (item.Category.Name)
                {
                    case ("Fun Cash"):
                        TempData["FBudget"] = item.AmountDedicated;
                        break;
                    case ("Daily Life"):
                        TempData["DBudget"] = item.AmountDedicated;
                        break;
                    case ("Utilities"):
                        TempData["UBudget"] = item.AmountDedicated;
                        break;
                    default:
                    break;
                }
            }

            return View(model);
        }

        // GET: Budgets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // GET: Budgets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Amount")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(budget);
        }

        // GET: Budgets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Amount")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.budgets.Find(id);
            db.budgets.Remove(budget);
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

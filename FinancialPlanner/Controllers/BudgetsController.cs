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
    [Authorize]
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        public ActionResult Index(int id)
        {
            string[] colorArr = { "#4bc5ea", "" };
            TempData["CatCount"] = db.categories.Count();
            var budgetId = db.accounts.Find(id);
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var model = new BudgetViewModel
            {
                account = db.accounts.Find(id),
                budget = db.budgets.Find(budgetId.BudgetId)
            };

            if (currentUser.HouseholdId != model.account.HouseholdId)
            {
                return RedirectToAction("Index", "Home", new { intent = true, malIntent = true });
            }

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
            List<string> categoryNames = new List<string>();
            List<double> amountSpent = new List<double>();
            TempData["Shop"] = shop;
            TempData["Util"] = util;
            TempData["Fun"] = fun;
            TempData["Rest"] = model.budget.Amount - shop - util - fun;

            foreach(var item in model.budget.BudgetCategories.ToList())
            {
                categoryNames.Add(item.Category.Name);
                double catTotal = 0;
                TempData[item.Category.Name] = item.Category.Name;
                TempData[item.Category.Name + "1"] = "";
                TempData[item.Category.Name + "L"] = "";
                TempData[item.Category.Name + "C"] = item.Category.color;

                if(item.Id != model.budget.BudgetCategories.LastOrDefault().Id)
                {
                    TempData["PieColor"] = (string)TempData["PieColor"] + "\'" + item.Category.color + "\',";
                }
                else
                {
                    TempData["PieColor"] = (string)TempData["PieColor"] + "\'" + item.Category.color + "\'";
                }

                var subCats = db.subCategories.Where(s => s.CategoryId == item.CategoryId).ToList();

                if(subCats == null)
                {
                    TempData[item.Category.Name + "1"] = (string)TempData[item.Category.Name + "1"] + item.AmountDedicated;
                    TempData[item.Category.Name + "L"] = TempData[item.Category.Name + "L"] + "\"Budget\"";
                }
                else
                {
                    TempData[item.Category.Name + "1"] = (string)TempData[item.Category.Name + "1"] + item.AmountDedicated + ",";
                    TempData[item.Category.Name + "L"] = TempData[item.Category.Name + "L"] + "\"Budget\"" + ",";
                }

                foreach(var item2 in subCats)
                {
                    double subTotal = 0;
                    var subTransactions = model.account.Transactions.Where(t => t.Date > check && t.SubCategoryId == item2.Id && t.TransactionType.Name == "Debit" && t.TransactionStatus.Name != "Void").ToList();
                    foreach(var item3 in subTransactions)
                    {
                        subTotal = subTotal + item3.Amount;
                    }

                    if(item2.Id == subCats.Last().Id)
                    {
                        TempData[item.Category.Name + "1"] = (string)TempData[item.Category.Name + "1"] + subTotal;
                        TempData[item.Category.Name + "L"] = (string)TempData[item.Category.Name + "L"] + "\"" + item2.Name + "\"";
                    }
                    else
                    {
                        TempData[item.Category.Name + "1"] = (string)TempData[item.Category.Name + "1"] + subTotal + ",";
                        TempData[item.Category.Name + "L"] = (string)TempData[item.Category.Name + "L"] + "\"" + item2.Name + "\",";
                    }
                    catTotal = catTotal + subTotal;
                }
                amountSpent.Add(catTotal);
            }

            double amountLeft = 0;
            for (var index = 0; index < categoryNames.Count(); index++)
            {
                amountLeft = amountLeft + amountSpent[index];
            }

            amountLeft = model.budget.Amount - amountLeft;

            TempData["PieLable"] = "\"Amount Left\",";
            TempData["PieData"] = amountLeft + ",";

            for(var index = 0; index < categoryNames.Count(); index++)
            {
                TempData["PieLable"] = (string)TempData["PieLable"] + "\"" + categoryNames[index] + "\",";
                TempData["PieData"] = (string)TempData["PieData"] + amountSpent[index] + ",";
            }

            return View(model);
        }

        // GET: Budgets/Details/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index(int id)
        {
            var transactions = db.transactions.Include(t => t.Account).Include(t => t.SubCategory).Include(t => t.TransactionStatus).Include(t => t.TransactionType);

            var model = new TransactionViewModel
            {
                account = db.accounts.Find(id),
                transactions = transactions.Where(t => t.AccountId == id).ToList()
            };

            return View(model);
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create(int id)
        {
            TempData["AccountId"] = id;

            ViewBag.AccountId = new SelectList(db.accounts, "Id", "Name");
            ViewBag.SubCategoryId = new SelectList(db.subCategories, "Id", "Name");
            ViewBag.TransactionStatusId = new SelectList(db.transactionStatuses, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.transactionTypes.Where(t => t.Name != "Void"), "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,From,Date,Amount,AccountId,TransactionTypeId,SubCategoryId,TransactionStatusId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var currentUser = db.Users.Find(User.Identity.GetUserId());
                var accountBelong = db.accounts.Find((int)TempData["AccountId"]);
                var type = db.transactionTypes.Find(transaction.TransactionTypeId);

                if(type.Name == "Debit")
                {
                    accountBelong.CurrentBalance = accountBelong.CurrentBalance - transaction.Amount;
                    if(accountBelong.CurrentBalance < 0)
                    {
                        TempData["Overdraft"] = "Over";
                    }
                }
                else if(type.Name == "Credit")
                {
                    accountBelong.CurrentBalance = accountBelong.CurrentBalance + transaction.Amount;
                }

                transaction.AccountId = accountBelong.Id;
                db.transactions.Add(transaction);
                db.Entry(accountBelong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Transactions", new { id = accountBelong.Id });
            }

            ViewBag.AccountId = new SelectList(db.accounts, "Id", "Name", transaction.AccountId);
            ViewBag.SubCategoryId = new SelectList(db.subCategories, "Id", "Name", transaction.SubCategoryId);
            ViewBag.TransactionStatusId = new SelectList(db.transactionStatuses, "Id", "Name", transaction.TransactionStatusId);
            ViewBag.TransactionTypeId = new SelectList(db.transactionTypes, "Id", "Name", transaction.TransactionTypeId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.accounts, "Id", "Name", transaction.AccountId);
            ViewBag.SubCategoryId = new SelectList(db.subCategories, "Id", "Name", transaction.SubCategoryId);
            ViewBag.TransactionStatusId = new SelectList(db.transactionStatuses, "Id", "Name", transaction.TransactionStatusId);
            ViewBag.TransactionTypeId = new SelectList(db.transactionTypes, "Id", "Name", transaction.TransactionTypeId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,From,Date,Amount,AccountId,TransactionTypeId,SubCategoryId,TransactionStatusId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var currentAccount = db.accounts.Find((int)TempData["Account"]);
                var currentAmount = (double)TempData["CurrentAmount"];
                var currentType = db.transactionTypes.Find((int)TempData["Type"]);
                var type = db.transactionTypes.Find(transaction.TransactionTypeId);

                if(currentAmount != transaction.Amount)
                {
                    if(currentType.Name == type.Name)
                    {
                        if(type.Name == "Debit")
                        {
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance + currentAmount;
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance - transaction.Amount;
                            if (currentAccount.CurrentBalance < 0)
                            {
                                TempData["Overdraft"] = "Over";
                            }
                        }
                        else
                        {
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance - currentAmount;
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance + transaction.Amount;
                        }
                    }
                    else
                    {
                        if (type.Name == "Debit")
                        {
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance - currentAmount;
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance - transaction.Amount;
                            if (currentAccount.CurrentBalance < 0)
                            {
                                TempData["Overdraft"] = "Over";
                            }
                        }
                        else
                        {
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance + currentAmount;
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance + transaction.Amount;
                        }
                    }
                }
                else
                {
                    if(currentType.Name != type.Name)
                    {
                        if (type.Name == "Debit")
                        {
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance - currentAmount;
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance - transaction.Amount;
                            if(currentAccount.CurrentBalance < 0)
                            {
                                TempData["Overdraft"] = "Over";
                            }
                        }
                        else
                        {
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance + currentAmount;
                            currentAccount.CurrentBalance = currentAccount.CurrentBalance + transaction.Amount;
                        }
                    }
                }

                transaction.Id = (int)TempData["Id"];
                transaction.AccountId = (int)TempData["Account"];
                transaction.Date = (DateTimeOffset)TempData["Date"];
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Transactions", new { id = currentAccount.Id });
            }
            ViewBag.AccountId = new SelectList(db.accounts, "Id", "Name", transaction.AccountId);
            ViewBag.SubCategoryId = new SelectList(db.subCategories, "Id", "Name", transaction.SubCategoryId);
            ViewBag.TransactionStatusId = new SelectList(db.transactionStatuses, "Id", "Name", transaction.TransactionStatusId);
            ViewBag.TransactionTypeId = new SelectList(db.transactionTypes, "Id", "Name", transaction.TransactionTypeId);
            return View(transaction);
        }

        public ActionResult Void(int id)
        {
            var transaction = db.transactions.Find(id);
            var currentAccount = db.accounts.Find(transaction.AccountId);
            var statusChange = db.transactionStatuses.Where(s => s.Name == "Void").FirstOrDefault();
            var type = db.transactionTypes.Find(transaction.TransactionTypeId);

            if (type.Name == "Debit")
            {
                currentAccount.CurrentBalance = currentAccount.CurrentBalance + transaction.Amount;
            }
            else
            {
                currentAccount.CurrentBalance = currentAccount.CurrentBalance - transaction.Amount;
            }

            transaction.TransactionStatusId = statusChange.Id;

            db.Entry(transaction).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Transactions", new { id = transaction.AccountId });
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.transactions.Find(id);
            db.transactions.Remove(transaction);
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

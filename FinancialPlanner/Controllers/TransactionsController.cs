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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.transactions.Include(t => t.Account).Include(t => t.SubCategory).Include(t => t.TransactionStatus).Include(t => t.TransactionType);
            return View(transactions.ToList());
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
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.accounts, "Id", "Name");
            ViewBag.SubCategoryId = new SelectList(db.subCategories, "Id", "Name");
            ViewBag.TransactionStatusId = new SelectList(db.transactionStatuses, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.transactionTypes, "Id", "Name");
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
                db.transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
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
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.accounts, "Id", "Name", transaction.AccountId);
            ViewBag.SubCategoryId = new SelectList(db.subCategories, "Id", "Name", transaction.SubCategoryId);
            ViewBag.TransactionStatusId = new SelectList(db.transactionStatuses, "Id", "Name", transaction.TransactionStatusId);
            ViewBag.TransactionTypeId = new SelectList(db.transactionTypes, "Id", "Name", transaction.TransactionTypeId);
            return View(transaction);
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

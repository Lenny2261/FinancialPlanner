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
    public class TransactionStatusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TransactionStatus
        public ActionResult Index()
        {
            return View(db.transactionStatuses.ToList());
        }

        // GET: TransactionStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionStatus transactionStatus = db.transactionStatuses.Find(id);
            if (transactionStatus == null)
            {
                return HttpNotFound();
            }
            return View(transactionStatus);
        }

        // GET: TransactionStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TransactionStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] TransactionStatus transactionStatus)
        {
            if (ModelState.IsValid)
            {
                db.transactionStatuses.Add(transactionStatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(transactionStatus);
        }

        // GET: TransactionStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionStatus transactionStatus = db.transactionStatuses.Find(id);
            if (transactionStatus == null)
            {
                return HttpNotFound();
            }
            return View(transactionStatus);
        }

        // POST: TransactionStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] TransactionStatus transactionStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transactionStatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(transactionStatus);
        }

        // GET: TransactionStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionStatus transactionStatus = db.transactionStatuses.Find(id);
            if (transactionStatus == null)
            {
                return HttpNotFound();
            }
            return View(transactionStatus);
        }

        // POST: TransactionStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TransactionStatus transactionStatus = db.transactionStatuses.Find(id);
            db.transactionStatuses.Remove(transactionStatus);
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

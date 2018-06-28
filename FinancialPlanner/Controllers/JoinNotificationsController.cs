﻿using System;
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
    public class JoinNotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: JoinNotifications
        public ActionResult Index()
        {
            var joinNotifications = db.joinNotifications.Include(j => j.Household);
            return View(joinNotifications.ToList());
        }

        // GET: JoinNotifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JoinNotifications joinNotifications = db.joinNotifications.Find(id);
            if (joinNotifications == null)
            {
                return HttpNotFound();
            }
            return View(joinNotifications);
        }

        // GET: JoinNotifications/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.households, "Id", "Name");
            return View();
        }

        // POST: JoinNotifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Message,HouseholdId,UserId")] JoinNotifications joinNotifications)
        {
            if (ModelState.IsValid)
            {
                db.joinNotifications.Add(joinNotifications);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.households, "Id", "Name", joinNotifications.HouseholdId);
            return View(joinNotifications);
        }

        // GET: JoinNotifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JoinNotifications joinNotifications = db.joinNotifications.Find(id);
            if (joinNotifications == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.households, "Id", "Name", joinNotifications.HouseholdId);
            return View(joinNotifications);
        }

        // POST: JoinNotifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Message,HouseholdId,UserId")] JoinNotifications joinNotifications)
        {
            if (ModelState.IsValid)
            {
                db.Entry(joinNotifications).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.households, "Id", "Name", joinNotifications.HouseholdId);
            return View(joinNotifications);
        }

        // GET: JoinNotifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JoinNotifications joinNotifications = db.joinNotifications.Find(id);
            if (joinNotifications == null)
            {
                return HttpNotFound();
            }
            return View(joinNotifications);
        }

        // POST: JoinNotifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JoinNotifications joinNotifications = db.joinNotifications.Find(id);
            db.joinNotifications.Remove(joinNotifications);
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

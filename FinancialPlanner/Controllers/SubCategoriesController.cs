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
    [Authorize]
    public class SubCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SubCategories
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var subCategories = db.subCategories.Include(s => s.Category);
            return View(subCategories.ToList());
        }

        // GET: SubCategories/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategories subCategories = db.subCategories.Find(id);
            if (subCategories == null)
            {
                return HttpNotFound();
            }
            return View(subCategories);
        }

        // GET: SubCategories/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name");
            return View();
        }

        // POST: SubCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CategoryId")] SubCategories subCategories, string returnLoc, int accountId)
        {
            if (ModelState.IsValid)
            {
                db.subCategories.Add(subCategories);
                db.SaveChanges();
                return RedirectToAction("Index", returnLoc, new { id = accountId });
            }

            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", subCategories.CategoryId);
            return View(subCategories);
        }

        // GET: SubCategories/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategories subCategories = db.subCategories.Find(id);
            if (subCategories == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", subCategories.CategoryId);
            return View(subCategories);
        }

        // POST: SubCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CategoryId")] SubCategories subCategories)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subCategories).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name", subCategories.CategoryId);
            return View(subCategories);
        }

        // GET: SubCategories/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategories subCategories = db.subCategories.Find(id);
            if (subCategories == null)
            {
                return HttpNotFound();
            }
            return View(subCategories);
        }

        // POST: SubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubCategories subCategories = db.subCategories.Find(id);
            db.subCategories.Remove(subCategories);
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

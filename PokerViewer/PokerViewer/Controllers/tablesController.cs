using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PokerViewer.Models;
using PagedList;

namespace PokerViewer.Controllers
{
    public class tablesController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: tables
		public ActionResult Index(int? itemsPerPage, int? page)
        {
			return View(db.tables.ToList().ToPagedList(pageNumber: page ?? 1, pageSize: itemsPerPage ?? 25));
        }

        // GET: tables/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            table table = db.tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // GET: tables/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TableID,MaxPlayers,Stakes,Site,Limit")] table table)
        {
            if (ModelState.IsValid)
            {
                db.tables.Add(table);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(table);
        }

        // GET: tables/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            table table = db.tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // POST: tables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TableID,MaxPlayers,Stakes,Site,Limit")] table table)
        {
            if (ModelState.IsValid)
            {
                db.Entry(table).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(table);
        }

        // GET: tables/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            table table = db.tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // POST: tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            table table = db.tables.Find(id);
            db.tables.Remove(table);
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
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
		public ActionResult Index(int? itemsPerPage, int? page, string sortOrder, string searchString, string currentFilter)
		{

			ViewBag.CurrentSort = sortOrder;
			ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";  //String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
			ViewBag.MaxPlayers = sortOrder == "MaxPlayers" ? "MaxPlayers_desc" : "MaxPlayers";
			ViewBag.StakesSortParm = sortOrder == "StakesSortParm" ? "StakesSortParm_desc" : "StakesSortParm";
			ViewBag.SiteSortParm = sortOrder == "SiteSortParm" ? "SiteSortParm_desc" : "SiteSortParm";
			
			ViewBag.CurrentItemsPerPage = itemsPerPage;

			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			var tables = db.tables.Where(p => p.TableName.Length>0);
			//parse search string
			if (!String.IsNullOrEmpty(searchString))
			{
				long searchID = 0;
				long.TryParse(searchString, out searchID);
				tables = tables.Where(s => s.TableID == searchID);
			}
			
			ViewBag.CurrentFilter = searchString;
			//Handle toggling between asc and desc
			switch (sortOrder)
			{
				case "id":
					tables = tables.OrderBy(s => s.TableID);
					break;

				case "id_desc":
					tables = tables.OrderByDescending(s => s.TableID);
					break;

				case "MaxPlayers":
					tables = tables.OrderBy(s => s.MaxPlayers);
					break;
				case "MaxPlayers_desc":
					tables = tables.OrderByDescending(s => s.MaxPlayers);
					break;
				case "StakesSortParm":
					tables = tables.OrderBy(s => s.Stakes);
					break;
				case "StakesSortParm_desc":
					tables = tables.OrderByDescending(s => s.Stakes);
					break;
				case "SiteSortParm":
					tables = tables.OrderBy(s => s.Site);
					break;
				case "SiteSortParm_desc":
					tables = tables.OrderByDescending(s => s.Site);
					break;

				default:
					tables = tables.OrderBy(s => s.TableID);
					break;
			}
            
			return View(tables.ToList().ToPagedList(pageNumber: page ?? 1, pageSize: itemsPerPage ?? 25));
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
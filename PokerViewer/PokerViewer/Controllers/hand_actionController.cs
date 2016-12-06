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
    public class hand_actionController : Controller
    {
        private PokerDB db = new PokerDB();

        // GET: hand_action
		public ActionResult Index(int? itemsPerPage, int? page, string sortOrder, string searchString, string currentFilter)
	
		{
			ViewBag.CurrentSort = sortOrder;
			ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";  //String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
			ViewBag.ActionIdSortParm = sortOrder == "ActionIdSortParm" ? "ActionIdSortParm_desc" : "ActionIdSortParm"; 
			ViewBag.PlayerIdSortParm = sortOrder == "PlayerIdSortParm" ? "PlayerIdSortParm_desc" : "PlayerIdSortParm";
			ViewBag.ActionNameSortParm = sortOrder == "ActionNameSortParm" ? "ActionNameSortParm_desc" : "ActionNameSortParm";
			ViewBag.StreetSortParm  = sortOrder == "StreetSortParm" ? "StreetSortParm_desc" : "StreetSortParm";
			ViewBag.AmountSortParm  = sortOrder == "AmountSortParm" ? "AmountSortParm_desc" : "AmountSortParm";
			ViewBag.CurrentItemsPerPage = itemsPerPage;


			searchStringFilter(ref page, ref searchString, currentFilter);

			var hand_action = db.hand_action.Include(h => h.hand).Include(h => h.player);
			//parse search string
			if (!String.IsNullOrEmpty(searchString))
			{
				long searchID = 0;
				long.TryParse(searchString,out searchID);
				hand_action = hand_action.Where(s => s.PlayerID == searchID);
			}
			//Handle toggling between asc and desc
			switch (sortOrder)
			{
				case "id":
					hand_action = hand_action.OrderBy(s => s.HandID);
					break;

				case "id_desc":
				hand_action = hand_action.OrderByDescending(s => s.HandID);
			break;

				case "ActionIdSortParm":
			hand_action = hand_action.OrderBy(s => s.ActionID);
			break;
				case "ActionIdSortParm_desc":
			hand_action = hand_action.OrderByDescending(s => s.ActionID);
			break;
				case "PlayerIdSortParm":
			hand_action = hand_action.OrderBy(s => s.PlayerID);
			break;
				case "PlayerIdSortParm_desc":
			hand_action = hand_action.OrderByDescending(s => s.PlayerID);
			break;
				case "ActionNameSortParm":
			hand_action = hand_action.OrderBy(s => s.ActionName);
			break;
				case "ActionNameSortParm_desc":
			hand_action = hand_action.OrderByDescending(s => s.ActionName);
			break;
				case "StreetSortParm":
			hand_action = hand_action.OrderBy(s => s.Street);
			break;
				case "StreetSortParm_desc":
			hand_action = hand_action.OrderByDescending(s => s.Street);
			break;
				case "AmountSortParm_desc":
			hand_action = hand_action.OrderByDescending(s => s.Amount);
			break;
				case "AmountSortParm":
			hand_action = hand_action.OrderBy(s => s.Amount);
			break;
				default:
	
			break;

		}
			return View(hand_action.ToList().ToPagedList(pageNumber: page ?? 1, pageSize: itemsPerPage ?? 25));

		
        }

		private void searchStringFilter(ref int? page, ref string searchString, string currentFilter)
		{
			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}
			ViewBag.CurrentFilter = searchString;
		}

		
        // GET: hand_action/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hand_action hand_action = db.hand_action.Find(id);
            if (hand_action == null)
            {
                return HttpNotFound();
            }
            return View(hand_action);
        }

        // GET: hand_action/Create
        public ActionResult Create()
        {
            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID");
            ViewBag.PlayerName = new SelectList(db.players, "Name", "Name");
            return View();
        }

        // POST: hand_action/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HandID,ActionID,PlayerName,ActionName,Street,Amount,IsPFR,IsVPIP,Is3Bet,Is4Bet")] hand_action hand_action)
        {
            if (ModelState.IsValid)
            {
                db.hand_action.Add(hand_action);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID", hand_action.HandID);
            ViewBag.PlayerID = new SelectList(db.players, "Name", "Name", hand_action.PlayerID);
            return View(hand_action);
        }

        // GET: hand_action/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hand_action hand_action = db.hand_action.Find(id);
            if (hand_action == null)
            {
                return HttpNotFound();
            }
            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID", hand_action.HandID);
            ViewBag.PlayerID = new SelectList(db.players, "Name", "Name", hand_action.PlayerID);
            return View(hand_action);
        }

        // POST: hand_action/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HandID,ActionID,PlayerName,ActionName,Street,Amount,IsPFR,IsVPIP,Is3Bet,Is4Bet")] hand_action hand_action)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hand_action).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HandID = new SelectList(db.hands, "HandID", "TableID", hand_action.HandID);
			ViewBag.PlayerID = new SelectList(db.players, "Name", "Name", hand_action.PlayerID);
            return View(hand_action);
        }

        // GET: hand_action/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hand_action hand_action = db.hand_action.Find(id);
            if (hand_action == null)
            {
                return HttpNotFound();
            }
            return View(hand_action);
        }

        // POST: hand_action/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            hand_action hand_action = db.hand_action.Find(id);
            db.hand_action.Remove(hand_action);
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